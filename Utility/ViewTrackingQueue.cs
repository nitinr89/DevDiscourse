using System.Threading.Channels;
using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Utility
{
    public record ViewTrackingItem(
        Guid DevNewsId,
        string? Ipv4,
        string? Country,
        string? City,
        string? MacAddress,
        DateTime ViewedOnUtc);

    public interface IViewTrackingQueue
    {
        bool Enqueue(ViewTrackingItem item);
    }

    public sealed class ViewTrackingQueue : BackgroundService, IViewTrackingQueue
    {
        private const int BatchSize = 200;
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(2);

        private readonly Channel<ViewTrackingItem> _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ViewTrackingQueue> _logger;

        public ViewTrackingQueue(IServiceScopeFactory scopeFactory, ILogger<ViewTrackingQueue> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _channel = Channel.CreateBounded<ViewTrackingItem>(new BoundedChannelOptions(5000)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        }

        public bool Enqueue(ViewTrackingItem item)
        {
            return _channel.Writer.TryWrite(item);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var batch = new List<ViewTrackingItem>(BatchSize);
            using var timer = new PeriodicTimer(FlushInterval);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    while (batch.Count < BatchSize && _channel.Reader.TryRead(out var item))
                    {
                        batch.Add(item);
                    }

                    if (batch.Count >= BatchSize)
                    {
                        await PersistBatchAsync(batch, stoppingToken);
                        batch.Clear();
                        continue;
                    }

                    var readTask = _channel.Reader.WaitToReadAsync(stoppingToken).AsTask();
                    var tickTask = timer.WaitForNextTickAsync(stoppingToken).AsTask();
                    var completedTask = await Task.WhenAny(readTask, tickTask);

                    if (completedTask == tickTask && batch.Count > 0)
                    {
                        await PersistBatchAsync(batch, stoppingToken);
                        batch.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // graceful shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in view tracking queue.");
            }

            // Flush any remaining items.
            while (_channel.Reader.TryRead(out var pending))
            {
                batch.Add(pending);
                if (batch.Count >= BatchSize)
                {
                    await PersistBatchAsync(batch, CancellationToken.None);
                    batch.Clear();
                }
            }
            if (batch.Count > 0)
            {
                await PersistBatchAsync(batch, CancellationToken.None);
            }
        }

        private async Task PersistBatchAsync(IReadOnlyCollection<ViewTrackingItem> batch, CancellationToken cancellationToken)
        {
            if (batch.Count == 0)
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var groupedViews = batch
                    .GroupBy(x => x.DevNewsId)
                    .Select(g => new { DevNewsId = g.Key, Count = g.Count() })
                    .ToList();

                foreach (var item in groupedViews)
                {
                    var updatedRows = await db.Database.ExecuteSqlRawAsync(
                        "UPDATE DevNewsMetaDatas SET Views = Views + @count WHERE DevNewsId = @id",
                        new SqlParameter("@count", item.Count),
                        new SqlParameter("@id", item.DevNewsId));

                    if (updatedRows == 0)
                    {
                        try
                        {
                            await db.Database.ExecuteSqlRawAsync(
                                "INSERT INTO DevNewsMetaDatas (DevNewsId, Views) VALUES (@id, @count)",
                                new SqlParameter("@id", item.DevNewsId),
                                new SqlParameter("@count", item.Count));
                        }
                        catch
                        {
                            await db.Database.ExecuteSqlRawAsync(
                                "UPDATE DevNewsMetaDatas SET Views = Views + @count WHERE DevNewsId = @id",
                                new SqlParameter("@count", item.Count),
                                new SqlParameter("@id", item.DevNewsId));
                        }
                    }
                }

                db.TrendingNews.AddRange(batch.Select(x => new TrendingNews
                {
                    Id = Guid.NewGuid(),
                    NewsId = x.DevNewsId,
                    Ipv4 = x.Ipv4,
                    Country = x.Country,
                    City = x.City,
                    MacAddress = x.MacAddress,
                    ViewedOn = x.ViewedOnUtc
                }));

                await db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist view tracking batch of {BatchCount} items.", batch.Count);
            }
        }
    }
}
