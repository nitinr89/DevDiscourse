using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class InterestViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public InterestViewComponent(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            this._db = _db;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = userManager.GetUserId(User as ClaimsPrincipal);
            var search = _db.UserInterests.Where(a => a.UserId == userId && a.InterestType != "AutoDefined").ToList();
            return View(search);
        }
    }
}
