using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class UsersViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public UsersViewComponent(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            this._db = _db;
            this.userManager = userManager;
        }
        public AssignedRoleView UserList(string id, string roleid, string role, string user, string username, string email, DateTime date)
        {
            AssignedRoleView obj = new AssignedRoleView
            {
                Id = id,
                RoleId = roleid,
                Role = role,
                User = user,
                Email = email,
                UserName = username,
                CreatedOn = date
            };
            return obj;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<AssignedRoleView> result = new List<AssignedRoleView>();
            var roles = _db.Roles.Where(a => a.Name == "Admin" || a.Name == "Upfront" || a.Name == "Author").ToList();
            foreach (var item in roles)
            {
                var users = await userManager.GetUsersInRoleAsync(item.Name);
                //var users = _db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(item.Id)).ToList();
                foreach (var m in users)
                {
                    result.Add(UserList(m.Id, item.Id, item.Name, m.FirstName + " " + m.LastName, m.UserName, m.Email, m.CreatedOn));
                }
            }
            ViewBag.data = result;
            return View();
        }
    }
}
