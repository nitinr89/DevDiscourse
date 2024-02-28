using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class AmpInterviewViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public AmpInterviewViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();
            DateTime todayDate = DateTime.Today.AddDays(-10);
            var result = (from a in _db.DevNews
                         // where a.AdminCheck == true && a.SubType == "Interview" && a.CreatedOn > todayDate
                          //orderby a.PublishedOn descending
                          select new LatestNewsView
                          {
                              Title = a.Title,
                              CreatedOn = a.CreatedOn,
                              ImageUrl = a.ImageUrl,
                              NewId = a.NewsId,
                              Label = a.NewsLabels,
                              Country = a.Country
                          }).AsNoTracking().Take(10);
            return View(result);
        }
    }
}
