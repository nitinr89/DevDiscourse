using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class LiveBlogsampViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public LiveBlogsampViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            var search = _db.LiveBlogs.Where(a => a.ParentId == id).OrderByDescending(a => a.CreatedOn)
                .Select(s => new LiveblogViewModel
            {
                Title = s.Title,
                Description = s.Description,
                Id = s.Id,
                CreatedOn = s.CreatedOn,
                ImageUrl = s.ImageUrl,
                ImageCopyright = s.ImageCopyright,
                ModifiedOn = s.ModifiedOn,
                DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id).ToList()
            }).ToList();
            return View(search);
        }
    }
}
