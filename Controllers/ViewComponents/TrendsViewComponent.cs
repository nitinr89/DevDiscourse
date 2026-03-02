using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class TrendsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        public TrendsViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? id, string? reg, string filter = "")
        {
            try
            {
                ViewBag.filter = filter;
                DateTime today = DateTime.Today;
                DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
                reg ??= "Global Edition";

                // Cache key based on region (id excluded since trends are region-wide)
                string cacheKey = $"vc:trends:{reg}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached != null)
                {
                    var cachedResult = JsonSerializer.Deserialize<List<LatestNewsView>>(cached);
                    if (cachedResult != null) return View(cachedResult);
                }

                var query = _db.DevNews
                    .AsNoTracking()
                    .Where(dn =>
                        dn.CreatedOn > weekStart &&
                        dn.AdminCheck &&
                        dn.Id != id &&
                        dn.Sector == "14");

                if (!string.Equals(reg, "Global Edition", StringComparison.OrdinalIgnoreCase))
                {
                    var regionNewsIds = _db.RegionNewsRankings
                        .AsNoTracking()
                        .Where(r => r.Region.Title == reg)
                        .Select(r => r.NewsId);

                    query = query.Where(dn => regionNewsIds.Contains(dn.Id));
                }

                var resultList = await query
                    .OrderByDescending(dn => dn.ViewCount)
                    .Take(4)
                    .Select(dn => new LatestNewsView
                    {
                        Id = dn.Id,
                        NewId = dn.NewsId,
                        Title = dn.Title,
                        ImageUrl = dn.ImageUrl,
                        CreatedOn = dn.ModifiedOn,
                        Type = dn.Type,
                        SubType = dn.SubType,
                        Country = dn.Country,
                        Label = dn.NewsLabels,
                        Ranking = 0
                    })
                    .ToListAsync();

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(resultList), _cacheOptions);
                return View(resultList);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
