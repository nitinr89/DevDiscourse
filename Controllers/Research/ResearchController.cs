using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ResearchModels;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using System.Web.Mvc;
using DevDiscourse.Controllers;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Host;

namespace Devdiscourse.Controllers.Research
{
    public class ResearchController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public ResearchController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {

            this._db = _db;
            this.userManager = userManager;
        }
        public ActionResult Index(int id = 0)
        {
            ViewBag.sdgGoalId = id;
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.edition = "Global Edition";
            }
            else
            {
                ViewBag.edition = cookie ?? "Global Edition";
            }
            ViewBag.loginId = userManager.GetUserId(User);
            return View();
        }
        [System.Web.Mvc.Authorize]
        public ActionResult Dashboard()
        {
            string id = userManager.GetUserId(User);
            if (User.IsInRole("InstitutionalPartner") && id != "")
            {
                ViewBag.userInfo = _db.Users.Find(id);
                var search = _db.AdoptSDGTools.FirstOrDefault(a => a.UserId == id);
                ViewBag.sectorcount = search.ThemeticArea.Split(',').Count();
                ViewBag.sector = search.ThemeticArea;
                ViewBag.location = search.GeographicalData.Split(',')[0];
            }
            else if (User.IsInRole("SuperAdmin"))
            {
                ViewBag.userInfo = _db.Users.Find(id);
                ViewBag.sectorcount = 9;
                ViewBag.location = "";
            }
            return View();
        }
        [System.Web.Mvc.Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ResearchFeedBack()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //1
        public ActionResult NoPoverty()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //2
        public ActionResult ZeroHunger()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //3
        public ActionResult GoodHealth()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //4
        public ActionResult QualityEducation()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //5
        public ActionResult GenderEquality()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //6
        public ActionResult CleanWater()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //7
        public ActionResult Affordable()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //8
        public ActionResult DecentWork()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //9
        public ActionResult IndustryInnovation()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //10
        public ActionResult ReducedInequalities()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //11
        public ActionResult SustainableCities()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //12
        public ActionResult ResponsibleConsumption()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //13
        public ActionResult ClimateAction()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //14
        public ActionResult LifeBelowWater()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //15
        public ActionResult LifeOnLand()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //16
        public ActionResult PeaceJustice()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        //17
        public ActionResult Partnerships()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        public ActionResult AdvisoryPanel()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        public ActionResult SDGSamurai()
        {
            ViewBag.edition = "Global Edition";
            return View();
        }
        public ActionResult AdopTools(int? page = 1)
        {
            ViewBag.edition = "Global Edition";
            return View(_db.AdoptSDGTools.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }
        public ActionResult SDGType()
        {
            return PartialView("_SDGType");
        }
        // Partial Views
        //public PartialViewResult GetAdvisoryPanel(int skip = 0)
        //{
        //    ViewBag.skipCount = skip;
        //    var search = _db.AdvisoryPanels.OrderBy(a => a.SrNo).Skip(skip).Take(12).ToList();
        //    return PartialView("_getAdvisoryPanel", search);
        //}
        //public PartialViewResult GetPanelMember()
        //{
        //    var search = _db.AdvisoryPanels.OrderBy(a => a.SrNo).Take(9).ToList();
        //    return PartialView("_getPanelMember", search);
        //}
        //public PartialViewResult GetInstPartners()
        //{
        //    var search = _db.AdoptSDGTools.OrderByDescending(a => a.CreatedOn).ToList();
        //    return PartialView("_getInstPartners", search);
        //}
        //public PartialViewResult GetSDGs(int skip = 0)
        //{
        //    ViewBag.skipCount = skip;
        //    var search = _db.SDGSamurais.OrderByDescending(a => a.SDGPosition).Skip(skip).Take(9).ToList();
        //    return PartialView("_getSDGs", search);
        //}
        //public PartialViewResult GetSamuai()
        //{
        //    DateTime today = DateTime.UtcNow;
        //    DateTime weekend = DateTime.UtcNow.AddDays(8).AddTicks(-1);
        //    var search = _db.SDGSamurais.OrderByDescending(a => a.SDGPosition).Take(12).ToList();
        //    return PartialView("_getSamuai", search);
        //}
        public Microsoft.AspNetCore.Mvc.JsonResult GetAmpSDGNews(string __amp_source_origin, int? moreItemsPageIndex)
        {
            ViewBag.reg = "Global Edition";
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.Category.Contains("13")).Select(a => new { a.Region, a.IsGlobal, a.Sector, a.ModifiedOn, a.Title, Url = "/article/" + a.NewsId.ToString(), a.ImageUrl });
            if (!string.IsNullOrEmpty(__amp_source_origin))
            {
               // HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);// do later

            }
            var result = search.OrderByDescending(m => m.ModifiedOn).Select(b => new { b.Title, b.Url, b.ImageUrl });
            int pageSize = 10;
            int pageNumber = (moreItemsPageIndex ?? 1);
            var resultData = result.ToPagedList(pageNumber, pageSize);
            return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        }
        public Microsoft.AspNetCore.Mvc.PartialViewResult GoalDetails(int? id)
        {
            if (id == null || id == 0)
            {
                throw new HttpException(404, "Error 404");
            }
            switch (id)
            {
                case 1:
                    return PartialView("_goal1");
                case 2:
                    return PartialView("_goal2");
                case 3:
                    return PartialView("_goal3");
                case 4:
                    return PartialView("_goal4");
                case 5:
                    return PartialView("_goal5");
                case 6:
                    return PartialView("_goal6");
                case 7:
                    return PartialView("_goal7");
                case 8:
                    return PartialView("_goal8");
                case 9:
                    return PartialView("_goal9");
                case 10:
                    return PartialView("_goal10");
                case 11:
                    return PartialView("_goal11");
                case 12:
                    return PartialView("_goal12");
                case 13:
                    return PartialView("_goal13");
                case 14:
                    return PartialView("_goal14");
                case 15:
                    return PartialView("_goal15");
                case 16:
                    return PartialView("_goal16");
                case 17:
                    return PartialView("_goal17");
                default:
                    return PartialView("");
            }
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }
        public Microsoft.AspNetCore.Mvc.JsonResult SDGsJoin(string profession, string description, string nationality, string gender)
        {
            string userId = userManager.GetUserId(User);
            var search = _db.SDGSamurais.FirstOrDefault(a => a.Creator == userId);
            if (search != null)
            {
                return Json("Already Joined", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var user = _db.Users.Find(userId);
                string sdgCode = RandomString();
                SDGSamurai obj = new SDGSamurai
                {
                    Name = user.FirstName + " " + user.LastName,
                    Profession = profession,
                    Description = description,
                    Email = user.Email,
                    Nationality = nationality,
                    Gender = gender,
                    IsActive = true,
                    SDGCode = sdgCode,
                    ReferenceCode = "",
                    SDGPoints = 1.0,
                    SDGPosition = 1,
                    Creator = userId
                };
                _db.SDGSamurais.Add(obj);
                _db.SaveChanges();
                CreateLog("SDG Samurai", obj.Ranks.Title, obj.Id.ToString(), obj.Creator, "/Research/");
                return Json("Successfully Join", JsonRequestBehavior.AllowGet);
            }
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
        public Microsoft.AspNetCore.Mvc.JsonResult AdoptTool(string name, string institution, string location, string email, string contact, string neededData, string geoData, string themeticArea, string message)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json("Something went wrong!", JsonRequestBehavior.AllowGet);
            }
            var search = _db.AdoptSDGTools.FirstOrDefault(a => a.Email == email);
            if (search != null)
            {
                return Json("Already Adopt this user", JsonRequestBehavior.AllowGet);
            }
            else
            {
                AdoptSDGTool obj = new AdoptSDGTool
                {
                    Name = name,
                    Institution = institution,
                    Location = location,
                    Email = email,
                    Contact = contact,
                    NeededData = neededData,
                    GeographicalData = geoData,
                    ThemeticArea = themeticArea,
                    Message = message,
                    IsActive = false,
                    UserId = "",
                };
                _db.AdoptSDGTools.Add(obj);
                _db.SaveChanges();

                EmailController emailobj = new EmailController();
                string emailData = string.Format("<div style='padding:30px;background-color:#ececec'><div style='margin-left:32%'><img src='http://www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></div>" +
                         "<div style='font-size:20px; padding-top:30px;'><span>Thanks for your valuable feedback. Devdiscourse team will revert you back.</span></div>" +
                         "<div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
                         "© Copyright 2017 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span><span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
                        "<span><a href='http://devdiscourse.com/Home/PrivacyPolicy' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");

                emailobj.SendMail(email, emailData, "Institutional Partnership");
                return Json("Successfully Adopt", JsonRequestBehavior.AllowGet);
            }
        }
        public Microsoft.AspNetCore.Mvc.JsonResult GetUserPic(string email)
        {
            var search = _db.Users.FirstOrDefault(a => a.Email == email).ProfilePic;
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        // Generate Random Code String
        public string RandomString()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            return FormNumber.ToString("X");
        }
        public ActionResult Map(string id = "", string userId = "")
        {
            ViewBag.userId = userId;
            ViewBag.fid = id;
            return View();
        }
        [System.Web.Mvc.Authorize]
        public ActionResult SummaryReport()
        {
            string id = userManager.GetUserId(User);
            if (User.IsInRole("InstitutionalPartner") && id != "")
            {
                ViewBag.userInfo = _db.Users.Find(id);
                var search = _db.AdoptSDGTools.FirstOrDefault(a => a.UserId == id);
                ViewBag.sectorcount = search.ThemeticArea.Split(',').Count();
                ViewBag.sector = search.ThemeticArea;
                ViewBag.location = search.GeographicalData.Split(',')[0];
            }
            else if (User.IsInRole("SuperAdmin"))
            {
                ViewBag.sectorcount = 9;
                ViewBag.location = "";
            }
            return View();
        }
        [System.Web.Mvc.Authorize]
        public ActionResult Feedbacks()
        {
            string id = userManager.GetUserId(User);
            if (User.IsInRole("InstitutionalPartner") && id != "")
            {
                ViewBag.userInfo = _db.Users.Find(id);
                var search = _db.AdoptSDGTools.FirstOrDefault(a => a.UserId == id);
                ViewBag.sectorcount = search.ThemeticArea.Split(',').Count();
                ViewBag.sector = search.ThemeticArea;
                ViewBag.location = search.GeographicalData.Split(',')[0];
            }
            else if (User.IsInRole("SuperAdmin"))
            {
                ViewBag.sectorcount = 9;
                ViewBag.location = "";
            }
            return View();
        }
        public ActionResult UserResponse(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult SurveyResponse(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ViewBag.id = id;
            string userId = userManager.GetUserId(User);
            if (!String.IsNullOrEmpty(userId))
            {
                ViewBag.userEmail = _db.Users.Find(userId).Email;
            }
            return View();
        }
        public ActionResult MobileResponse(Guid id, string location, string userId = "", string backTo = "responses")
        {
            ViewBag.id = id;
            ViewBag.backTo = backTo;
            ViewBag.location = location;
            ViewBag.userId = userId;
            var user = _db.Users.Find(userId);
            if (user != null)
            {
                ViewBag.fullName = user.FirstName + " " + user.LastName;
                ViewBag.avatar = user.ProfilePic;
            }
            return View();
        }
        public Microsoft.AspNetCore.Mvc.JsonResult ResponseReport(Guid? id, string text, string question)
        {
            if (id == null || id == new Guid())
            {
                return Json("Something went wrong!", JsonRequestBehavior.AllowGet);
            }
            string userId = userManager.GetUserId(User);
            var search = _db.ResponseReports.FirstOrDefault(a => a.ReportBy == userId && a.ResponseId == id);
            if (search != null)
            {
                return Json("Already report on this response!", JsonRequestBehavior.AllowGet);
            }
            else
            {
                ResponseReport resObj = new ResponseReport
                {
                    ResponseId = Guid.Parse(id.ToString()),
                    ResponseTitle = question,
                    ReportText = text,
                    ReportBy = userId,
                    ReportedOn = DateTime.UtcNow,
                };
                _db.ResponseReports.Add(resObj);
                _db.SaveChanges();
                return Json("Submit successfully!", JsonRequestBehavior.AllowGet);
            }
        }
        public Microsoft.AspNetCore.Mvc.JsonResult SaveComment(Guid itemId, string itemTitle, string comment, long parentId)
        {
            string userId = userManager.GetUserId(User);
            UserComment obj = new UserComment()
            {
                CommentText = comment,
                ItemId = itemId,
                ItemTitle = itemTitle,
                ParentId = parentId,
                CommentBy = userId,
                CommentOn = DateTime.UtcNow,
            };
            _db.UserComments.Add(obj);
            _db.SaveChanges();
            return Json("Success!", JsonRequestBehavior.AllowGet);
        }
        public Microsoft.AspNetCore.Mvc.JsonResult MobileResponseReport(Guid id, string text, string question, string userId)
        {
            var search = _db.ResponseReports.FirstOrDefault(a => a.ReportBy == userId && a.ResponseId == id);
            if (search != null)
            {
                return Json("Already report on this response!", JsonRequestBehavior.AllowGet);
            }
            else
            {
                ResponseReport resObj = new ResponseReport
                {
                    ResponseId = id,
                    ResponseTitle = question,
                    ReportText = text,
                    ReportBy = userId,
                    ReportedOn = DateTime.UtcNow,
                };
                _db.ResponseReports.Add(resObj);
                _db.SaveChanges();
                return Json("Submit successfully!", JsonRequestBehavior.AllowGet);
            }
        }
        public Microsoft.AspNetCore.Mvc.JsonResult SaveMobileComment(Guid itemId, string itemTitle, string comment, long parentId, string userId)
        {
            UserComment obj = new UserComment()
            {
                CommentText = comment,
                ItemId = itemId,
                ItemTitle = itemTitle,
                ParentId = parentId,
                CommentBy = userId,
                CommentOn = DateTime.UtcNow,
            };
            _db.UserComments.Add(obj);
            _db.SaveChanges();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        //public PartialViewResult GetComments(Guid itemId)
        //{
        //    ViewBag.loginId = User.Identity.GetUserId();
        //    var search = _db.UserComments.Where(a => a.ItemId == itemId && a.ParentId == 0);
        //    return PartialView("_getComments", search.OrderByDescending(a => a.CommentOn).ToList());
        //}
        //public PartialViewResult GetMobileComments(Guid itemId, string userId)
        //{
        //    ViewBag.loginId = userId;
        //    var search = _db.UserComments.Where(a => a.ItemId == itemId && a.ParentId == 0);
        //    return PartialView("_getComments", search.OrderByDescending(a => a.CommentOn).ToList());
        //}
        //public PartialViewResult GetReplyComments(long id)
        //{
        //    ViewBag.loginId = User.Identity.GetUserId();
        //    var search = _db.UserComments.Where(a => a.ParentId == id);
        //    return PartialView("_getReplyComments", search.OrderByDescending(a => a.CommentOn).ToList());
        //}
        public Microsoft.AspNetCore.Mvc.JsonResult EditComment(long id, string text)
        {
            var search = _db.UserComments.Find(id);
            search.CommentText = text;
            _db.Entry(search).State = EntityState.Modified;
            _db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public Microsoft.AspNetCore.Mvc.JsonResult LikeDislike(Guid id, string question, Behaviour behaviour)
        {
            string userId = userManager.GetUserId(User);
            var search = _db.UserBehaviours.FirstOrDefault(a => a.ResponseId == id && a.Creator == userId);
            if (search != null)
            {
                search.BehaviourType = behaviour;
                _db.Entry(search).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                UserBehaviour obj = new UserBehaviour()
                {
                    ResponseTitle = question,
                    ResponseId = id,
                    BehaviourType = behaviour,
                    Creator = userId,
                    LikeCount = 0,
                    CreatedOn = DateTime.UtcNow,
                };
                _db.UserBehaviours.Add(obj);
                _db.SaveChanges();
            }
            var responseCount = _db.UserBehaviours.Where(a => a.ResponseId == id);
            return Json(new { like = responseCount.Count(a => a.BehaviourType == Behaviour.Like), dislike = responseCount.Count(a => a.BehaviourType == Behaviour.Dislike) }, JsonRequestBehavior.AllowGet);
        }
        public Microsoft.AspNetCore.Mvc.JsonResult MobileLikeDislike(Guid id, string question, string userId, Behaviour behaviour)
        {
            var search = _db.UserBehaviours.FirstOrDefault(a => a.ResponseId == id && a.Creator == userId);
            if (search != null)
            {
                search.BehaviourType = behaviour;
                _db.Entry(search).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                UserBehaviour obj = new UserBehaviour()
                {
                    ResponseTitle = question,
                    ResponseId = id,
                    BehaviourType = behaviour,
                    Creator = userId,
                    LikeCount = 0,
                    CreatedOn = DateTime.UtcNow,
                };
                _db.UserBehaviours.Add(obj);
                _db.SaveChanges();
            }
            var responseCount = _db.UserBehaviours.Where(a => a.ResponseId == id);
            return Json(new { like = responseCount.Count(a => a.BehaviourType == Behaviour.Like), dislike = responseCount.Count(a => a.BehaviourType == Behaviour.Dislike) }, JsonRequestBehavior.AllowGet);
        }
        public Microsoft.AspNetCore.Mvc.JsonResult GetCount(Guid id)
        {
            var responseCount = _db.UserBehaviours.Where(a => a.ResponseId == id);
            return Json(new { like = responseCount.Count(a => a.BehaviourType == Behaviour.Like), dislike = responseCount.Count(a => a.BehaviourType == Behaviour.Dislike) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NearContribution(string id, string location)
        {
            ViewBag.id = id;
            ViewBag.location = location;
            return View();
        }
        public ActionResult Contest()
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
        public ActionResult ContestComingSoon()
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
        [System.Web.Mvc.Authorize]
        public ActionResult ResearchSurvey()
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
        [System.Web.Mvc.Authorize]
        public ActionResult SubmitSurvey(string id)
        {
            var userId = userManager.GetUserId(User);
            var userInfo = _db.Users.Find(userId);
            if (userInfo != null)
            {
                ViewBag.email = userInfo.Email;
                ViewBag.name = userInfo.FirstName + " " + userInfo.LastName;
                ViewBag.userId = userInfo.Id;
                ViewBag.image = userInfo.ProfilePic;
            }
            ViewBag.id = id;
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
