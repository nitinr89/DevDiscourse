using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class UserNewsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public UserNewsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string name, int? page)
        {
            try
            {
                ViewBag.name = name;
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                var news = _db.DevNews.Where(a => a.ApplicationUsers.UserName == name && a.OriginalSource == "Devdiscourse News Desk" && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new LatestNewsView { NewId = s.NewsId, Title = s.Title, CreatedOn = s.CreatedOn, ImageUrl = s.ImageUrl, Label = s.NewsLabels }).ToPagedList(pageNumber, pageSize);
                return View(news);
            }catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
