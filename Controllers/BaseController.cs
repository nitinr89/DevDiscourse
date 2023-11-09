using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace DevDiscourse.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Request.Cookies.ContainsKey("timezoneoffset"))
            {
                string timezoneOffset = HttpContext.Request.Cookies["timezoneoffset"];
                HttpContext.Session.SetString("timezoneoffset", timezoneOffset);
            }

            base.OnActionExecuting(context);
        }
    }
}