using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var lastThreeHour = DateTime.UtcNow.AddHours(-12);
                var infocus = await (from a in _db.RegionNewsRankings
                                     where a.DevNews.AdminCheck == true
                                        && a.Region.Title == reg
                                        && a.DevNews.CreatedOn > lastThreeHour
                                        && a.DevNews.Title != null
                                        && !a.DevNews.Title.Contains("News Summary")
                                        && !a.DevNews.Title.Contains("Highlights")
                                        && a.DevNews.NewsLabels != "Newsalert"
                                        && !new[] { "14", "18", "19", "9" }.Contains(a.DevNews.Sector)
                                     orderby a.DevNews.CreatedOn descending
                                     select new LatestNewsView
                                     {
                                         Id = a.DevNews.Id,
                                         NewId = a.DevNews.NewsId,
                                         Title = a.DevNews.Title,
                                         ImageUrl = a.DevNews.ImageUrl,
                                         CreatedOn = a.DevNews.ModifiedOn,
                                         Type = a.DevNews.Type,
                                         SubType = a.DevNews.SubType,
                                         Country = a.DevNews.Country,
                                         Label = a.DevNews.NewsLabels,
                                         Ranking = a.Ranking
                                     }).Take(65).ToListAsync();
                infocus = infocus.GroupBy(s => s.Title).Select(a => a.First()).OrderByDescending(o => o.Ranking).Take(6).ToList();
                return View(infocus);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}

