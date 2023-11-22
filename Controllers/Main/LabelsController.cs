using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using X.PagedList;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.Main
{
    public class LabelsController : Controller
    {
        private readonly ApplicationDbContext db;
        public LabelsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: Labels
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index()
        {
            return View(db.Labels.ToList());
        }
        public ActionResult News(string label, int? page)
        {
            ViewBag.edition = "Global Edition";
            var searchLabel = db.Labels.FirstOrDefault(a => a.Title == label);
            ViewBag.label = label;
            var labelId = searchLabel.Id.ToString();
            var search = from m in db.DevNews
                         where m.AdminCheck == true &&
                         (m.NewsLabels.Contains("," + labelId + ",") || m.NewsLabels.StartsWith("," + labelId) || m.NewsLabels.EndsWith(labelId + ",") || m.NewsLabels.Equals(labelId))
                         orderby m.CreatedOn descending
                         select new PublisherView { ModifiedOn = m.ModifiedOn, Title = m.Title, Id = m.NewsId, ImageUrl = m.ImageUrl, Country = m.Country };
            return View(search.ToPagedList((page ?? 1), 20));

        }

        // GET: Labels/Details/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Labels? labels = db.Labels.Find(id);
            if (labels == null)
            {
                return NotFound();
            }
            return View(labels);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // GET: Labels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Labels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,SrNo,Title")] Labels labels)
        {
            if (ModelState.IsValid)
            {
                labels.Slug = ReturnSlug(labels.Title);
                db.Labels.Add(labels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(labels);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // GET: Labels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Labels? labels = db.Labels.Find(id);
            if (labels == null)
            {
                return NotFound();
            }
            return View(labels);
        }

        // POST: Labels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,SrNo,Title")] Labels labels)
        {
            if (ModelState.IsValid)
            {
                labels.Slug = ReturnSlug(labels.Title);
                db.Labels.Update(labels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(labels);
        }
        [Authorize(Roles = "SuperAdmin")]
        // GET: Labels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Labels? labels = db.Labels.Find(id);
            if (labels == null)
            {
                return NotFound();
            }
            return View(labels);
        }

        // POST: Labels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Labels? labels = db.Labels.Find(id);
            if (labels == null)
            {
                return NotFound();
            }
            db.Labels.Remove(labels);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public string CreateSlug()
        {
            var search = db.Labels.ToList();
            foreach (var item in search)
            {
                item.Slug = ReturnSlug(item.Title);
                db.Labels.Update(item);
            }
            db.SaveChanges();
            return "OK";
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
            //byte[] bytes = System.Text.Encoding.GetEncoding("Windows-1251").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
        public string ChangeLabel(int skip = 0, int take = 0)
        {
            var getNews = db.DevNews.Where(a => a.NewsLabels != null).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(take).ToList();
            foreach (var news in getNews)
            {
                var labelarr = news.NewsLabels.Split(',');
                var label1 = labelarr[0];
                if (label1.Length <= 2)
                {
                    int id = int.Parse(label1);
                    var search = db.Labels.Find(id).Slug;
                    news.NewsLabels = search;
                }
                db.DevNews.Update(news);
            }
            db.SaveChanges();
            return "OK";
        }
        public async Task<JsonResult> GetNewsLabels()
        {
            var labels = await db.Labels.ToListAsync();
            return Json(labels);
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
