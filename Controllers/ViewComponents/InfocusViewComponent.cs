using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
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
            try
            {
                var lastThreeHour = DateTime.UtcNow.AddDays(-12);
                var infocus = (from a in _db.RegionNewsRankings
                               where a.DevNews.AdminCheck == true
                                  && a.Region.Title == reg
                                  && a.DevNews.CreatedOn > lastThreeHour
                                  && !a.DevNews.Title.Contains("News Summary")
                                  && !a.DevNews.Title.Contains("Highlights")
                                  && a.DevNews.NewsLabels != "Newsalert"
                                  && !new[] { "14", "18", "19", "9" }.Contains(a.DevNews.Sector)
                               orderby a.DevNews.ModifiedOn descending

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
                               }).Take(65).ToList();
                return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);

            }

        }
    }
}

