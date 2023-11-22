using System.Net;
using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.Main
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class RegionsController : Controller
    {
        private readonly ApplicationDbContext db;
        public RegionsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: Regions
        public ActionResult Index()
        {
            return View(db.Regions.ToList().OrderBy(a => a.SrNo));
        }

        // GET: Regions/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Region? region = db.Regions.Find(id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // GET: Regions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,Title")] Region region)
        {
            if (ModelState.IsValid)
            {
                region.Id = Guid.NewGuid();
                region.SrNo = 0;
                db.Regions.Add(region);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(region);
        }

        // GET: Regions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Region? region = db.Regions.Find(id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,SrNo,Title")] Region region)
        {
            if (ModelState.IsValid)
            {
                db.Regions.Update(region);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(region);
        }

        // GET: Regions/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Region? region = db.Regions.Find(id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Region? region = db.Regions.Find(id);
            if (region == null)
            {
                return NotFound();
            }
            db.Regions.Remove(region);
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
