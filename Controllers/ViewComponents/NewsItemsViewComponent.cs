using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class NewsItemsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public NewsItemsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sector, string region, string country, string tag, string cat, string label, int? page)
        {
            // DateTime oneMonth = DateTime.Today.AddDays(-10);
            // DateTime oneMonth = DateTime.Today.AddDays(-10);
            cat = cat ?? "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            //var resultList = _db.RegionNewsRankings.AsNoTracking()
            //    //.Where(a => a.DevNews.AdminCheck == true && a.DevNews.NewsLabels == label && a.Region.Title == region && a.DevNews.IsSponsored == false)
            //    //.OrderByDescending(a => a.DevNews.CreatedOn)
            //    .Select(a => new NewsViewModel
            //{
            //    Title = a.DevNews.Title,
            //    NewsId = a.DevNews.NewsId,
            //    ImageUrl = a.DevNews.ImageUrl,
            //    Subtitle = a.DevNews.SubTitle,
            //    Country = a.DevNews.Country,
            //    CreatedOn = a.DevNews.ModifiedOn,
            //    Sector = a.DevNews.Type,
            //    SubType = a.DevNews.SubType,
            //    Label = a.DevNews.NewsLabels,
            //    Ranking = a.Ranking
            //}).ToPagedList(pageNumber, pageSize);

            ////return View(resultList.OrderByDescending(s => s.CreatedOn.Date).ThenByDescending(o => o.Ranking));
            //return View(resultList.OrderByDescending(s => s.CreatedOn.Date).ThenByDescending(o => o.Ranking));
            var resultList = (from dn in _db.DevNews.AsNoTracking()
                              where dn.AdminCheck == true && dn.NewsLabels == label && dn.IsSponsored == false
                              select new NewsViewModel
                              {
                                  NewsId = dn.NewsId,
                                  Title = dn.Title,
                                  ImageUrl = dn.ImageUrl,
                                  CreatedOn = dn.ModifiedOn,
                                  Subtitle = dn.SubTitle,
                                  Sector = dn.Sector,
                                  SubType = dn.SubType,
                                  Country = dn.Country,
                                  Label = dn.NewsLabels,
                                  Ranking = 0
                              }).OrderByDescending(a => a.CreatedOn)
           .Take(65)
           .ToList();
            return View(resultList.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());



        }
    }
}
