using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Devdiscourse.Controllers
{
    public class AgencyWireController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AgencyWireController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            DateTime oneMonth = DateTime.Today.AddDays(-10);
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > oneMonth && a.Type == "News" && (a.Source == "PTI" || a.Source == "Reuters" || a.Source == "IANS")).Select(a => new PublisherView { ModifiedOn = a.CreatedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn);
            return View(search.ToPagedList((page ?? 1), 40));
        }
    }
}
