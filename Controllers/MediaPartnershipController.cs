using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Devdiscourse.Data;

namespace Devdiscourse.Controllers
{
    public class MediaPartnershipController : Controller
    {
        public ApplicationDbContext _db { get; }

        public MediaPartnershipController(ApplicationDbContext db)
        {
            _db = db;
        }
        public ActionResult Index()
        {
            ViewBag.result = "";
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

        public ActionResult Partners()
        {
            ViewBag.result = "";
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

        [HttpPost]
        //[HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult MediaPartners(EmailFormModel emailData)
        {
            string emailhtml = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<meta charset=\"utf-8\" />" +
                "<title>Email from Devdiscourse</title>" +
                "</head>" +
                "<body style=\"padding:0;margin:0;font-family: Arial, Helvetica, sans-serif; font-size: 14px;color:#333;\">" +
                "<div style=\"max-width:800px;margin:0 auto;\">" +
                "<img src=\"https://www.devdiscourse.com/AdminFiles/Logo/Mail_Banner.jpg\" style=\"width:100%;max-width:800px;\"/>" +
                "<div style=\"padding:20px;\">" +
                "<p><strong>Name:</strong>&nbsp;<span>{0}</span></p>" +
                "<p><strong>Email:</strong>&nbsp;<span>{1}</span></p>" +
                "<p><strong>Message:</strong>&nbsp;<pre style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;color:#333;\">{2}</pre></p>" +
                "</div>" +
                "<table style=\"width:100%;\">" +
                "<tbody>" +
                "<tr style=\"background-color:#e1e1e1;font-size:12px;\">" +
                "<td style=\"padding:30px 0 30px 30px;width:85%;vertical-align:bottom;\">" +
                "<div>For customized alerts, <a href=\"https://www.devdiscourse.com/Account/Register\" target=\"_blank\" title=\"This external link will open in a new window\">Register</a> with us.</div>" +
                "<div>If you have any questions or concerns, <a href=\"https://www.devdiscourse.com/AboutUs#meet\" target=\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
                "<br><div style=\"color:#696969;\"> © Copyright 2018 <a href=\"http://www.visionri.com/\" style=\"color:#222;text-decoration:unset;\"> VisionRI</a></div>" +
                "</td>" +
                "<td style=\"padding:0 30px 30px 0;width:15%;vertical-align:bottom;\">" +
                "<div style=\"width:100%;text-align:center;\"> Follow us:</div>" +
                "<br><div style=\"width:100%;text-align:center;\">" +
                "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"https://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
                "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"https://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
                "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"https://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
                "</div></td></tr></tbody></table></div></body></html>";
            var recieverEmail = "mediapartner@devdiscourse.com";
            if (ModelState.IsValid)
            {
                ViewBag.result = "Success";
                var message = new MailMessage();
                message.To.Add(new MailAddress(recieverEmail)); // replace with valid value 
                message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
                message.Subject = "Media Partnership";
                message.Body = string.Format(emailhtml, emailData.FromName, emailData.FromEmail, emailData.Message);
                message.IsBodyHtml = true;
                message.Priority = MailPriority.Normal;
                SmtpClient client = new SmtpClient("in.mailjet.com", 587)
                {
                    Credentials = new NetworkCredential("f81887b0a57aa0603312a98443d32f40", "84d180273651fd6cfe3b95b5b4d653b7"),
                    EnableSsl = true
                };
                client.Send(message);
                ModelState.Clear();
                return View();
            }
            ViewBag.result = "";
            return View(emailData);
        }

        public ActionResult MediaPartners()
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

        public ActionResult KnowledgePartners(string type)
        {
            ViewBag.type = type;
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
    }
}
