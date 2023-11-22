using System.Net;
using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DevDiscourse.Controllers.Main
{
    public class AdvertisementsController : Controller
    {
        private readonly ApplicationDbContext db;
        public AdvertisementsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: Advertisements
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index(int? page = 1)
        {
            ViewBag.Srno = (page - 1) * 20;
            return View(db.Advertisements.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
        }
        public ActionResult AdvertiseWithUs()
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
        // GET: Advertisements/Details/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Advertisement? advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        // GET: Advertisements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,Advertisor,Email,Description,Phone")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                EmailController emailObj = new EmailController();
                advertisement.Id = Guid.NewGuid();
                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                // Reply to advertisers
                string replyData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
                                    "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
                                    "<tr><td colspan=\"2\">Thank you for showing interest in advertising with us.</td></tr>" +
                                    "<tr><td colspan=\"2\">We will process your request and our team will get back to you.</td></tr>" +
                                    "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Thank you, <br> The Devdiscourse team</td></tr>" +
                                    "<tr style=\"background-color:#e1e1e1;font-size:12px;\"><td style=\"padding: 30px 0 30px 30px; width: 85%; vertical-align: bottom;\">" +
                                    "<div>If you have any questions or concerns, <a href=\"http://www.devdiscourse.com/AboutUs#meet\" target =\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
                                    "<br><div style=\"color: #696969;\"> © Copyright 2017 <a href=\"http://www.visionri.com/\" style =\"color:#222;text-decoration:unset;\"> VisionRI</a></div></td>" +
                                    "<td style=\"padding: 0 30px 30px 0; width: 15%; vertical-align: bottom;\" ><div style= \"width: 100%; text-align: center;\" > Follow us:</div>" +
                                    "<br><div style =\"width: 100%; text-align: center;\">" +
                                    "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
                                    "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
                                    "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
                                    "</div></td></tr>" +
                                    "</table>" +
                                    "</div>");

                emailObj.SendMail(advertisement.Email, replyData, "Thank you for your request - Devdiscourse!");
                // Send Details to Info@devdiscourse.com
                EmailController email = new EmailController();
                string parentEmail = "info@devdiscourse.com";
                string emailData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
                                    "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
                                    "<tr><td colspan=\"2\">Advertisor: " + advertisement.Advertisor + "</td></tr>" +
                                    "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Email: " + advertisement.Email + "</td></tr>" +
                                    "<tr><td colspan=\"2\"><table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; width: 100%; color: #555555; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\"><tbody>" +
                                    advertisement.Description +
                                    "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Phone: " + advertisement.Phone + "</td></tr>" +
                                    "</tbody></table>" +
                                    "</table>" +
                                    "</div>");
                email.SendMail(parentEmail, emailData, "Advertisement Details!");
                return RedirectToAction("Create", "Advertisements");
            }
            return View(advertisement);
        }

        // GET: Advertisements/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Advertisement? advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,Advertisor,Email,Description,Phone,CreatedOn")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                db.Advertisements.Update(advertisement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Advertisement? advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Advertisement? advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            db.Advertisements.Remove(advertisement);
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
