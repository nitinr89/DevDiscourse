using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class LatestNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        public LatestNewsViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            string cacheKey = $"vc:latestnews:{reg}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var cachedResult = JsonSerializer.Deserialize<List<NewsViewModel>>(cached);
                if (cachedResult != null) return View(cachedResult);
            }

            DateTime twoDays = DateTime.Today.AddDays(-2);

            var query = _db.DevNews
                .AsNoTracking()
                .Where(dn => dn.CreatedOn > twoDays && dn.AdminCheck && dn.Type != "Blog");

            if (!string.Equals(reg, "Global Edition", StringComparison.OrdinalIgnoreCase))
            {
                var regionNewsIds = _db.RegionNewsRankings
                    .AsNoTracking()
                    .Where(r => r.Region.Title == reg)
                    .Select(r => r.NewsId);

                query = query.Where(dn => regionNewsIds.Contains(dn.Id));
            }

            var result = await query
                .OrderByDescending(a => a.ModifiedOn)
                .Take(5)
                .Select(dn => new NewsViewModel
                {
                    NewsId = dn.NewsId,
                    Title = dn.Title,
                    ImageUrl = dn.ImageUrl,
                    CreatedOn = dn.ModifiedOn,
                    Subtitle = dn.SubTitle,
                    Label = dn.NewsLabels
                })
                .ToListAsync();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), _cacheOptions);
            return View(result);
        }
    }
}
