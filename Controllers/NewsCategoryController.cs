using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers
{
    public class NewsCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NewsCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string slug)
        {

            // HttpCookie cookie = Request.Cookies["Edition"];
            string? cookie = Request.Cookies["Edition"];
            
            
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            var search = _db.Categories.SingleOrDefault(a => a.Slug == slug);
            //ViewBag.data = search;
            return View(search);
            //return View();
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
