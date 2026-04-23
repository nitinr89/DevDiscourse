using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
        };

        public SectorNewsViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sector, string reg = "Global Edition")
        {
            try
            {
                string cacheKey = $"vc:sectorNews:{sector}:{reg}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached != null)
                {
                    var cachedResult = JsonSerializer.Deserialize<List<NewsViewModel>>(cached);
                    if (cachedResult != null) return View(cachedResult);
                }

                var sectorText = sector.ToString();
                DateTime thirtyDays = DateTime.UtcNow.AddDays(-30);

                var resultList = await _db.RegionNewsRankings
                    .AsNoTracking()
                    .Where(dn =>
                        dn.Region.Title == reg &&
                        dn.DevNews.AdminCheck &&
                        !dn.DevNews.IsSponsored &&
                        dn.DevNews.Sector == sectorText &&
                        dn.DevNews.CreatedOn > thirtyDays)
                    .OrderByDescending(dn => dn.DevNews.CreatedOn)
                    .ThenByDescending(dn => dn.Ranking)
                    .Take(20)
                    .Select(dn => new NewsViewModel
                    {
                        NewsId = dn.DevNews.NewsId,
                        Title = dn.DevNews.Title,
                        ImageUrl = dn.DevNews.ImageUrl,
                        CreatedOn = dn.DevNews.ModifiedOn,
                        Subtitle = dn.DevNews.SubTitle,
                        SubType = dn.DevNews.SubType,
                        Country = dn.DevNews.Country,
                        Sector = dn.DevNews.Sector,
                        Label = dn.DevNews.NewsLabels,
                        Ranking = dn.Ranking
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
