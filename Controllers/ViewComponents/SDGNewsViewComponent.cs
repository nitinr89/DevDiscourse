
using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SDGNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public SDGNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();
            ViewBag.reg = "Global Edition";
            var search = _db.DevNews
                //Where(a => a.AdminCheck == true && a.Category.Contains("13"))
                .Select(a => new LatestNewsView { Id = a.Id, NewId = a.NewsId, Title = a.Title, Country = a.Country, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).OrderByDescending(a => a.CreatedOn)
                .Take(9).ToList();
            return View(search);

        }
    }
}
