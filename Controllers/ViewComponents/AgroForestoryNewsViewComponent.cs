using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            //  await Task.Yield();
            try
            {
                ViewBag.Sector = "10";
                DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
                //      var result = await _db.RegionNewsRankings
                //   .Where(a => a.DevNews.CreatedOn > twoMonth
                //       && a.DevNews.AdminCheck
                //       && a.DevNews.Sector == "10"
                //       && a.Region.Title == reg
                //       && !a.DevNews.IsSponsored)
                //   .OrderByDescending(a => a.DevNews.CreatedOn)
                //   .GroupBy(a => a.DevNews.Title)
                //   .SelectMany(group => group.OrderByDescending(a => a.Ranking).Take(10))
                //   .Take(6)
                //.Select(a => new NewsViewModel
                //   {
                //       Title = a.DevNews.Title,
                //       NewsId = a.DevNews.NewsId,
                //       ImageUrl = a.DevNews.ImageUrl,
                //       Subtitle = a.DevNews.SubTitle,
                //       Country = a.DevNews.Country,
                //       CreatedOn = a.DevNews.ModifiedOn,
                //       Sector = a.DevNews.Type,
                //       SubType = a.DevNews.SubType,
                //       Label = a.DevNews.NewsLabels,
                //       Ranking = a.Ranking
                //   })
                //   .AsNoTracking().Take(20).ToListAsync();
                //      return View(result);

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

            //var result = (from ranking in _db.RegionNewsRankings
            //              where ranking.DevNews.CreatedOn > twoMonth
            //                    && ranking.DevNews.AdminCheck
            //                    && ranking.DevNews.Sector == "10"
            //                    && ranking.Region.Title == reg
            //                    && !ranking.DevNews.IsSponsored
            //              orderby ranking.DevNews.CreatedOn descending
            //              group ranking by ranking.DevNews.Title into newsGroup
            //              from topNews in newsGroup.OrderByDescending(a => a.Ranking).Take(10)
            //              orderby topNews.DevNews.CreatedOn descending
            //              select new NewsViewModel
            //              {
            //                  Title = topNews.DevNews.Title,
            //                  NewsId = topNews.DevNews.NewsId,
            //                  ImageUrl = topNews.DevNews.ImageUrl,
            //                  Subtitle = topNews.DevNews.SubTitle,
            //                  Country = topNews.DevNews.Country,
            //                  CreatedOn = topNews.DevNews.ModifiedOn,
            //                  Sector = topNews.DevNews.Type,
            //                  SubType = topNews.DevNews.SubType,
            //                  Label = topNews.DevNews.NewsLabels,
            //                  Ranking = topNews.Ranking
            //              })
            //     .Take(6)
            //     .AsNoTracking()
            //     .Take(20)
            //     .ToListAsync();

            //return View(result);
        }
    }
}
