using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class ArticleVideosViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ArticleVideosViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            await Task.Yield();
            DateTime todayDate = DateTime.Today.AddDays(-45);
            if (reg == "Global Edition")
            {
                var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > todayDate && a.IsVideo == true)
                    .Select(a => new SearchView
                    {
                        NewsId = a.NewsId,
                        Title = a.Title,
                        ImageUrl = a.ImageUrl,
                        CreatedOn = a.CreatedOn,
                        Label = a.NewsLabels
                    }).OrderByDescending(m => m.CreatedOn).Take(4).AsNoTracking();
                return View(resultList);
            }
            else
            {
                var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > todayDate && a.Region.Contains(reg) && a.IsVideo == true)
                    .Select(a => new SearchView
                    {
                        NewsId = a.NewsId,
                        Title = a.Title,
                        ImageUrl = a.ImageUrl,
                        CreatedOn = a.CreatedOn,
                        Label = a.NewsLabels
                    }).OrderByDescending(m => m.CreatedOn).Take(4).AsNoTracking();
                return View(resultList);
            }
        }
    }
}
