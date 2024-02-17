using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SectorNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sector, string reg = "Global Edition")
        {
            await Task.Yield();
            try
            {
                //var resultList = _db.RegionNewsRankings
                //    ./*Where(a => a.DevNews.AdminCheck == true && a.DevNews.Sector == sector && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).*/
                //    Select(a => new NewsViewModel
                //    {
                //        Title = a.DevNews.Title,
                //        NewsId = a.DevNews.NewsId,
                //        ImageUrl = a.DevNews.ImageUrl,
                //        Subtitle = a.DevNews.SubTitle,
                //        Country = a.DevNews.Country,
                //        CreatedOn = a.DevNews.ModifiedOn,
                //        Sector = a.DevNews.Type,
                //        SubType = a.DevNews.SubType,
                //        Label = a.DevNews.NewsLabels,
                //        Ranking = a.Ranking
                //    }).AsNoTracking().Take(20).ToList();
                //return View(resultList.OrderByDescending(s => s.CreatedOn.Date).ThenByDescending(o => o.Ranking));
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
    .Where(dn => dn.DevNews.AdminCheck == true && dn.DevNews.Sector == Convert.ToString(sector) && dn.Region.Title==regionTitle && dn.DevNews.IsSponsored == false)
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
                    .Take(20)
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
