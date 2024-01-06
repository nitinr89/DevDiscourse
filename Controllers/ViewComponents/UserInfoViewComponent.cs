using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class UserInfoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public UserInfoViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string UserId = "")
        {
            if (UserId == "")
            {
                UserId = userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);
            }
            ViewBag.userInfo = db.Users.Find(UserId);
            // Total Stories Count
            ViewBag.totalStories = db.NewsContents.Count(a => a.Creator == UserId);
            // Total Views on Devdiscourse
            var find = db.Earnings.Where(a => a.Creator == UserId).ToList();
            ViewBag.totalViews = find.Sum(a => a.ViewCount);
            return View();
        }
    }
}
