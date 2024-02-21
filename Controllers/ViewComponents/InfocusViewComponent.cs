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
                var infocus = (from dn in _db.DevNews
                               where dn.AdminCheck == true && dn.CreatedOn > lastThreeHour && dn.NewsLabels != "Newsalert"
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
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);

            }
        }
    }
}

