using Devdiscourse.Data;
using DocumentFormat.OpenXml.Drawing;
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
            string? cookie = Request.Cookies["Edition"];  
            var reg = cookie;
            var regs = (from c in _db.Countries join r in _db.Regions on c.RegionId equals r.Id where c.Title == reg select new { r.Title }).FirstOrDefault();
            string regionTitle = string.Empty;
            var region = string.Empty;
            if (reg == "")
            {
                region = "Global Edition";
            }
            else
            {
                region = regs != null && regs.Title != null ? regionTitle = regs.Title : regionTitle = reg;
            }
            if (string.IsNullOrWhiteSpace(sector)) sector = "all";
            ViewBag.sectorSlug = sector;
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = region ?? "Global Edition";

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

        [Route("news/videos/{id?}")]
        public async Task<ActionResult> Videos(long? id)
        {
            var videoNews = await _db.VideoNews.FindAsync(id);
            if (videoNews != null && videoNews.VideoNewsTags != null)
            {
                ViewBag.videoNews = videoNews;
                  ViewBag.Tags = string.Join(", ", videoNews.VideoNewsTags.Select(s => s.Tagstb?.TagTitle).ToArray());

                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                bool isCrawler = userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase);
                if (!isCrawler)
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
