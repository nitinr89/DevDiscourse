using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using X.PagedList;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers
{
    public class DiscourseTagController : Controller
    {
        private readonly ApplicationDbContext db;
        public DiscourseTagController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: DiscourseTag
        public ActionResult Index(string search, int? sector, string currentFilter, int? page)
        {
            ViewBag.search = search;
            ViewBag.sector = sector;
            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }

            ViewBag.CurrentFilter = search;
            search = search ?? "";
            var TagSearch = db.DiscourseTags.Where(a => a.Title.Contains(search) && a.ParentId == 0).AsEnumerable();
            if (sector != null)
            {
                TagSearch = TagSearch.Where(a => a.SectorId == sector);
            }
            return View(TagSearch.OrderBy(o => o.CreatedOn).ToPagedList(page ?? 1, 20));
        }
        public async Task<ActionResult> Create(long? id)
        {
            ViewBag.id = id ?? 0;
            ViewBag.sector = "";
            if (id != null)
            {
                var discourseTag = await db.DiscourseTags.FindAsync(id);
                if (discourseTag == null)
                {
                    return NotFound();
                }
                ViewBag.discourseTag = discourseTag;
                ViewBag.sector = discourseTag.SectorId;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,SectorId,ParentId")] DiscourseTag discourseTag)
        {
            if (ModelState.IsValid)
            {
                discourseTag.CreatedOn = DateTime.UtcNow;
                db.DiscourseTags.Add(discourseTag);
                await db.SaveChangesAsync();
                if (discourseTag.ParentId != 0)
                {
                    return RedirectToAction("Details", new { id = discourseTag.ParentId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(discourseTag);
        }
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var discourseTag = await db.DiscourseTags.FindAsync(id);
            if (discourseTag == null)
            {
                return NotFound();
            }
            return View(discourseTag);
        }
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var discourseTag = await db.DiscourseTags.FindAsync(id);
            if (discourseTag == null)
            {
                return NotFound();
            }
            return View(discourseTag);
        }

        // POST: DevSectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,Title,SectorId,ParentId,CreatedOn")] DiscourseTag discourseTag)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discourseTag).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discourseTag);
        }
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var discourseTag = await db.DiscourseTags.FindAsync(id);
            if (discourseTag == null)
            {
                return NotFound();
            }
            return View(discourseTag);
        }

        // POST: DevSectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            var discourseTag = await db.DiscourseTags.FindAsync(id);
            if (discourseTag == null)
            {
                return NotFound();
            }
            db.DiscourseTags.Remove(discourseTag);
            await db.SaveChangesAsync();
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