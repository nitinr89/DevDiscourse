using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorMostPopularViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        public SectorMostPopularViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sector, string reg = "Global Edition")
        {
            try
            {
                string cacheKey = $"vc:sectorMostPopular:{sector}:{reg}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached != null)
                {
                    var cachedResult = JsonSerializer.Deserialize<List<SearchView>>(cached);
                    if (cachedResult != null) return View(cachedResult);
                }

                var sectorText = sector.ToString();
                DateTime recentWindow = DateTime.Today.AddDays(-4);

                var query = _db.DevNews
                    .AsNoTracking()
                    .Where(a =>
                        a.AdminCheck &&
                        !a.IsSponsored &&
                        a.CreatedOn > recentWindow &&
                        a.Sector == sectorText);

                if (!string.Equals(reg, "Global Edition", StringComparison.OrdinalIgnoreCase))
                {
                    var regionNewsIds = _db.RegionNewsRankings
                        .AsNoTracking()
                        .Where(r => r.Region.Title == reg)
                        .Select(r => r.NewsId);

                    query = query.Where(a => regionNewsIds.Contains(a.Id));
                }

                var resultList = await query
                    .OrderByDescending(a => a.ViewCount)
                    .ThenByDescending(a => a.CreatedOn)
                    .Take(3)
                    .Select(a => new SearchView
                    {
                        Id = a.Id,
                        NewsId = a.NewsId,
                        Title = a.Title,
                        ImageUrl = a.ImageUrl,
                        Country = a.Country,
                        CreatedOn = a.CreatedOn,
                        Type = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
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
