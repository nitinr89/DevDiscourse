using System.Net;
using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DevDiscourse.Controllers.Main
{
    public class CareersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        public CareersController(ApplicationDbContext db, IWebHostEnvironment _environment)
        {
            this.db = db;
            this._environment = _environment;
        }

        // GET: Careers
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index(int? page = 1)
        {
            return View(db.Careers.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        // GET: Careers/Details/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Career? career = db.Careers.Find(id);
            if (career == null)
            {
                return NotFound();
            }
            return View(career);
        }

        // GET: Careers/Create
        public ActionResult Create()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.edition = "Global Edition";
            }
            else
            {
                ViewBag.edition = cookie ?? "Global Edition";
            }
            return View();
        }

        // POST: Careers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Name,Email,PhoneNumber,Address,Nationality,Qualification,CurrentEmployment,AreaOfExpertise,UploadCV,JobId,JobTitle")] Career career, IFormFile? UploadCV)
        {
            if (ModelState.IsValid)
            {
                if (UploadCV != null && UploadCV.Length > 0)
                {
                    var fileName = RandomName();
                    var extension = Path.GetExtension(UploadCV.FileName);
                    var filePath = "/images/Attachments/";

                    if (extension == ".docx" || extension == ".doc")
                    {
                        filePath += "docx/";
                    }
                    else if (extension == ".pdf")
                    {
                        filePath += "pdf/";
                    }
                    else if (extension == ".txt")
                    {
                        filePath += "txt/";
                    }
                    else
                    {
                        filePath += "other/";
                    }

                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Attachments", filePath);
                    Directory.CreateDirectory(directoryPath);

                    var path = Path.Combine(directoryPath, fileName + extension);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await UploadCV.CopyToAsync(stream);
                    }
                    career.UploadCV = path;
                    career.FileExtension = extension;
                }

                db.Careers.Add(career);
                db.SaveChanges();
                var applyFor = "";
                if (!string.IsNullOrEmpty(career.JobTitle))
                {
                    applyFor = "<p>Apply For: " + career.JobTitle + "</p>";
                }
                // Send Details to Info@devdiscourse.com
                EmailController email = new EmailController();
                string parentEmail = "info@devdiscourse.com";
                string emailData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
                                    "<p>User: " + career.Name + "</p>" +
                                    "<p>Qualification: " + career.Qualification + "</p>" +
                                    "<p>Email: " + career.Email + "</p>" +
                                    "<p>Nationality: " + career.Nationality + "</p>" +
                                    "<p>Area of Expertise: " + career.AreaOfExpertise + "</p>" + applyFor +
                                    "</div>");
                email.SendMail(parentEmail, emailData, "Career Email!");
                TempData["msg"] = "success";
                return RedirectToAction("Create", "Careers");
            }

            return View(career);
        }

        // GET: Careers/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Career? career = db.Careers.Find(id);
            if (career == null)
            {
                return NotFound();
            }
            return View(career);
        }

        // POST: Careers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Name,Email,PhoneNumber,Address,Nationality,Qualification,CurrentEmployment,AreaOfExpertise,UploadCV,CreatedOn")] Career career, IFormFile? UploadCVUpdate)
        {
            if (ModelState.IsValid)
            {
                if (UploadCVUpdate != null && UploadCVUpdate.Length > 0)
                {
                    var fileName = RandomName();
                    var extension = Path.GetExtension(UploadCVUpdate.FileName);
                    var filePath = "/images/Attachments/";

                    if (extension == ".docx" || extension == ".doc")
                    {
                        filePath += "docx/";
                    }
                    else if (extension == ".pdf")
                    {
                        filePath += "pdf/";
                    }
                    else if (extension == ".txt")
                    {
                        filePath += "txt/";
                    }
                    else
                    {
                        filePath += "other/";
                    }

                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Attachments", filePath);
                    Directory.CreateDirectory(directoryPath);

                    var path = Path.Combine(directoryPath, fileName + extension);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await UploadCVUpdate.CopyToAsync(stream);
                    }
                    career.UploadCV = path;
                    career.FileExtension = extension;
                }

                db.Careers.Update(career);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(career);
        }

        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }
        // GET: Careers/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Career? career = db.Careers.Find(id);
            if (career == null)
            {
                return NotFound();
            }
            return View(career);
        }

        // POST: Careers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Career? career = db.Careers.Find(id);
            if (career == null)
            {
                return NotFound();
            }
            db.Careers.Remove(career);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public PartialViewResult GetJobs()
        {
            var search = db.Jobs.Where(a => a.IsPublished == true).OrderByDescending(a => a.CreatedOn).ToList();
            return PartialView("_getJobs", search);
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
