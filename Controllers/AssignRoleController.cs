using ClosedXML.Excel;
using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace DevDiscourse.Controllers
{
    public class AssignRoleController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AssignRoleController(ApplicationDbContext _db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._db = _db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        //[Authorize(Roles = "SuperAdmin")]
        // GET: AssignRole
        public ActionResult Index(string text = "", string role = "", int? page = 1)
        {
            ViewBag.searchText = text;
            ViewBag.selRole = role;
            ViewBag.Srno = (page - 1) * 20;


            var query = (from ur in _db.UserRoles
                         join u in _db.Users on ur.UserId equals u.Id
                         join r in _db.Roles on ur.RoleId equals r.Id
                         select new AssignedRoleView
                         {
                             Id = u.Id,
                             Email = u.Email,
                             UserName = u.UserName,
                             User = u.FirstName,
                             CreatedOn = u.CreatedOn,
                             OrganizationType = u.OrganizationType,
                             Role = r.Name,
                         });
            if (!string.IsNullOrEmpty(text))
            {
                query = query.Where(x => x.User.ToUpper().Contains(text.ToUpper()));
            }
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(x => x.Role == role);
            }

            List<AssignedRoleView> result = query.ToList();

            //List<AssignedRoleView> result = new List<AssignedRoleView>();
            //var roles = _db.Roles.ToList();
            //if (role != "")
            //{
            //    roles = roles.Where(a => a.Name == role).ToList();
            //}
            //foreach (var item in roles)
            //{
            //    var users = _db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(item.Id)).ToList();
            //    foreach (var m in users)
            //    {
            //        result.Add(UserList(m.Id, item.Id, item.Name, m.FirstName + " " + m.LastName, m.UserName, m.Email, m.CreatedOn));
            //    }
            //}

            //if (!String.IsNullOrEmpty(text))
            //{
            //    result = result.Where(a => a.User.ToUpper().Contains(text.ToUpper())).ToList();
            //}
            return View(result.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
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
        //[Authorize(Roles = "SuperAdmin")]
        public ActionResult Users(string name, string role, int? page)
        {
            name = name ?? "";
            ViewBag.name = name;
            ViewBag.role = role;

            var query = (from ur in _db.UserRoles
                         join u in _db.Users on ur.UserId equals u.Id
                         join r in _db.Roles on ur.RoleId equals r.Id
                         select new AssignedRoleView
                         {
                             Id = u.Id,
                             Email = u.Email,
                             UserName = u.UserName,
                             User = u.FirstName,
                             CreatedOn = u.CreatedOn,
                             OrganizationType = u.OrganizationType,
                             Role = r.Name,
                         });
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.User.ToUpper().Contains(name.ToUpper()));
            }
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(x => x.Role == role);
            }

            //var users = _db.Users.Where(x => x.FirstName.Contains(name));
            //if (!string.IsNullOrEmpty(role))
            //{
            //    var roleUser = _db.Roles.Single(a => a.Name == role).Id;
            //    users = users.Where(s => s.Roles.Any(r => r.RoleId == roleUser));
            //}

            return View(query.Select(s => new AssignedRoleView { Email = s.Email, Id = s.Id, UserName = s.UserName, User = s.User, CreatedOn = s.CreatedOn, OrganizationType = s.OrganizationType }).OrderByDescending(o => o.CreatedOn).ToPagedList((page ?? 1), 20));
        }
        // In admin panel for news, blog and event
        public async Task<PartialViewResult> GetUsers()
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
            return PartialView("_getUsers");
        }

        //[Authorize(Roles = "SuperAdmin")]
        public ActionResult Create(string id)
        {
            ViewBag.id = id;
            var user = _db.Users.Find(id);
            if (user == null)
            {
                throw new Exception();
            }
            else
            {
                ViewBag.user = user;
            }
            var userRole = _db.Roles.ToList();
            ViewBag.role = userRole;
            return View();
        }
        //[Authorize(Roles = "SuperAdmin")]
        public ActionResult Update(string id, string roleId)
        {
            ViewBag.id = id;
            ViewBag.roleId = roleId;
            var userRole = _db.Roles.ToList();
            // List of all roles
            ViewBag.role = userRole;
            ViewBag.roleTitle = userRole.Where(a => a.Id == roleId).Select(a => a.Name).FirstOrDefault();
            // Users Details
            var user = _db.Users.Find(id);
            ViewBag.user = user.FirstName + " " + user.LastName;
            return View();
        }
        //[Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(string id, string role)
        {
            ViewBag.id = id;
            ViewBag.role = role;
            // Users Details
            var user = _db.Users.Find(id);
            ViewBag.user = user.FirstName + " " + user.LastName;
            return View();
        }
        public DataTable GetData()
        {
            var lastHours = DateTime.UtcNow.AddHours(-12);
            DataTable dt = new DataTable();
            dt.TableName = "NewsData";
            var users = _db.Users.OrderBy(s => s.FirstName).ThenBy(l => l.LastName).Select(s => new { s.FirstName, s.LastName, s.Email }).AsQueryable();
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            foreach (var user in users)
            {
                dt.Rows.Add(user.FirstName + " " + user.LastName, user.Email);
            }
            dt.AcceptChanges();
            return dt;
        }
        //[Authorize(Roles = "SuperAdmin")]
        public ActionResult ExportUsers()
        {
            DataTable dt = GetData();
            string fileName = "Users.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
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