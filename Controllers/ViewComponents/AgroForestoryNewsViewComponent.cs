using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class AgroForestoryNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public AgroForestoryNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            try
            {
                ViewBag.Sector = "10";
                DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
                var region = (from c in _db.Countries
                              join r in _db.Regions on c.RegionId equals r.Id
                              where c.Title == reg
                              select new
                              {
                                  r.Title
                              }).FirstOrDefault();
                string regionTitle = "Global Edition";

                if (region != null && region.Title != null) regionTitle = region.Title;

                var resultList = _db.RegionNewsRankings
                    .Where(dn => dn.DevNews.CreatedOn > twoMonth &&
                       dn.DevNews.AdminCheck == true &&
                       dn.DevNews.Sector == "10" &&
                       dn.Region.Title == regionTitle &&
                       !dn.DevNews.IsSponsored
                  )
    .OrderByDescending(dn => dn.DevNews.CreatedOn)
     .Take(65)
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
         Ranking = 0
     })
     .ToList();

                var groupedResult = resultList
                    .GroupBy(s => s.Title)
                    .Select(group => group.OrderByDescending(a => a.Ranking).FirstOrDefault())
                    .OrderByDescending(o => o.Ranking)
                    .Take(30)
                    .ToList();

                return View(groupedResult);

            }
            catch (Exception ex)
            {
                return Content($"Internel Server Error : {ex.Message}");
            }
        }
    }
}
