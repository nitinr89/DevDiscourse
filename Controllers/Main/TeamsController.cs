using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.Main
{
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        public TeamsController(ApplicationDbContext db, IWebHostEnvironment _environment)
        {
            this.db = db;
            this._environment = _environment;
        }

        // GET: Teams
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index()
        {
            return View(db.Teams.OrderBy(a => a.SrNo).ToList());
        }

        // GET: Teams/Details/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Team? team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Create()
        {
            ViewBag.Type = Enum.GetValues(typeof(TeamType));
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,SrNo,TeamMember,Designation,ProfilePic,Type,Active")] Team team, IFormFile? ProfilePic)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePic != null && ProfilePic.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(ProfilePic.FileName);

                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "TeamProfile", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePic.CopyToAsync(fileStream);
                    }

                    // Here, you can handle the file path as needed, such as saving it to a database.
                    // For example, assuming "team" is an instance of your Team model:
                    team.ProfilePic = "/AdminFiles/TeamProfile/" + fileName + fileExtension;
                }
                if (team.Type == 0)
                {
                    team.Type = TeamType.Devdiscourse;
                }
                team.Id = Guid.NewGuid();
                team.AboutMe = "";
                db.Teams.Add(team);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Type = Enum.GetValues(typeof(TeamType));
            return View(team);
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }

        // GET: Teams/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Team? team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewBag.Type = Enum.GetValues(typeof(TeamType));
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,SrNo,TeamMember,Designation,ProfilePic,Type,Active,CreatedOn")] Team team, IFormFile? ProfilePicUpdate)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePicUpdate != null && ProfilePicUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(ProfilePicUpdate.FileName);

                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "TeamProfile", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicUpdate.CopyToAsync(fileStream);
                    }

                    // Here, you can handle the file path as needed, such as saving it to a database.
                    // For example, assuming "team" is an instance of your Team model:
                    team.ProfilePic = "/AdminFiles/TeamProfile/" + fileName + fileExtension;
                }
                team.AboutMe = "";
                db.Teams.Update(team);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Type = Enum.GetValues(typeof(TeamType));
            return View(team);
        }

        // GET: Teams/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Team? team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Team? team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            db.Teams.Remove(team);
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
