using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.Others
{
    public class NewsWireController : Controller
    {
        public ActionResult Index()
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
    }
}
