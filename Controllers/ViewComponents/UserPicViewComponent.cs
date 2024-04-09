using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class UserPicViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public UserPicViewComponent(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            this._db = _db;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = userManager.GetUserId(User as ClaimsPrincipal);
            if (!string.IsNullOrEmpty(userId))
            {
                ViewBag.profilePic = _db.Users.Find(userId)?.ProfilePic;
            }
            return View();
        }
    }
}
