using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.Others
{
    public class InternshipController : Controller
    {
        private readonly ApplicationDbContext db;

        public InternshipController(ApplicationDbContext db)
        {
            this.db = db;
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
        public async Task<ActionResult> AppliedBy()
        {
            var search = from m in db.MediaInternships
                         join a in db.Users on m.UserId equals a.Id
                         select new InternshipApplicant
                         {
                             Id = a.Id,
                             Email = a.Email,
                             CVUrl = m.CVUrl,
                             FirstName = a.FirstName,
                             LastName = a.LastName,
                             PhoneNumber = a.PhoneNumber
                         };
            return View(await search.ToListAsync());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
