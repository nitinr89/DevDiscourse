using Devdiscourse.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers
{
    public class TrainingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TrainingController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Training
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        public string GetCVEmail()
        {
            var cvemail = _db.Users.Select(s => s.Email).Distinct().ToArray();
            return string.Join(",", cvemail).ToLower();
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