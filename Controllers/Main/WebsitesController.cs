using System.Net;
using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace DevDiscourse.Controllers.Main
{
    [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
    public class WebsitesController : Controller
    {
        private readonly ApplicationDbContext db;

        public WebsitesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: Websites
        public ActionResult Index(Guid? sec, int? page = 1, string str = "")
        {
            ViewBag.region = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").ToList();
            var websites = db.Websites.Where(a => a.Type != "PressRelease" && a.Regions != null);
            if (sec != null)
            {
                websites = websites.Where(a => a.RegionId == sec);
            }
            if (!String.IsNullOrEmpty(str))
            {
                websites = websites.Where(a => a.Country.ToUpper().Contains(str.ToUpper()));
            }
            ViewBag.secId = sec;
            return View(websites.OrderBy(a => a.Country).ToPagedList((page ?? 1), 50));
        }

        // GET: Websites/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Website? website = db.Websites.Find(id);
            if (website == null)
            {
                return NotFound();
            }
            return View(website);
        }

        // GET: Websites/Create
        public ActionResult Create()
        {
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title");
            return View();
        }

        // POST: Websites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,RegionId,Country,SiteUrl,PressReleaseUrl")] Website website)
        {
            if (ModelState.IsValid)
            {
                website.Id = Guid.NewGuid();
                website.Type = "Website";
                website.PressRelease = "";
                website.CreatedOn = DateTime.UtcNow;
                db.Websites.Add(website);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title", website.RegionId);
            return View(website);
        }

        // GET: Websites/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Website? website = db.Websites.Find(id);
            if (website == null)
            {
                return NotFound();
            }
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title", website.RegionId);
            return View(website);
        }

        // POST: Websites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,RegionId,Country,SiteUrl,PressReleaseUrl,CreatedOn")] Website website)
        {
            if (ModelState.IsValid)
            {
                website.Type = "Website";
                website.PressRelease = "";
                db.Websites.Update(website);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title", website.RegionId);
            return View(website);
        }

        // GET: Websites/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Website? website = db.Websites.Find(id);
            if (website == null)
            {
                return NotFound();
            }
            return View(website);
        }

        // POST: Websites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Website? website = db.Websites.Find(id);
            if (website == null)
            {
                return NotFound();
            }
            db.Websites.Remove(website);
            db.SaveChanges();
            if (website.Type == "Website")
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("PressRelease");
            }
        }

        // GET: PressRelease
        public ActionResult PressRelease(int? page = 1, string str = "")
        {
            ViewBag.str = str;
            var websites = db.Websites.Where(a => a.Type == "PressRelease");
            if (!String.IsNullOrEmpty(str))
            {
                websites = websites.Where(a => a.PressRelease.ToUpper().Contains(str.ToUpper()));
            }
            return View(websites.OrderBy(a => a.CreatedOn).ToPagedList((page ?? 1), 50));
        }
        // GET: Websites/Create
        public ActionResult CreateNew()
        {
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title == "Global Edition"), "Id", "Title");
            return View();
        }

        // POST: Websites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew([Bind("Id,RegionId,PressRelease,PressReleaseUrl,SiteUrl")] Website website)
        {
            if (ModelState.IsValid)
            {
                website.Id = Guid.NewGuid();
                website.Type = "PressRelease";
                website.Country = "";
                website.CreatedOn = DateTime.UtcNow;
                db.Websites.Add(website);
                db.SaveChanges();
                return RedirectToAction("PressRelease");
            }
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title == "Global Edition"), "Id", "Title", website.RegionId);
            return View(website);
        }
        // GET: Websites/Update/5
        public ActionResult Update(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Website? website = db.Websites.Find(id);
            if (website == null)
            {
                return NotFound();
            }
            return View(website);
        }

        // POST: Websites/Update/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind("Id,RegionId,PressRelease,PressReleaseUrl,SiteUrl,CreatedOn")] Website website)
        {
            if (ModelState.IsValid)
            {
                website.Type = "PressRelease";
                website.Country = "";
                db.Websites.Update(website);
                db.SaveChanges();
                return RedirectToAction("PressRelease");
            }
            return View(website);
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
