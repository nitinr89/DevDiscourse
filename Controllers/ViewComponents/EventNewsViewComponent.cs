using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class EventNewsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public EventNewsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id, int skip = 0, int take = 0)
        {
            var search = _db.DevNews
                .Where(a => a.AdminCheck == true)// && (a.Category.StartsWith(id + ",") || a.Category.EndsWith("," + id) || a.Category.Contains("," + id + ",") || a.Category == id))
                .OrderByDescending(a => a.CreatedOn).Skip(skip).Take(take)
                .Select(a => new LatestNewsView
                {
                    Id = a.Id,
                    Title = a.Title, 
                    CreatedOn = a.CreatedOn,
                    ImageUrl = a.ImageUrl, 
                    Sector = a.SubTitle,
                    Country = a.Country,
                    NewId = a.NewsId,
                    Type = a.Type,
                    SubType = a.SubType,
                    Label = a.NewsLabels
                });
            return View(search);
        }
    }
}
