using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class NewsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public NewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition", int skip = 0, int take = 0)
        {
            try
            {
                ViewBag.skipCount = skip;
                DateTime tenDays = DateTime.Today.AddHours(-10);
                if (reg == "Global Edition")
                {
                    var result = await _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Sector != null && a.NewsLabels != null).AsNoTracking().OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take).ToListAsync();
                    return View(result);
                }
                else
                {
                    var result = await _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Sector != null && a.NewsLabels != null && a.Region != null && a.Region.Contains(reg)).AsNoTracking().OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take).ToListAsync();
                    return View(result);
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }

        }
    }
}
