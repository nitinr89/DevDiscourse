using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
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

            int pageSize = 25;
            int pageNumber = (page ?? 1);
            ViewBag.page = pageNumber;
            DateTime oneMonth = pageNumber == 1 ? DateTime.Today.AddDays(-120) : DateTime.Today.AddDays(-150);
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
                return View( search);
            }
            else
            {
                //var search = _db.RegionNewsRankings
                //    //.Where(a => a.DevNews.AdminCheck == true && a.Region.Title == region && a.DevNews.CreatedOn > oneMonth)
                //    .Select(a => new NewsAnalysisViewModel
                //    {
                //        NewsId = a.DevNews.NewsId,
                //        Title = a.DevNews.Title,
                //        ImageUrl = a.DevNews.ImageUrl,
                //        Country = a.DevNews.Country,
                //        CreatedOn = a.DevNews.ModifiedOn,
                //        Type = a.DevNews.Type,
                //        SubType = a.DevNews.SubType,
                //        Label = a.DevNews.NewsLabels,
                //        Ranking = a.Ranking
                //    })
                //    //.OrderByDescending(o => o.CreatedOn)
                //    .ToPagedList(pageNumber, pageSize);
                //return View(search/*.OrderByDescending(o => o.CreatedOn.Date).ThenByDescending(s => s.Ranking).AsEnumerable()*/);

                var resultList = _db.RegionNewsRankings
                    //.Where(a => a.DevNews.AdminCheck == true && a.Region.Title == region && a.DevNews.CreatedOn > oneMonth)
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
}).ToPagedList(pageNumber, pageSize);

                return View(resultList.OrderByDescending(o => o.CreatedOn.Date).ThenByDescending(s => s.Ranking).AsEnumerable());

            }

        }
    }
}
