using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class TagNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TagNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(long id, string tag, string? sector)
        {
            var tagList = tag.Split(',').Reverse().Skip(3).Take(3).ToList().ToString();
            DateTime threemonths = DateTime.Today.AddDays(-15);
            if (!string.IsNullOrEmpty(sector))
            {
                var result = (from a in _db.DevNews
                              where a.CreatedOn > threemonths && a.NewsId != id && a.AdminCheck == true
                              && a.Sector == sector
                              orderby a.ModifiedOn descending
                              select new LatestNewsView { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels })
                              .AsEnumerable().Where(d => tagList.Any(tag => d.Title.Contains(tag)))
                              .Distinct().Take(5).ToList();
                return View(result);
            }
            else
            {
                var result = (from a in _db.DevNews
                              where a.CreatedOn > threemonths && a.NewsId != id && a.AdminCheck == true
                              orderby a.ModifiedOn descending
                              select new LatestNewsView { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels })
                              .AsEnumerable().Where(d => tagList.Any(tag => d.Title.Contains(tag)))
                              .Distinct().Take(5).ToList();
                return View(result);
            }

        }
    }
}
