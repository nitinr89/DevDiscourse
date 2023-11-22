using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class GetEventNewsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public GetEventNewsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(int skip = 0, int take = 0)
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && (a.Category.StartsWith("21,") || a.Category.EndsWith(",21") || a.Category.Contains(",21,") || a.Category == "21")).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(take).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels });
            return View(search);
        }
    }
}
