using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class EconomyNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public EconomyNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            ViewBag.Sector = "1";
            DateTime twoMonth = DateTime.UtcNow.AddDays(-2);

            var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "1" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
            }).AsNoTracking().Take(30).ToList();
            return View( result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
            //}
        }
    }
}
