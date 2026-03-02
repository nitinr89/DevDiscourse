using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevDiscourse.Controllers.Main
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext db;
        public CountriesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: Countries
        public ActionResult Index(Guid? id, string searchText = "")
        {
            ViewBag.regionList = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA" && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo).ToList();
            var countries = db.Countries.Where(c => c.Regions != null);
            if (id != null)
            {
                countries = countries.Where(a => a.RegionId == id);
            }
            if (!String.IsNullOrEmpty(searchText))
            {
                countries = countries.Where(a => a.Title.ToUpper().Contains(searchText.ToUpper()));
            }
            ViewBag.rid = id;
            ViewBag.text = searchText;
            ViewBag.region = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").ToList();
            return View(countries.OrderBy(a => a.Title).ToList());
        }

        // GET: Countries/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            //Country? country = db.Countries.Find(id);
            var country = (from c in db.Countries
                           join r in db.Regions on c.RegionId equals r.Id
                           select new Country
                           {
                               Id = c.Id,
                               RegionId = c.RegionId,
                               Title = c.Title,
                               Regions = new Region
                               {
                                   Id = r.Id,
                                   Title = r.Title
                               }
                           }).FirstOrDefault(f => f.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // GET: Countries/Create
        public ActionResult Create()
        {
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title");
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,RegionId,Title")] Country country)
        {
            if (ModelState.IsValid)
            {
                country.Id = Guid.NewGuid();
                db.Countries.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title", country.RegionId);
            return View(country);
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Country? country = db.Countries.Find(id);
            if (country == null)
            {
                return NotFound();
            }
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title", country.RegionId);
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,RegionId,Title")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Update(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION"), "Id", "Title", country.RegionId);
            return View(country);
        }

        // GET: Countries/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Country? country = db.Countries.Find(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Country? country = db.Countries.Find(id);
            if (country == null)
            {
                return NotFound();
            }
            db.Countries.Remove(country);
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
