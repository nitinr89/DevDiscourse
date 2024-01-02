using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
//using reCAPTCHA.MVC;
//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using System.Threading.Tasks;

namespace DevDiscourse.Controllers
{
    public class AboutController : Controller
    {
        private ApplicationDbContext _db;
        public AboutController(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public ActionResult Index(string reg = "Global Edition")
        {
            ViewBag.edition = reg;
            //HttpCookie cookie = Request.Cookies["Edition"];
            //if (reg != "")
            //{
            //	ViewBag.edition = reg;
            //}
            //else if (cookie == null)
            //{
            //	ViewBag.edition = "Global Edition";
            //}
            //else
            //{
            //	ViewBag.edition = cookie.Value ?? "Global Edition";
            //}
            ViewBag.edition = "Global Edition";
            return View();
        }

        /*[HttpPost]
        [CaptchaValidator]
        public async Task<ActionResult> Index(EmailFormModel model, bool captchaValid)
        {
            if (ModelState.IsValid)
            {
                EmailController emailObj = new EmailController();
                var body = "<h3>Contact Us Email:</h3><p><b>Name :</b> {0}</p><p><b>Email id :</b> {1}</p><p><b>Message :</b> {2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("info@devdiscourse.com")); // replace with valid value 
                message.From = new MailAddress("info@devdiscourse.com"); // replace with valid value
                message.Subject = "Contact Us message";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "info@devdiscourse.com", // replace with valid value
                        Password = "#devdiscourse@2018#" // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }
                string replyData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
                        "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
                        "<tr><td colspan=\"2\">Thank you for contacting us.</td></tr>" +
                        "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Team Devdiscourse</td></tr>" +
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

                await emailObj.SendEmailAsync(model.FromEmail, replyData, "Thank you - Devdiscourse!");
                ViewBag.msg = "Success";
                ModelState.Clear();
                return View();
            }
            ViewBag.msg = "Error";
            return View(model);
        }*/

        //public async Task<JsonResult> ContactWithUS(string name, string email, string messagetext)
        //{
        //    EmailController emailObj = new EmailController();
        //    var body = "<h3>Contact Us Email:</h3><p><b>Name :</b> {0}</p><p><b>Email id :</b> {1}</p><p><b>Message :</b> {2}</p>";
        //    var message = new MailMessage();
        //    message.To.Add(new MailAddress("info@devdiscourse.com")); // replace with valid value 
        //    message.From = new MailAddress("info@devdiscourse.com"); // replace with valid value
        //    message.Subject = "Contact Us message";
        //    message.Body = string.Format(body, name, email, messagetext);
        //    message.IsBodyHtml = true;
        //    using (var smtp = new SmtpClient())
        //    {
        //        var credential = new NetworkCredential
        //        {
        //            UserName = "info@devdiscourse.com", // replace with valid value
        //            Password = "#devdiscourse@2018#" // replace with valid value
        //        };
        //        smtp.Credentials = credential;
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.Port = 587;
        //        smtp.EnableSsl = true;
        //        await smtp.SendMailAsync(message);
        //    }
        //    string replyData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
        //            "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
        //            "<tr><td colspan=\"2\">Thank you for showing interest with us.</td></tr>" +
        //            "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Thank you, <br> The Devdiscourse team</td></tr>" +
        //            "<tr style=\"background-color:#e1e1e1;font-size:12px;\"><td style=\"padding: 30px 0 30px 30px; width: 85%; vertical-align: bottom;\">" +
        //            "<div>If you have any questions or concerns, <a href=\"http://www.devdiscourse.com/AboutUs#meet\" target =\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
        //            "<br><div style=\"color: #696969;\"> © Copyright 2017 <a href=\"http://www.visionri.com/\" style =\"color:#222;text-decoration:unset;\"> VisionRI</a></div></td>" +
        //            "<td style=\"padding: 0 30px 30px 0; width: 15%; vertical-align: bottom;\" ><div style= \"width: 100%; text-align: center;\" > Follow us:</div>" +
        //            "<br><div style =\"width: 100%; text-align: center;\">" +
        //            "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
        //            "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
        //            "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
        //            "</div></td></tr>" +
        //            "</table>" +
        //            "</div>");

        //    emailObj.SendMail(email, replyData, "Thank you - Devdiscourse!");
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}

        /* public JsonResult SubmitAdvertisement(string name, string email, string details, string phone)
		 {
			 Advertisement obj = new Advertisement
			 {
				 Advertisor = name,
				 Email = email,
				 Description = details,
				 Phone = phone
			 };
			 _db.Advertisements.Add(obj);
			 _db.SaveChanges();

			 EmailController emailObj = new EmailController();
			 // Reply to advertisers
			 string replyData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
								 "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
								 "<tr><td colspan=\"2\">Thankyou for showing interest in advertising with us.</td></tr>" +
								 "<tr><td colspan=\"2\">We will process your request and our team will get back to you.</td></tr>" +
								 "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Thank you, <br> The Devdiscourse team</td></tr>" +
								 "<tr style=\"background-color:#e1e1e1;font-size:12px;\"><td style=\"padding: 30px 0 30px 30px; width: 85%; vertical-align: bottom;\">" +
								 "<div>If you have any questions or concerns, <a href=\"https://www.devdiscourse.com/AboutUs#meet\" target =\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
								 "<br><div style=\"color: #696969;\"> © Copyright 2017 <a href=\"http://www.visionri.com/\" style =\"color:#222;text-decoration:unset;\"> VisionRI</a></div></td>" +
								 "<td style=\"padding: 0 30px 30px 0; width: 15%; vertical-align: bottom;\" ><div style= \"width: 100%; text-align: center;\" > Follow us:</div>" +
								 "<br><div style =\"width: 100%; text-align: center;\">" +
								 "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
								 "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
								 "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
								 "</div></td></tr>" +
								 "</table>" +
								 "</div>");

			 emailObj.SendMail(email, replyData, "Thank you for your request -Team Devdiscourse!");
			 // Send Details to Info@devdiscourse.com
			 EmailController emailobj2 = new EmailController();
			 string parentEmail = "info@devdiscourse.com";
			 string emailData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
								 "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
								 "<tr><td colspan=\"2\">Advertisor: " + name + "</td></tr>" +
								 "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Email: " + email + "</td></tr>" +
								 "<tr><td colspan=\"2\"><table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; width: 100%; color: #555555; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\"><tbody>" +
								 details +
								 "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Phone: " + phone + "</td></tr>" +
								 "</tbody></table>" +
								 "</table>" +
								 "</div>");
			 emailobj2.SendMail(parentEmail, emailData, "Advertisement Details!");
			 return Json("Success!", JsonRequestBehavior.AllowGet);
		 }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                _db.Dispose();
                _db = null;
            }
            base.Dispose(disposing);
        }
    }
}