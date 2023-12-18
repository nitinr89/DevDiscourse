using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Devdiscourse.Utility;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class InfocusViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public InfocusViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            await Task.Yield();

            var lastThreeHour = DateTime.UtcNow.AddDays(-123);
            //var infocus = _db.RegionNewsRankings
            //  .Where(a => a.DevNews.AdminCheck == true
            //              && a.Region.Title == reg
            //              && a.DevNews.CreatedOn > lastThreeHour
            //              && !a.DevNews.Title.Contains("News Summary")
            //              && !a.DevNews.Title.Contains("Highlights")
            //              && a.DevNews.NewsLabels != "Newsalert"
            //              && !new[] { "14", "18", "19", "9" }.Contains(a.DevNews.Sector))
            //  .Select(s => new LatestNewsView
            //  {
            //      Id = dn.Id,
            //      NewId = dn.NewsId,
            //      Title = dn.Title,
            //      ImageUrl = dn.ImageUrl,
            //      CreatedOn = dn.ModifiedOn,
            //      Type = dn.Type,
            //      SubType = dn.SubType,
            //      Country = dn.Country,
            //      Label = dn.NewsLabels,
            //      Ranking = s.Ranking
            //  }).OrderByDescending(a => a.CreatedOn)
            //      .Take(65)
            //      .ToList();


            var infocus = (from dn in _db.DevNews
                           select new LatestNewsView
                           {
                               Id = dn.Id,
                               NewId = dn.NewsId,
                               Title = dn.Title,
                               ImageUrl = dn.ImageUrl,
                               CreatedOn = dn.ModifiedOn,
                               Type = dn.Type,
                               SubType = dn.SubType,
                               Country = dn.Country,
                               Label = dn.NewsLabels,
                               Ranking = 0
                           }).OrderByDescending(a => a.CreatedOn)
                  .Take(65)
                  .ToList();
            return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());

            //            var resultList = _db.DevNews
            //.Where(dn => dn.AdminCheck == true &&
            //         dn.CreatedOn > lastThreeHour && !dn.Title.Contains("News Summary")
            //                          && !dn.Title.Contains("Highlights")
            //                          && dn.NewsLabels != "Newsalert"
            //                          && !new[] { "14", "18", "19", "9" }.Contains(dn.Sector) )
            //.OrderByDescending(dn => dn.CreatedOn)
            //.Take(65)
            //.Select(dn => new NewsViewModel
            //{
            //    NewsId = dn.NewsId,
            //    Title = dn.Title,
            //    ImageUrl = dn.ImageUrl,
            //    CreatedOn = dn.ModifiedOn,
            //    Subtitle = dn.SubTitle,
            //    SubType = dn.SubType,
            //    Country = dn.Country,
            //    Sector = dn.Sector,
            //    Label = dn.NewsLabels,
            //    Ranking = 0
            //})
            //.ToList();

            //            var groupedResult = resultList
            //                .GroupBy(s => s.Title)
            //                .Select(group => group.OrderByDescending(a => a.Ranking).FirstOrDefault())
            //                .OrderByDescending(o => o.Ranking)
            //                .Take(30)
            //                .ToList();

            //            return View(groupedResult);
        }
    }
}

