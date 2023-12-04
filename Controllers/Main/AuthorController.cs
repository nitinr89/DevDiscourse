using Devdiscourse.Models;
using Devdiscourse.Models.ViewModel;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;

namespace DevDiscourse.Controllers.Main
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AuthorController(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        // GET: Author
        public ActionResult Index(string name)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            var Author = _db.Users.FirstOrDefault(a => a.UserName == name);
            ViewBag.Author = Author ?? throw new System.Net.Http.HttpRequestException("Error 404: Author not found");
            return View();
        }
        public PartialViewResult GetUserNews(string name, int? page)
        {
            ViewBag.name = name;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var news = _db.DevNews.Where(a => a.ApplicationUsers.UserName == name && a.OriginalSource == "Devdiscourse News Desk" && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new LatestNewsView { NewId = s.NewsId, Title = s.Title, CreatedOn = s.CreatedOn, ImageUrl = s.ImageUrl, Label = s.NewsLabels }).ToPagedList(pageNumber, pageSize);
            return PartialView("_getUserNews", news);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}