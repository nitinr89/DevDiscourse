using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class VideoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public VideoViewComponent(ApplicationDbContext db)
        {
            _db= db;
        }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            await Task.Yield();
            var videoNews = _db.VideoNews.Find(id);
            return View(videoNews);
        }
    }
}
