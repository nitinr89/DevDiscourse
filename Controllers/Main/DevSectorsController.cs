using System.Text.RegularExpressions;
using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.Main
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DevSectorsController : Controller
    {
        private readonly ApplicationDbContext db;
        public DevSectorsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: DevSectors
        public ActionResult Index()
        {
            return View(db.DevSectors.OrderBy(a => a.Title).ToList());
        }

        // GET: DevSectors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevSector? devSector = db.DevSectors.Find(id);
            if (devSector == null)
            {
                return NotFound();
            }
            return View(devSector);
        }

        // GET: DevSectors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DevSectors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,SrNo,Title")] DevSector devSector)
        {
            if (ModelState.IsValid)
            {
                devSector.Slug = ReturnSlug(devSector.Title);
                db.DevSectors.Add(devSector);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(devSector);
        }

        [Route("DevSector/GetSector/")]
        public IActionResult GetSector()
        {
            var search = db.DevSectors
                //.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.Title)
                .Select(s => new { s.Id, s.Slug, s.SrNo, s.Title });
            return Ok(search);
        }

        // GET: DevSectors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevSector? devSector = db.DevSectors.Find(id);
            if (devSector == null)
            {
                return NotFound();
            }
            return View(devSector);
        }

        // POST: DevSectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,SrNo,Title")] DevSector devSector)
        {
            if (ModelState.IsValid)
            {
                devSector.Slug = ReturnSlug(devSector.Title);
                db.DevSectors.Update(devSector);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(devSector);
        }

        // GET: DevSectors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevSector? devSector = db.DevSectors.Find(id);
            if (devSector == null)
            {
                return NotFound();
            }
            return View(devSector);
        }

        // POST: DevSectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DevSector? devSector = db.DevSectors.Find(id);
            if (devSector == null)
            {
                return NotFound();
            }
            db.DevSectors.Remove(devSector);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<string> updateSlug()
        {
            var data = await db.DevSectors.ToListAsync();
            foreach (var item in data)
            {
                item.Slug = ReturnSlug(item.Title);
                db.DevSectors.Update(item);
            }
            await db.SaveChangesAsync();
            return "Ok";
        }
        public string ReturnSlug(string title)
        {
            string str = RemoveAccent(title).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }
        private string RemoveAccent(string text)
        {
            //byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
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
