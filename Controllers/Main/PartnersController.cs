using System.Net;
using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DevDiscourse.Controllers.Main
{
    public class PartnersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> userManager;
        public PartnersController(ApplicationDbContext db, IWebHostEnvironment _environment, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this._environment = _environment;
            this.userManager = userManager;
        }

        // GET: Partners
        public ActionResult Index(int? page = 1)
        {
            var search = db.Partners.OrderByDescending(a => a.CreatedOn);
            return View(search.ToPagedList((page ?? 1), 10));
        }

        // GET: Partners/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Partners? partners = db.Partners.Find(id);
            if (partners == null)
            {
                return NotFound();
            }
            return View(partners);
        }

        // GET: Partners/Create
        public ActionResult Create()
        {
            ViewBag.Type = Enum.GetValues(typeof(PartnerType));
            return View();
        }

        // POST: Partners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Name,Description,ImageUrl,Country,Type,SubType")] Partners partners, IFormFile? ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = RandomName();
                    var fileExtension = Path.GetExtension(ImageUrl.FileName);

                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "Partners", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrl.CopyToAsync(fileStream);
                    }

                    partners.ImageUrl = "/AdminFiles/Partners/" + fileName + fileExtension;
                }
                partners.Creator = userManager.GetUserId(User);
                partners.IsActive = true;
                db.Partners.Add(partners);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Type = Enum.GetValues(typeof(PartnerType));
            return View(partners);
        }

        // GET: Partners/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Partners? partners = db.Partners.Find(id);
            if (partners == null)
            {
                return NotFound();
            }
            ViewBag.Type = Enum.GetValues(typeof(PartnerType));
            return View(partners);
        }

        // POST: Partners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Name,Description,ImageUrl,Country,Type,SubType,CreatedOn,IsActive,Creator")] Partners partners, IFormFile? ImageUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName();
                    var fileExtension = Path.GetExtension(ImageUrlUpdate.FileName);

                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "Partners", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrlUpdate.CopyToAsync(fileStream);
                    }

                    partners.ImageUrl = "/AdminFiles/Partners/" + fileName + fileExtension;
                }
                db.Partners.Update(partners);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Type = Enum.GetValues(typeof(PartnerType));
            return View(partners);
        }

        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }
        // GET: Partners/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Partners? partners = db.Partners.Find(id);
            if (partners == null)
            {
                return NotFound();
            }
            return View(partners);
        }

        // POST: Partners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Partners? partners = db.Partners.Find(id);
            if (partners == null)
            {
                return NotFound();
            }
            db.Partners.Remove(partners);
            db.SaveChanges();
            return RedirectToAction("Index");
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
