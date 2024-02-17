using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class VideoTopStoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public VideoTopStoriesViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sectore, string reg = "Global Edition")
        {
            await Task.Yield();
            try {
                DateTime lastTenDays = DateTime.Today.AddDays(-10);
                if (reg == "Global Edition")
                {
                    var resultList = _db.VideoNews
                        .Where(a => a.AdminCheck == true && a.CreatedOn > lastTenDays && a.EditorPick == true)
                        .Select(a => new VideoViewModel
                        {
                            Id = a.Id,
                            Title = a.Title,
                            FileThumbUrl = a.VideoThumbUrl,
                            CreatedOn = a.CreatedOn,
                            Description = a.Description,
                            Duration = a.Duration
                        }).OrderByDescending(m => m.CreatedOn).Take(5).AsNoTracking();
                    return View(resultList);
                }
                else
                {
                    var resultList = _db.VideoNews
                        .Where(a => a.AdminCheck == true && a.CreatedOn > lastTenDays && a.EditorPick == true && a.VideoNewsRegions.Any(r => r.Edition.Title == reg))
                        .Select(a => new VideoViewModel
                        {
                            Id = a.Id,
                            Title = a.Title,
                            FileThumbUrl = a.VideoThumbUrl,
                            CreatedOn = a.CreatedOn,
                            Description = a.Description,
                            Duration = a.Duration
                        }).OrderByDescending(m => m.CreatedOn).Take(5).AsNoTracking();
                    return View(resultList);
                }
            }catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
