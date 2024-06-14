using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext _db;
        public TestController(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult dev()
        {
            List<DevNews> news = _db.DevNews.OrderByDescending(o => o.CreatedOn).Take(1).ToList();
            return Json(news);
        }
    }
}
