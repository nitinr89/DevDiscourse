using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.Others;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DevDiscourse.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public AdminController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            this._db = _db;
            this.userManager = userManager;
        }

        // GET: Admin
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminView()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Media()
        {
            string? userId = userManager.GetUserId(User);
            var search = _db.UserWorks.FirstOrDefault(a => a.UserId == userId);
            if (search == null)
            {
                ViewBag.workStage = "Allow";
                ViewBag.userName = "";
            }
            else
            {
                ViewBag.workStage = "Not Allow";
                ViewBag.userName = search.UserName;
            }
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Dashboard()
        {
            DateTime startDateTime = DateTime.Today;
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime yesterdayDate = DateTime.Today.AddDays(-1);
            DateTime weeklyDate = DateTime.Today.AddDays(-7).AddTicks(-1);
            ViewBag.todayCount = _db.DevNews.Where(a => a.CreatedOn >= startDateTime && a.CreatedOn <= endDateTime).Sum(b => b.ViewCount);
            ViewBag.yesterdayCount = _db.DevNews.Where(a => a.CreatedOn < startDateTime && a.CreatedOn >= yesterdayDate).Sum(b => b.ViewCount);
            ViewBag.monthlyCount = _db.DevNews.Where(a => a.CreatedOn <= yesterdayDate && a.CreatedOn >= weeklyDate).Sum(b => b.ViewCount);
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Users(string text = "", int? page = 1)
        {
            ViewBag.searchText = text;
            ViewBag.Srno = (page - 1) * 10;
            var search = _db.Users.ToList();
            if (!string.IsNullOrEmpty(text))
            {
                search = search.Where(a => a.FirstName.Contains(text)).ToList();
            }
            //return View(search.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
            return View(search);
        }
        public AuthorView dataList(string user, int count)
        {
            AuthorView obj = new AuthorView
            {
                Author = user,
                NewsCount = count
            };
            return obj;
        }
        public void CreateLog(string title, string logFor, string creator, string activityToUser, string activityUrl)
        {
            ActivityLog logs = new ActivityLog
            {
                LogTitle = title,
                LogDescription = title,
                CreatorId = creator,
                ActivityUserId = activityToUser,
                Activity = logFor,
                ActivityUrl = activityUrl,
                ActivityDate = DateTime.Now,
                IsRead = false
            };
            _db.ActivityLogs.Add(logs);
            _db.SaveChanges();
        }
        public JsonResult SaveWorkStage(string stage, string bgclass)
        {
            string? userId = userManager.GetUserId(User);
            string userName = "";
            if (userId != null)
            {
                var user = _db.Users.Find(userId);
                if (user != null)
                {
                    userName = user.FirstName;
                }
            }
            var search = _db.UserWorks.FirstOrDefault(a => a.UserId == userId);
            if (search == null)
            {
                UserWork obj = new UserWork
                {
                    WorkStage = stage,
                    ColorCode = bgclass,
                    UserName = userName,
                    UserId = userId == null ? "" : userId,
                    CreatedOn = DateTime.UtcNow
                };
                _db.UserWorks.Add(obj);
                _db.SaveChanges();
            }
            else
            {
                search.WorkStage = stage;
                search.ColorCode = bgclass;
                _db.UserWorks.Update(search);
                _db.SaveChanges();
            }
            return Json("Success");
        }
        public async Task<ActionResult> ManageShift()
        {
            var Role = _db.Roles.FirstOrDefault(m => m.Name == "Upfront");
            if (Role != null && Role.Name != null)
            {
                var Users = await userManager.GetUsersInRoleAsync(Role.Name);
                List<ShiftUser> shiftUsers = new List<ShiftUser>();
                foreach (var a in Users)
                {
                    shiftUsers.Add(new ShiftUser { FirstName = a.FirstName, LastName = a.LastName, UserId = a.Id, isActice = a.isPRManager });
                }
                return View(shiftUsers);
            }
            return View(new List<ShiftUser>());
        }
        public async Task<ActionResult> PressReleaseManager()
        {
            var Role = _db.Roles.FirstOrDefault(m => m.Name == "Press Release Manager");
            if (Role != null && Role.Name != null)
            {
                var Users = await userManager.GetUsersInRoleAsync(Role.Name);
                List<ShiftUser> shiftUsers = new List<ShiftUser>();
                foreach (var a in Users)
                {
                    shiftUsers.Add(new ShiftUser { FirstName = a.FirstName, LastName = a.LastName, UserId = a.Id, isActice = a.isPRManager });
                }
                return View(shiftUsers);
            }
            return View(new List<ShiftUser>());
        }
        public JsonResult UpdatePressReleaseManager(string userId, bool status)
        {
            var User = _db.Users.Find(userId);
            if (User != null)
            {
                User.isPRManager = status;
                _db.Users.Update(User);
                _db.SaveChanges();
                return Json("Success");
            }
            return Json("Failed");
        }
        public async Task<JsonResult> UpdateNewsLabel(string Id, string label)
        {
            var userNewsLabel = await _db.UserNewsLabels.FirstOrDefaultAsync(a => a.UserId == Id && a.NewsLabel == label);
            if (userNewsLabel == null)
            {
                UserNewsLabel userlabel = new UserNewsLabel();
                userlabel.UserId = Id;
                userlabel.NewsLabel = label;
                _db.UserNewsLabels.Add(userlabel);
                await _db.SaveChangesAsync();
                return Json(new { msg = "Added", result = new { Id, label } });
            }
            else
            {
                _db.UserNewsLabels.Remove(userNewsLabel);
                await _db.SaveChangesAsync();
                return Json(new { msg = "Delete", result = new { Id, label } });
            }
        }
        public JsonResult UpdateShiftStatus(string userId, bool status)
        {
            var User = _db.Users.Find(userId);
            if (User != null)
            {
                User.isActive = status;
                _db.Users.Update(User);
                _db.SaveChanges();
                return Json("Success");
            }
            return Json("Failed");
        }
        public JsonResult GetShiftUser()
        {
            var ShiftUsers = _db.Users.Where(m => m.isActive == true).Select(s => new { s.FirstName, s.LastName, s.Id });
            return Json(ShiftUsers);
        }
        //public JsonResult PTIAdminCheck()
        //{
        //    int status = 1;
        //    if (System.IO.File.Exists(Server.MapPath("~/Content/PTI.txt")))
        //    {
        //        string statusText = System.IO.File.ReadAllText(Server.MapPath("~/Content/PTI.txt"));
        //        status = Int32.Parse(statusText);
        //    }
        //    if (status == 0)
        //    {
        //        System.IO.File.WriteAllText(Server.MapPath("~/Content/PTI.txt"), "1");
        //    }
        //    else
        //    {
        //        System.IO.File.WriteAllText(Server.MapPath("~/Content/PTI.txt"), "0");
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult ReutersAdminCheck()
        //{
        //    int status = 1;
        //    if (System.IO.File.Exists(Server.MapPath("~/Content/Reuters.txt")))
        //    {
        //        string statusText = System.IO.File.ReadAllText(Server.MapPath("~/Content/Reuters.txt"));
        //        status = Int32.Parse(statusText);
        //    }
        //    if (status == 0)
        //    {
        //        System.IO.File.WriteAllText(Server.MapPath("~/Content/Reuters.txt"), "1");
        //    }
        //    else
        //    {
        //        System.IO.File.WriteAllText(Server.MapPath("~/Content/Reuters.txt"), "0");
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult IANSAdminCheck()
        //{
        //    int status = 1;
        //    if (System.IO.File.Exists(Server.MapPath("~/Content/IANS.txt")))
        //    {
        //        string statusText = System.IO.File.ReadAllText(Server.MapPath("~/Content/IANS.txt"));
        //        status = Int32.Parse(statusText);
        //    }
        //    if (status == 0)
        //    {
        //        System.IO.File.WriteAllText(Server.MapPath("~/Content/IANS.txt"), "1");
        //    }
        //    else
        //    {
        //        System.IO.File.WriteAllText(Server.MapPath("~/Content/IANS.txt"), "0");
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}
        //public bool AdminCheckStatus(string source)
        //{
        //    if(source == "PTI")
        //    {
        //        if (System.IO.File.Exists(Server.MapPath("~/Content/PTI.txt")))
        //        {
        //            string statusText = System.IO.File.ReadAllText(Server.MapPath("~/Content/PTI.txt"));
        //            if(statusText == "0")
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    else if (source == "Reuters")
        //    {
        //        if (System.IO.File.Exists(Server.MapPath("~/Content/Reuters.txt")))
        //        {
        //            string reutersText = System.IO.File.ReadAllText(Server.MapPath("~/Content/Reuters.txt"));
        //            if (reutersText == "0")
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (System.IO.File.Exists(Server.MapPath("~/Content/IANS.txt")))
        //        {
        //            string iansText = System.IO.File.ReadAllText(Server.MapPath("~/Content/IANS.txt"));
        //            if (iansText == "0")
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        public ActionResult NewsAnalysis()
        {
            return View();
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