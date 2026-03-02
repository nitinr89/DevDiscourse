using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DevDiscourse.Controllers
{
    public class EmailController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //public EmailController(ApplicationDbContext _db)
        //{
        //    this._db = _db;
        //}
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }
        public void SendMail(string RecieverEmail, string body, string Subject)
        {
            //var message = new MailMessage();
            //message.To.Add(new MailAddress(RecieverEmail)); // replace with valid value 
            //message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
            //message.Subject = Subject;
            //message.Body = string.Format(body, "Devdiscourse");
            //message.IsBodyHtml = true;
            //SmtpClient client = new SmtpClient("in.mailjet.com", 587)
            //{
            //    Credentials = new NetworkCredential("f81887b0a57aa0603312a98443d32f40", "84d180273651fd6cfe3b95b5b4d653b7"),
            //    EnableSsl = true
            // };
            //client.Send(message);
            var message = new MailMessage();
            message.To.Add(new MailAddress(RecieverEmail)); // replace with valid value 
            message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
            message.Subject = Subject;
            message.Body = string.Format(body, "Devdiscourse");
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "noreply@devdiscourse.com", // replace with valid value
                    Password = "#visionRI@121#" // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.SendMailAsync(message);
            }
        }
        public void SendEmail(string RecieverEmail, string body, string Subject)
        {
            //var message = new MailMessage();
            //message.To.Add(new MailAddress(RecieverEmail)); // replace with valid value 
            //message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
            //message.Subject = Subject;
            //message.Body = body;
            //message.IsBodyHtml = true;
            //SmtpClient client = new SmtpClient("in.mailjet.com", 587)
            //{
            //    Credentials = new NetworkCredential("f81887b0a57aa0603312a98443d32f40", "84d180273651fd6cfe3b95b5b4d653b7"),
            //    EnableSsl = true
            //};
            var message = new MailMessage();
            message.To.Add(new MailAddress(RecieverEmail)); // replace with valid value 
            message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
            message.Subject = Subject;
            message.Body = body;
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "noreply@devdiscourse.com", // replace with valid value
                    Password = "#visionRI@121#" // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.SendMailAsync(message);
            }
        }
        public async Task SendEmailAsync(string RecieverEmail, string body, string Subject)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(RecieverEmail)); // replace with valid value 
            message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
            message.Subject = Subject;
            message.Body = body;
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "noreply@devdiscourse.com", // replace with valid value
                    Password = "#visionRI@121#" // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }
        //public void campaignMail()
        //{
        //    EmailController emailObj = new EmailController();
        //    string FilePath = Server.MapPath("~/Content/email-templates/email.html");
        //    string Emailbody;
        //    using (var sr = new StreamReader(FilePath))
        //    {
        //        Emailbody = sr.ReadToEnd();
        //    }
        //    var message = new MailMessage();
        //    message.To.Add(new MailAddress("sunnymultani111@gmail.com")); // replace with valid value 
        //    message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
        //    message.Subject = "Mother, Not Patient!";
        //    message.Body = Emailbody;
        //    message.IsBodyHtml = true;
        //    using (var smtp = new SmtpClient())
        //    {
        //        var credential = new NetworkCredential
        //        {
        //            UserName = "noreply@devdiscourse.com", // replace with valid value
        //            Password = "#visionRI@121#" // replace with valid value
        //        };
        //        smtp.Credentials = credential;
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.Port = 587;
        //        smtp.EnableSsl = true;
        //        smtp.Send(message);
        //    }
        //}
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}