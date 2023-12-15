using Devdiscourse.Data;
using Devdiscourse.Models.VideoNewsModels;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class VideoNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public VideoNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            await Task.Yield();
            DateTime todayDate = DateTime.Today.AddDays(-30);
            if (reg == "Global Edition")
            {
                //var resultList = _db.VideoNews/*.Where(a => a.AdminCheck == true)*/
                //    .Select(a => new VideoViewModel
                //    {
                //        Id = a.Id,
                //        Title = a.Title,
                //        FileThumbUrl = a.VideoThumbUrl,
                //        CreatedOn = a.CreatedOn,
                //        Duration = a.Duration
                //    }).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
                var resultList = _db.VideoNews.FirstOrDefault();
                return View(resultList);
            }
            else
            {
                //var resultList = _db.VideoNews/*.Where(a => a.AdminCheck == true && a.VideoNewsRegions.Any(r => r.Edition.Title == reg))*/
                //    .Select(a => new VideoViewModel
                var resultList = _db.VideoNews.FirstOrDefault();
                    //{
                    //    Id = a.Id,
                    //    Title = a.Title,
                    //  //  FileThumbUrl = a.VideoThumbUrl,
                    //    CreatedOn = a.CreatedOn,
                    //    Duration = a.Duration
                    //}).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
                return View(resultList);
            }
        }
    }
}
