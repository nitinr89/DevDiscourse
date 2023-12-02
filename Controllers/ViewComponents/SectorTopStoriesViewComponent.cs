using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorTopStoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SectorTopStoriesViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sector, string reg = "Global Edition")
        {
            await Task.Yield();
           // DateTime twoMonth = DateTime.UtcNow.AddDays(-3);
            var result = _db.RegionNewsRankings
                //.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == sector && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn)
                .Select(a => new NewsViewModel
                {
                    Title = a.DevNews.Title,
                    NewsId = a.DevNews.NewsId,
                    ImageUrl = a.DevNews.ImageUrl,
                    Subtitle = a.DevNews.SubTitle,
                    Country = a.DevNews.Country,
                    CreatedOn = a.DevNews.ModifiedOn,
                    Sector = a.DevNews.Type,
                    SubType = a.DevNews.SubType,
                    Label = a.DevNews.NewsLabels,
                    Ranking = a.Ranking
                }).AsNoTracking().Take(60).ToList();
            return View(result.OrderByDescending(a => a.Ranking).Take(12));
        }
    }
}
