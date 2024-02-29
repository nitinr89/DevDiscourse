using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.Others
{
    public class CoronaVirusController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CoronaVirusController(ApplicationDbContext db)
        {
            _db = db;
        }
        public ActionResult Index()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        //public PartialViewResult GetCoronaNews()
        //{
        //    ViewBag.reg = "Global Edition";
        //    var search = _db.DevNews.Where(a => a.AdminCheck == true && a.Category.Contains("35")).Select(a => new LatestNewsView { Id = a.Id, NewId = a.NewsId, Title = a.Title, Country = a.Country, CreatedOn = a.PublishedOn, ImageUrl = a.ImageUrl, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).OrderByDescending(a => a.CreatedOn).Take(9).ToList();
        //    return PartialView("_getcoronaNews", search);
        //}
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
