using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DevDiscourse.Controllers
{
    public class CampaignController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CampaignController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Campaign
        public ActionResult MotherNotPatient()
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
        public ActionResult Volunteer()
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
        public ActionResult Pledges(int? page)
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
            return View(_db.CampaignPetitions.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
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