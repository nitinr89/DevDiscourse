using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.WorldCup
{
    public class CricketWorldCupController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public CricketWorldCupController(ApplicationDbContext _db, IWebHostEnvironment _environment)
        {
            this._db = _db;
            this._environment = _environment;
        }
        // GET: CricketWorldCup
        public ActionResult Index()
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            return View();
        }
        public ActionResult News()
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            return View();
        }
        public PartialViewResult GetNews(string filter, int skip = 0)
        {
            if (string.IsNullOrEmpty(filter))
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true && (a.Category.Contains(",23,") || a.Category.StartsWith("23,") || a.Category.EndsWith(",23") || a.Category.Equals("23"))).Select(a => new CricketWorldCupNews { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).Skip(skip).Take(10).ToList();
                return PartialView("_getNews", search);
            }
            else
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true && (a.Category.Contains(",23,") || a.Category.StartsWith("23,") || a.Category.EndsWith(",23") || a.Category.Equals("23"))).Select(a => new CricketWorldCupNews { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).Skip(skip).Take(10).ToList();
                return PartialView("_getNews", search);
            }
        }
        public PartialViewResult GetLatestNews()
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && (a.Category.Contains(",23,") || a.Category.StartsWith("23,") || a.Category.EndsWith(",23") || a.Category.Equals("23"))).Select(a => new CricketWorldCupNews { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).ToList().Take(5);
            return PartialView("_getLatestNews", search);
        }
        public PartialViewResult GetFeaturedNews()
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.EditorPick == true && (a.Category.Contains(",23,") || a.Category.StartsWith("23,") || a.Category.EndsWith(",23") || a.Category.Equals("23"))).Select(a => new CricketWorldCupNews { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).ToList().Take(5);
            return PartialView("_getLatestNews", search);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult UpdateStanding()
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            return View();
        }
        public JsonResult PostStanding(string match)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "images/worldcup/data/standing.json");
            System.IO.File.WriteAllText(filePath, match);
            //System.IO.File.WriteAllText(Server.MapPath("~/images/worldcup/data/standing.json"), match.ToString());
            return Json(match);
        }
        public JsonResult AddToFeaturedNews(Guid id, bool isFeatured)
        {
            var news = _db.DevNews.Find(id);
            if (news != null)
            {
                news.EditorPick = isFeatured;
                _db.Entry(news).State = EntityState.Modified;
                _db.Entry(news).Property(x => x.ViewCount).IsModified = false;
                _db.Entry(news).Property(x => x.NewsId).IsModified = false;
                _db.SaveChanges();
                return Json("Success");
            }
            return Json("Error");
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult UpdateMostRuns()
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            return View();
        }
        public JsonResult PostMostRuns(string runs)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "images/worldcup/data/mostruns.json");
            System.IO.File.WriteAllText(filePath, runs);
            //System.IO.File.WriteAllText(Server.MapPath("~/images/worldcup/data/mostruns.json"), runs.ToString());
            return Json(runs);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult UpdateMostWickets()
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            return View();
        }
        public JsonResult PostMostWickets(string wickets)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "images/worldcup/data/mostwickets.json");
            System.IO.File.WriteAllText(filePath, wickets);
            //System.IO.File.WriteAllText(Server.MapPath("~/images/worldcup/data/mostwickets.json"), wickets.ToString());
            return Json(wickets);
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