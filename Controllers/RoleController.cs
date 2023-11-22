using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers
{
    //[Authorize]
    public class RoleController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public RoleController(ApplicationDbContext _db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._db = _db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {

            //if (User.Identity.IsAuthenticated)
            //{
            //    if (!await isAdminUser())
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            var Roles = _db.Roles.ToList();
            return View(Roles);

        }
        public async Task<Boolean> isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
                var userRoles = await userManager.GetRolesAsync(user);
                if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    if (!await isAdminUser())
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(IdentityRole Role)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    if (!await isAdminUser())
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            var result = await roleManager.CreateAsync(Role);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else return View();
        }
        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Edit(string id = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!await isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            IdentityRole role = _db.Roles.Find(id);
            return View(role);
        }
        /// <summary>
        /// Edit a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Edit(IdentityRole Role)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!await isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            _db.Roles.Update(Role);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        // By Default First Role Create
        //public void CreateRole()
        //{
        //    List<string> roleList = new List<string>();
        //    roleList.Add("Admin");
        //    roleList.Add("Subscriber");
        //    foreach (var item in roleList)
        //    {
        //        IdentityRole Role = new IdentityRole();
        //        Role.Name = item;
        //        _db.Roles.Add(Role);
        //        _db.SaveChanges();
        //    }
        //}
        //public void CreateSubscriberRole()
        //{
        //    IdentityRole Role = new IdentityRole();
        //    Role.Name = "Subscriber";
        //    _db.Roles.Add(Role);
        //    _db.SaveChanges();
        //}
        // Assign Admin Role To User

        public async Task<IActionResult> AssignRole()
        {
            var user = await userManager.FindByNameAsync("uttam");
            if (user != null)
            {
                var roleExists = await roleManager.RoleExistsAsync("Admin");
                if (!roleExists)
                {
                    var role = new IdentityRole("Admin");
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {

                    }
                    else
                    {
                        return Ok(result.Errors.ToString());
                    }
                }
                bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin) return Ok("You are already Admin!!");
                else
                {
                    var result = await userManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        return Ok("You are Admin now!!");
                    }
                    else
                    {
                        return Ok(result.Errors.ToString());
                    }
                }
            }
            return Ok("User Not Found, Try Again!!");
        }

        public async Task<JsonResult> AssignRoleToUser(string id, string role)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                    return Json(new { status = "Success", msg = "Role Assigned Successfully" });
                else return Json(new { status = "Error", msg = "Role can't Assign" });
            }
            else
            {
                return Json(new { status = "Error", msg = "User ID is Invalid" });
            }
        }
        // Assigned Role Change
        //public async void ChangeRole()
        //{
        //    var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
        //    var userRoles = await userManager.GetRolesAsync(user);
        //    if (userRoles.Count == 1 && userRoles.Contains("Subscriber"))
        //    {
        //        await userManager.AddToRoleAsync(user, "Author");
        //    }
        //}
        public async Task<JsonResult> UpdateRole(string userId, string oldRole, string newrole)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user == null) { return Json(new { status = "Error", msg = "User ID is Invalid" }); }
            IList<string>? userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                if (role == oldRole) await userManager.RemoveFromRoleAsync(user, role);
            }
            var result = await userManager.AddToRoleAsync(user, newrole);
            if (result.Succeeded)
                return Json(new { status = "Success", msg = "User's Role Updated Successfully" });
            else return Json(new { status = "Error", msg = "User's Role can't Update" });
        }
        //public async Task<ActionResult> UpdateUserRole(string role)
        //{
        //    var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
        //    var userRoles = await userManager.GetRolesAsync(user);
        //    foreach (var r in userRoles)
        //    {
        //        await userManager.RemoveFromRoleAsync(user, r);
        //    }
        //    if (role == "Contributor" || role == "PressRelease")
        //    {
        //        await userManager.AddToRoleAsync(user, role);
        //    }
        //    if (role == "Contributor")
        //    {
        //        return RedirectToAction("Index", "ContentCreator");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Dashboard", "NewsWire");
        //    }
        //}
        public async Task<JsonResult> DeleteRole(string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.RemoveFromRoleAsync(user, role);
                if (result.Succeeded)
                    return Json(new { status = "Success", msg = "User's Role Removed Successfully" });
                else return Json(new { status = "Error", msg = "User's Role can't Remove" });
            }
            else
            {
                return Json(new { status = "Error", msg = "User ID is Invalid" });
            }
        }
        public JsonResult GetRoles()
        {
            var roles = _db.Roles.Select(a => new { a.Name, a.Id }).ToList();
            return Json(roles);
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