using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class EnergyNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public EnergyNewsViewComponent(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            try
            {

                ViewBag.Sector = "7";
                DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
                var region = (from c in _db.Countries
                              join r in _db.Regions on c.RegionId equals r.Id
                              where c.Title == reg
                              select new
                              {
                                  r.Title
                              }).FirstOrDefault();
                string regionTitle = "Global Edition";
                var result = region != null && region.Title != null ? regionTitle = region.Title : regionTitle = reg;
                var resultList = _db.RegionNewsRankings
    .Where(dn => dn.DevNews.CreatedOn > twoMonth && dn.DevNews.AdminCheck == true &&
                 dn.DevNews.Sector == "7" && dn.Region.Title == result && dn.DevNews.IsSponsored == false)
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
                return Content("Error: " + ex.Message);
            }
        }
    }
}
