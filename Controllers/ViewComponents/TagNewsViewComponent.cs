using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class TagNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        public TagNewsViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }
        public async Task<IViewComponentResult> InvokeAsync(long id, string tag, string? sector)
        {
            string cacheKey = $"vc:tagNews:{id}:{sector ?? "all"}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var cachedResult = JsonSerializer.Deserialize<List<LatestNewsView>>(cached);
                if (cachedResult != null) return View(cachedResult);
            }

            DateTime fifteenDays = DateTime.Today.AddDays(-15);
            // Build tag list for in-memory filtering
            var tagTokens = tag.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                               .Reverse().Skip(3).Take(3).ToList();

            List<LatestNewsView> result;
            if (!string.IsNullOrEmpty(sector))
            {
                // Fetch a limited set from DB, then filter in memory by tags
                var candidates = await _db.DevNews.AsNoTracking()
                    .Where(a => a.CreatedOn > fifteenDays && a.NewsId != id && a.AdminCheck == true && a.Sector == sector)
                    .OrderByDescending(a => a.ModifiedOn)
                    .Take(100) // limit DB scan, then filter client-side
                    .Select(a => new LatestNewsView { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels })
                    .ToListAsync();

                result = candidates.Where(d => tagTokens.Any(t => d.Title != null && d.Title.Contains(t, StringComparison.OrdinalIgnoreCase)))
                    .Distinct().Take(5).ToList();
            }
            else
            {
                var candidates = await _db.DevNews.AsNoTracking()
                    .Where(a => a.CreatedOn > fifteenDays && a.NewsId != id && a.AdminCheck == true)
                    .OrderByDescending(a => a.ModifiedOn)
                    .Take(100)
                    .Select(a => new LatestNewsView { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels })
                    .ToListAsync();

                result = candidates.Where(d => tagTokens.Any(t => d.Title != null && d.Title.Contains(t, StringComparison.OrdinalIgnoreCase)))
                    .Distinct().Take(5).ToList();
            }

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), _cacheOptions);
            return View(result);
        }
    }
}
