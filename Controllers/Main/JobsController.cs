using System.Net;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using System;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;

namespace DevDiscourse.Controllers.Main
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public JobsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        // GET: Jobs
        public ActionResult Index(int? page = 1)
        {
            return View(db.Jobs.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        // GET: Jobs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Job? job = db.Jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // GET: Jobs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,Title,OrganizationName,Location,AboutPosition,EmailId,OpeningDate,ClosingDate,Keywords,MinCTC,MaxCTC,Vacancy,MinExperience,MaxExperience")] Job job)
        {
            if (ModelState.IsValid)
            {
                job.JobCTCType = "Yearly";
                job.CTCCurrency = "INR";
                job.ViewCount = 0;
                job.PostedByUser = userManager.GetUserId(User);
                job.IsPublished = true;
                db.Jobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(job);
        }

        // GET: Jobs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Job? job = db.Jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,Title,OrganizationName,Location,AboutPosition,EmailId,IsPublished,PostedByUser,OpeningDate,ClosingDate,Keywords,MinCTC,MaxCTC,JobCTCType,CTCCurrency,Vacancy,MinExperience,MaxExperience,ViewCount,CreatedOn")] Job job)
        {
            if (ModelState.IsValid)
            {
                job.ModifiedOn = DateTime.UtcNow;
                db.Jobs.Update(job);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(job);
        }

        // GET: Jobs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Job? job = db.Jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Job? job = db.Jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            db.Jobs.Remove(job);
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
