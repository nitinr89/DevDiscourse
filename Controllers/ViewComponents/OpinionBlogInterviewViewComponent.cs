using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class OpinionBlogInterviewViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        public OpinionBlogInterviewViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            string cacheKey = $"vc:opinblog:{reg}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var cachedResult = JsonSerializer.Deserialize<List<NewsViewModel>>(cached);
                if (cachedResult != null) return View(cachedResult);
            }

            DateTime oneMonth = DateTime.Today.AddDays(-30);

            var query = _db.DevNews
                .AsNoTracking()
                .Where(a => a.CreatedOn > oneMonth && a.AdminCheck && a.Type == "Blog");

            if (!string.Equals(reg, "Global Edition", StringComparison.OrdinalIgnoreCase))
            {
                var regionNewsIds = _db.RegionNewsRankings
                    .AsNoTracking()
                    .Where(r => r.Region.Title == reg)
                    .Select(r => r.NewsId);

                query = query.Where(a => regionNewsIds.Contains(a.Id));
            }

            var search = await query
                .OrderByDescending(a => a.PublishedOn)
                .Take(4)
                .Select(a => new NewsViewModel
                {
                    Title = a.Title,
                    CreatedOn = a.PublishedOn,
                    ImageUrl = a.ImageUrl,
                    SubType = a.Themes,
                    Subtitle = a.SubTitle,
                    Country = a.Author,
                    NewsId = a.NewsId,
                    Label = a.NewsLabels
                })
                .ToListAsync();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(search), _cacheOptions);
            return View(search);
        }
    }
}
