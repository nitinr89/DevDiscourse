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
            var lastThreeHour = DateTime.UtcNow.AddHours(-12);
            var infocus = _db.RegionNewsRankings
              .Where(a => a.DevNews.AdminCheck == true
                          && a.Region.Title == reg
                          && a.DevNews.CreatedOn > lastThreeHour
                          && !a.DevNews.Title.Contains("News Summary")
                          && !a.DevNews.Title.Contains("Highlights")
                          && a.DevNews.NewsLabels != "Newsalert"
                          && !new[] { "14", "18", "19", "9" }.Contains(a.DevNews.Sector))
              .Select(s => new LatestNewsView
              {
                  Id = s.DevNews.Id,
                  NewId = s.DevNews.NewsId,
                  Title = s.DevNews.Title,
                  ImageUrl = s.DevNews.ImageUrl,
                  CreatedOn = s.DevNews.ModifiedOn,
                  Type = s.DevNews.Type,
                  SubType = s.DevNews.SubType,
                  Country = s.DevNews.Country,
                  Label = s.DevNews.NewsLabels,
                  Ranking = s.Ranking
              })
              .OrderByDescending(a => a.CreatedOn)
              .Take(65)
              .ToList();
            return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());
        }
    }
}
