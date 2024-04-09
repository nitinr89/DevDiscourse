using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class ProfileViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public ProfileViewComponent(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            this._db = _db;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = userManager.GetUserId(User as ClaimsPrincipal);
            var user = _db.Users.Where(a => a.Id == userId).Select(a => new UserView { FirstName = a.FirstName, LastName = a.LastName, AboutMe = a.AboutMe, Address = a.Address, Contact = a.PhoneNumber, Country = a.Country, Email = a.Email, ProfilePic = a.ProfilePic });
            ViewBag.sector = _db.DevSectors.OrderBy(a => a.SrNo).ToList();
            return View(user);
        }
    }
}
