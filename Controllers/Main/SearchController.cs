using Devdiscourse.Data;
using Devdiscourse.Services;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.Main
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly INewsLookupService _newsLookupService;

        public SearchController(ApplicationDbContext db, INewsLookupService newsLookupService)
        {
            _db = db;
            _newsLookupService = newsLookupService;
        }
        public async Task<ActionResult> Index(string sector = "all", string tag = "")
        {
            string? cookie = Request.Cookies["Edition"];
            if (string.IsNullOrWhiteSpace(sector)) sector = "all";
            ViewBag.sectorSlug = sector;
            ViewBag.region = cookie ?? "Global Edition";
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
                var sectorSearch = await _newsLookupService.GetSectorBySlugAsync(sector);
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
            var videoNews = await _db.VideoNews
                .AsNoTracking()
                .Include(video => video.VideoNewsTags!)
                .ThenInclude(tag => tag.Tagstb)
                .FirstOrDefaultAsync(video => video.Id == id);

            if (videoNews != null)
            {
                ViewBag.videoNews = videoNews;
                ViewBag.Tags = string.Join(", ", videoNews.VideoNewsTags?.Select(s => s.Tagstb?.TagTitle).Where(title => !string.IsNullOrWhiteSpace(title)).ToArray() ?? Array.Empty<string>());

                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                bool isCrawler = userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase);
                if (!isCrawler)
                {
                    await _db.VideoNews
                        .Where(video => video.Id == videoNews.Id)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(video => video.ViewCount, video => video.ViewCount + 1));
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
    }
}
