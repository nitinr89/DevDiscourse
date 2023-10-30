using Devdiscourse.Data;
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
        public JsonResult getDevenews()
        {
            var search = from m in _db.DevNews select m;
            return Json(search.Take(10).ToList());
        }
    }
}
