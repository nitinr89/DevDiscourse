using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using X.PagedList;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class NewsAnalysisItemsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public NewsAnalysisItemsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string region, string type, int? page)
        {
            await Task.Yield();
            try
            {
                int pageSize = 25;
                int pageNumber = (page ?? 1);
                ViewBag.page = pageNumber;
                DateTime oneMonth = pageNumber == 1 ? DateTime.Today.AddDays(-2) : DateTime.Today.AddDays(-15);
                if (type == "Other" || region == "Global Edition")
                {
                    var search = (from a in _db.DevNews
                                  where a.AdminCheck && a.CreatedOn > oneMonth
                                  select new NewsAnalysisViewModel
                                  {
                                      NewsId = a.NewsId,
                                      Title = a.Title,
                                      ImageUrl = a.ImageUrl,
                                      Country = a.Country,
                                      CreatedOn = a.ModifiedOn,
                                      Type = a.Type,
                                      SubType = a.SubType,
                                      Label = a.NewsLabels
                                  }).OrderByDescending(o => o.CreatedOn).AsNoTracking().ToPagedList(pageNumber, pageSize);
                    return View(search);
                }
                else
                {
                    //if (region == "India")
                    //{
                        var reg = (from c in _db.Countries
                                   join r in _db.Regions on c.RegionId equals r.Id
                                   where c.Title == region
                                   select new
                                   {
                                       r.Title
                                   }).FirstOrDefault();
                        string regionTitle = "Global Edition";

                        //if (region != null && reg.Title != null)
                        //{
                        //    regionTitle = reg.Title;
                        //    region = regionTitle;
                        //}
                        var result = reg != null && reg.Title != null ? regionTitle = reg.Title : regionTitle = region;
                 //   }

                    var search = _db.RegionNewsRankings.Where(a => a.DevNews.AdminCheck == true &&
                     a.Region.Title == result &&
                    a.DevNews.CreatedOn > oneMonth)
                        .Select(a => new NewsAnalysisViewModel
                        {
                            NewsId = a.DevNews.NewsId,
                            Title = a.DevNews.Title,
                            ImageUrl = a.DevNews.ImageUrl,
                            Country = a.DevNews.Country,
                            CreatedOn = a.DevNews.ModifiedOn,
                            Type = a.DevNews.Type,
                            SubType = a.DevNews.SubType,
                            Label = a.DevNews.NewsLabels,
                            Ranking = a.Ranking
                        }).AsNoTracking().OrderByDescending(o => o.CreatedOn).ToPagedList(pageNumber, pageSize);
                    return View(search.OrderByDescending(o => o.CreatedOn.Date).ThenByDescending(s => s.Ranking).AsEnumerable());

                    //var resultList = (from rnr in _db.RegionNewsRankings
                    //                  join dn in _db.DevNews on rnr.NewsId equals dn.Id
                    //                  join r in _db.Regions on rnr.RegionId equals r.Id
                    //                  let a = new { RegionNews = rnr, DevNews = dn, Region = r }
                    //                  where a.DevNews.AdminCheck == true
                    //                       // && a.Region.Title == "Global Edition" //need to change region
                    //                        && a.DevNews.ModifiedOn > oneMonth
                    //                  orderby a.DevNews.ModifiedOn descending, a.RegionNews.Ranking descending
                    //                  select new NewsAnalysisViewModel
                    //                  {
                    //                      NewsId = a.DevNews.NewsId,
                    //                      Title = a.DevNews.Title,
                    //                      ImageUrl = a.DevNews.ImageUrl,
                    //                      Country = a.DevNews.Country,
                    //                      CreatedOn = a.DevNews.ModifiedOn,
                    //                      Type = a.DevNews.Type,
                    //                      SubType = a.DevNews.SubType,
                    //                      Label = a.DevNews.NewsLabels,
                    //                      Ranking = a.RegionNews.Ranking
                    //                  })
                    //    .Take(50)
                    //    .ToList();
                    //return View(resultList);
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
