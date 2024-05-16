using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SocialNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public SocialNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            try
            {
                ViewBag.Sector = "9";
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
     .Where(dn => dn.DevNews.CreatedOn > twoMonth && dn.DevNews.AdminCheck == true &&
                  dn.DevNews.Sector == "9" && dn.Region.Title == regionTitle && dn.DevNews.IsSponsored == false)
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

                var sponsoredNews = (from sn in _db.SponsoredNews
                                     join n in _db.DevNews on sn.NewsId equals n.Id
                                     where sn.IsActive == true && n.AdminCheck == true && sn.Sector == 9 && sn.EndTime > DateTime.UtcNow
                                     select new NewsViewModelIndex
                                     {
                                         Index = sn.Position,
                                         News = new NewsViewModel
                                         {
                                             NewsId = n.NewsId,
                                             Title = n.Title,
                                             ImageUrl = n.ImageUrl,
                                             CreatedOn = n.ModifiedOn,
                                             Subtitle = n.SubTitle,
                                             SubType = n.SubType,
                                             Country = n.Country,
                                             Sector = n.Sector,
                                             Label = n.NewsLabels,
                                             Ranking = 0
                                         }

                                     }).ToList();

                foreach (var item in sponsoredNews)
                {
                    groupedResult.Insert(item.Index, item.News);
                }
                return View(groupedResult);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
