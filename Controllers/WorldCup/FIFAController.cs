using Devdiscourse.Models;
using Devdiscourse.Models.ViewModel;
using X.PagedList;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.WorldCup
{
    public class FIFAController : Controller
    {
        private ApplicationDbContext _db;
        public FIFAController(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        // GET: FIFA
        public ActionResult Index()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        public ActionResult Details(long? id)
        {
            ViewBag.edition = "Global Edition";
            if (id == null || id == 0)
            {
               return BadRequest();
            }
            var search = _db.DevNews.FirstOrDefault(a => a.NewsId == id);
            if (search == null)
            {
               return BadRequest();
            }
            if (search != null)
            {
                string description = Regex.Replace(search.Description, " style=[^>]*", "");
                description = Regex.Replace(description, "<img", "<amp-img layout='responsive'");
                description = Regex.Replace(description, "<iframe", "<amp-iframe layout='responsive' sandbox=\"allow-scripts allow-same-origin\"");
                description = Regex.Replace(description, "width=\"100%\"", "width=\"300\"");
                description = Regex.Replace(description, "height=\"100%\"", "height=\"240\"");
                ViewBag.AmpDescriptoion = description;
                search.ViewCount = search.ViewCount + 1;
                _db.Entry(search).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return View(search);
        }
        public async Task<JsonResult> AmpRelatedNews(long id)
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.NewsId != id && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new {
                title = a.Title,
                image = a.ImageUrl,
                url = "/FIFA/Details/" + a.NewsId + "?amp",
                ModifiedOn = a.ModifiedOn
            }).OrderByDescending(b => b.ModifiedOn).Take(5);
                return Json(new { items = await search.ToListAsync() });

        }
        public ActionResult Search(string fl = "")
        {
            ViewBag.edition = "Global Edition";
            ViewBag.filter = fl;
            return View();
        }
        public ActionResult MatchSchedule()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        public ActionResult GroupStanding()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        // Fifa News
        public PartialViewResult GetFifaLatestNews()
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsStandout == false && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a=> new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).ToList().Take(5);
            return PartialView("_getFifaLatestNews", search);
        }
        public PartialViewResult GetAmpFifaNews(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsStandout == false && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).Take(10).ToList();
                return PartialView("_getAmpFifaNews", search);
            }
            else
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true & a.IsStandout == true && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).Take(10).ToList();
                return PartialView("_getAmpFifaNews", search);
            }
        }
        public PartialViewResult GetFifaFeaturedNews()
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsStandout == true && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).ToList().Take(5);
            return PartialView("_getFifaFeaturedNews", search);
        }
        public PartialViewResult GetFifaNews(string filter, int skip = 0)
        {
            if(string.IsNullOrEmpty(filter))
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsStandout == false && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).Skip(skip).Take(10).ToList();
                return PartialView("_getFifaNews", search);
            }
            else
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true & a.IsStandout == true && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).Skip(skip).Take(10).ToList();
                return PartialView("_getFifaNews", search);
            }
        }
        public JsonResult GetFifaAmpNews(string filter,string __amp_source_origin, int? moreItemsPageIndex)
        {
            if (!string.IsNullOrEmpty(__amp_source_origin))
            {
                //HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
                HttpContext.Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
            }
            if (string.IsNullOrEmpty(filter))
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsStandout == false && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn);
                int pageSize = 10;
                int pageNumber = (moreItemsPageIndex ?? 1);
                var resultData = search.ToPagedList(pageNumber, pageSize);
                return Json(new { items = resultData, hasMorePages = resultData.Any() });
            }
            else
            {
                var search = _db.DevNews.Where(a => a.AdminCheck == true & a.IsStandout == true && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn);
                int pageSize = 10;
                int pageNumber = (moreItemsPageIndex ?? 1);
                var resultData = search.ToPagedList(pageNumber, pageSize);
                return Json(new { items = resultData, hasMorePages = resultData.Any() });
            }
        }
        public PartialViewResult GetRelatedPost(long id)
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.NewsId != id && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).ToList().Take(6);
            return PartialView("_getRelatedPost", search);
        }
        public PartialViewResult GetLatestArticle()
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14"))).Select(a => new FifaNewsView { Id = a.NewsId, Title = a.Title, ModifiedOn = a.ModifiedOn, ImageUrl = a.ImageUrl }).OrderByDescending(a => a.ModifiedOn).ToList().Take(3);
            return PartialView("_getLatestArticle", search);
        }
        public async Task<string> FIFANewsUpdate()
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsStandout == true && (a.Category.Contains(",14,") || a.Category.StartsWith(",14") || a.Category.EndsWith("14,") || a.Category.Equals("14")));
            if(search.Any())
            {
                foreach(var item in search)
                {
                    item.IsStandout = false;
                    _db.Entry(item).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
            }
            return "OK";
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