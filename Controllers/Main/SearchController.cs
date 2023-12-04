using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.Main
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SearchController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<ActionResult> Index(string sector = "all", string tag = "")
        {
            ViewBag.sectorSlug = sector;
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            if (!string.IsNullOrEmpty(tag))
            {
                ViewBag.sectorName = "";
                ViewBag.sector = "0";
                ViewBag.tag = tag;
                return View("TagSearch");
            }
            else if (sector == "all" || sector == "All")
            {
                return RedirectToActionPermanent("NewsAnalysis", "AgencyWire");
                //ViewBag.sectorName = "";
                //ViewBag.sector = "0";
                //return View("AllNews");
            }
            else
            {
                var sectorSearch = await _db.DevSectors.FirstOrDefaultAsync(a => a.Slug == sector);
                if (sectorSearch != null)
                {
                    ViewBag.sectorName = sectorSearch.Title;
                    ViewBag.sector = sectorSearch.Id;
                }
            }
            return View();
        }

        public ActionResult SDGStories()
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

        public async Task<ActionResult> Videos(long? id)
        {
            var videoNews = await _db.VideoNews.FindAsync(id);
            if (videoNews != null)
            {
                ViewBag.videoNews = videoNews;
                ViewBag.Tags = string.Join(", ", videoNews.VideoNewsTags.Select(s => s.Tagstb.TagTitle).ToArray());
                //if (!Request.Browser.Crawler)
                {
                    videoNews.ViewCount = videoNews.ViewCount + 1;
                    _db.Entry(videoNews).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
            }
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
