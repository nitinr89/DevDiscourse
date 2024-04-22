using System.Net;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using Devdiscourse.Models.ViewModel;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Devdiscourse.Hubs;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;

namespace DevDiscourse.Controllers.Main
{
    public class EventsController : BaseController
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<ChatHub> context;
        public EventsController(ApplicationDbContext db, IWebHostEnvironment _environment, UserManager<ApplicationUser> userManager, IHubContext<ChatHub> context)
        {
            this.db = db;
            this._environment = _environment;
            this.userManager = userManager;
            this.context = context;
        }

        // GET: Events
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin,Admin,Author")]
        public ActionResult Index(DateTime? bfd, DateTime? afd, int? page = 1, string region = "Global Edition", string sector = "0", string category = "0", string country = "", string text = "", string uid = "")
        {
            ViewBag.sector = sector;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            ViewBag.uid = uid;
            ViewBag.loginId = userManager.GetUserId(User);
            var events = db.Events.Where(a => a.Creator != null);
            if (sector != "0")
            {
                events = events.Where(a => a.Sector.Contains(sector));
            }
            if (category != "0")
            {
                events = events.Where(a => a.Category.Contains(category));
            }
            if (region != "Global Edition")
            {
                events = events.Where(a => a.Region.ToUpper() == region.ToUpper());
            }
            if (!String.IsNullOrEmpty(country))
            {
                events = events.Where(a => a.Country.ToUpper() == country.ToUpper());
            }
            if (!String.IsNullOrEmpty(text))
            {
                events = events.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
            }
            if (bfd != null)
            {
                DateTime filterDate = DateTime.Parse(bfd.ToString());
                events = events.Where(a => a.CreatedOn < filterDate);
            }
            if (afd != null)
            {
                DateTime filterDate2 = DateTime.Parse(afd.ToString());
                events = events.Where(a => a.CreatedOn > filterDate2);
            }
            if (!String.IsNullOrEmpty(uid))
            {
                events = events.Where(a => a.Creator == uid);
            }
            return View(events.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        // GET: Events/Details/5
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin,Admin,Author")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews? devnews = db.DevNews.Find(id);
            if (devnews == null)
            {
                return NotFound();
            }
            return View(devnews);
        }

        // GET: Events/Create
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin,Admin,Author")]
        public ActionResult Create()
        {
            ViewBag.Creator = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title");
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title");
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim()).OrderBy(a => a.SrNo), "Title", "Title");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,SubTitle,Description,Sector,Themes,Category,FileUrl,Source,Tags,Location,Region,Country,StartDate,EndDate")] Event devevent, IFormFile? FileUrl)
        {
            if (ModelState.IsValid)
            {
                if (FileUrl != null && FileUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(FileUrl.FileName);
                    string mimeType = GetMimeType(FileUrl.FileName);
                    string fileSize = FileUrl.Length.ToString();
                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "EventFiles", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileUrl.CopyToAsync(fileStream);
                    }
                    devevent.FileUrl = "/AdminFiles/EventFiles/" + fileName + fileExtension;
                    devevent.FileMimeType = mimeType;
                    devevent.FileSize = fileSize;
                }
                if (String.IsNullOrEmpty(devevent.FileUrl))
                {
                    devevent.FileUrl = "/images/defaultImage.jpg";
                    devevent.FileMimeType = "image/jpg";
                    devevent.FileSize = "88,651";
                }
                devevent.Id = Guid.NewGuid();
                devevent.Creator = userManager.GetUserId(User);
                devevent.AdminCheck = false;
                devevent.IsInfocus = false;
                devevent.IsGlobal = true;
                devevent.ViewCount = 0;
                db.Events.Add(devevent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devevent.Sector);
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devevent.Themes);
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devevent.Category);
            ViewBag.Creator = new SelectList(db.Users, "Id", "FirstName", devevent.Creator);
            return View(devevent);
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }

        // GET: Events/Edit/5
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin,Admin,Author")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Event? devevent = db.Events.Find(id);
            if (devevent == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devevent.AdminCheck;
            ViewBag.Creator = new SelectList(db.Users, "Id", "FirstName", devevent.Creator);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devevent.Sector);
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devevent.Themes);
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devevent.Category);
            ViewBag.Region = new SelectList(db.Regions.OrderBy(a => a.SrNo), "Title", "Title", devevent.Region);
            return View(devevent);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,SubTitle,Description,Sector,Themes,Category,FileUrl,Source,Tags,Location,Region,Country,StartDate,EndDate,AdminCheck,IsInfocus,IsGlobal,CreatedOn,ModifiedOn,ViewCount,FileSize")] Event devevent, IFormFile? FileUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                if (FileUrlUpdate != null && FileUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(FileUrlUpdate.FileName);
                    string mimeType = GetMimeType(FileUrlUpdate.FileName);
                    string fileSize = FileUrlUpdate.Length.ToString();
                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "EventFiles", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileUrlUpdate.CopyToAsync(fileStream);
                    }
                    devevent.FileUrl = "/AdminFiles/EventFiles/" + fileName + fileExtension;
                    devevent.FileMimeType = mimeType;
                    devevent.FileSize = fileSize;
                }
                if (TempData["AdminCheck"].ToString() == "False" && devevent.AdminCheck == true)
                {
                    devevent.ModifiedOn = DateTime.UtcNow;
                }
                devevent.Creator = userManager.GetUserId(User);
                db.Events.Update(devevent);
                db.SaveChanges();
                CreateLog(devevent.Title + " Event ", devevent.Title + " has been updated", devevent.Creator, userManager.GetUserId(User), "/Home/EventDetails/" + devevent.EventId.ToString());
                //if (TempData["AdminCheck"].ToString() == "False" && devevent.AdminCheck == true)
                //{
                //    TwitterAutoPost(devevent.Title + " " + "http://devdiscourse.com/Home/EventDetails/" + devevent.Id);
                //}
                return RedirectToAction("Index");
            }
            ViewBag.Creator = new SelectList(db.Users, "Id", "FirstName", devevent.Creator);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devevent.Sector);
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devevent.Themes);
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devevent.Category);
            ViewBag.Region = new SelectList(db.Regions.OrderBy(a => a.SrNo), "Title", "Title", devevent.Region);
            return View(devevent);
        }

        // GET: Events/Delete/5
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews? devnews = db.DevNews.Find(id);
            if (devnews == null)
            {
                return NotFound();
            }
            return View(devnews);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DevNews? devnews = db.DevNews.Find(id);
            if (devnews == null)
            {
                return NotFound();
            }
            db.DevNews.Remove(devnews);
            db.SaveChanges();
            CreateLog(devnews.Title + " Event", devnews.Title + " has been deleted", devnews.Creator, userManager.GetUserId(User), "/Home/NewsDetail/" + devnews.NewsId);
            return RedirectToAction("EventList");
        }
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin,Admin,Author")]
        // GET: DevNews
        public ActionResult EventList(DateTime? bfd, DateTime? afd, int? page = 1, string region = "Global Edition", string label = "0", string sector = "0", string category = "0", string country = "", string text = "", string uid = "")
        {
            ViewBag.label = label;
            ViewBag.sector = sector;
            ViewBag.category = category;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            ViewBag.uid = uid;
            ViewBag.loginId = userManager.GetUserId(User);
            var devNews = db.DevNews.Where(a => a.Type == "Event").Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Category = a.Category, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage });
            if (label != "0")
            {
                devNews = devNews.Where(a => a.Label.Contains("," + label + ",") || a.Label.StartsWith("," + label) || a.Label.EndsWith(label + ",") || a.Label.Equals(label));
            }
            if (sector != "0")
            {
                devNews = devNews.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
            }
            if (category != "0")
            {
                devNews = devNews.Where(a => a.Category.Contains("," + category + ",") || a.Category.StartsWith("," + category) || a.Category.EndsWith(category + ",") || a.Category.Equals(category));
            }
            if (region != "Global Edition")
            {
                devNews = devNews.Where(a => a.Region.Contains(region));
            }
            if (!String.IsNullOrEmpty(country))
            {
                devNews = devNews.Where(a => a.Country.Contains(country));
            }
            if (!String.IsNullOrEmpty(text))
            {
                devNews = devNews.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
            }
            if (bfd != null)
            {
                DateTime filterDate = DateTime.Parse(bfd.ToString());
                devNews = devNews.Where(a => a.CreatedOn < filterDate);
            }
            if (afd != null)
            {
                DateTime filterDate2 = DateTime.Parse(afd.ToString());
                devNews = devNews.Where(a => a.CreatedOn > filterDate2);
            }
            if (!String.IsNullOrEmpty(uid))
            {
                devNews = devNews.Where(a => a.Creator == uid);
            }
            return View(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }
        // GET: DevNews/Create
        [Microsoft.AspNet.SignalR.Authorize(Roles = "SuperAdmin,Admin,Author")]
        public ActionResult CreateEvent()
        {
            ViewBag.Creator = new SelectList(db.Users, "Id", "Email");
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title");
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title");
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title");
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim()).OrderBy(a => a.SrNo), "Title", "Title");
            return View();
        }

        // POST: DevNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvent([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Category,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,SourceUrl")] DevNews devNews, IFormFile? ImageUrl, string ChooseImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(ImageUrl.FileName);
                    string mimeType = GetMimeType(ImageUrl.FileName);
                    string fileSize = ImageUrl.Length.ToString();
                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUrl.CopyToAsync(fileStream);
                    }
                    devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                    devNews.FileMimeType = mimeType;
                    devNews.FileSize = fileSize;
                }
                if (String.IsNullOrEmpty(devNews.ImageUrl) && !String.IsNullOrEmpty(devNews.Sector))
                {
                    var sec = devNews.Sector.Split(',')[0];
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg"; // SelectDefaultImage(sec);
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
                    if (findimage != null)
                    {
                        // Saved Image in New Image Gallery
                        string imgcopyright = "";
                        if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        {
                            imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        }
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = findimage.Title,
                            ImageUrl = findimage.FileUrl,
                            ImageCopyright = imgcopyright,
                            Caption = "",
                            FileMimeType = findimage.FileMimeType,
                            FileSize = findimage.FileSize,
                            Sector = findimage.FileFor,
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGalleries.Add(fileobj);
                        db.SaveChanges();
                        // Remove from old gallery
                        db.UserFiles.Remove(findimage);
                        db.SaveChanges();
                    }
                }
                devNews.Id = Guid.NewGuid();
                devNews.AdminCheck = false;
                devNews.IsSponsored = false;
                devNews.EditorPick = false;
                devNews.IsInfocus = false;
                devNews.IsVideo = false;
                devNews.IsStandout = false;
                devNews.IsGlobal = true;
                devNews.IsIndexed = false;
                devNews.Author = "";
                devNews.Type = "Event";
                devNews.SubType = "";
                devNews.ViewCount = 0;
                devNews.LikeCount = 0;
                devNews.WorkStage = "";
                devNews.OriginalSource = devNews.Source;
                devNews.Creator = userManager.GetUserId(User);
                db.DevNews.Add(devNews);
                db.SaveChanges();
                return RedirectToAction("EventList");
            }

            ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim()).OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }
        // GET: DevNews/Edit/5
        public async Task<ActionResult> EditEvent(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews? devNews = db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devNews.AdminCheck;
            ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            string userId = userManager.GetUserId(User);
            var userWork = db.UserWorks.FirstOrDefault(a => a.UserId == userId);

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (userWork != null && userWork.WorkStage != "Image Change")
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        devNews.WorkStage = userWork.UserName + " - " + userWork.WorkStage + "," + userWork.ColorCode;
                        db.DevNews.Update(devNews);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        await context.Clients.All.SendAsync("NewsOpenNotification", devNews.NewsId, userWork.UserName + " - " + userWork.WorkStage, userWork.ColorCode);
                    }
                    catch (Exception ex) { dbContextTransaction.Rollback(); }
                }
            }
            return View(devNews);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEvent([Bind("Id,NewsId,Title,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,PublishedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount,WorkStage")] DevNews devNews, IFormFile? ImageUrlUpdate, string ChooseImage)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                        {
                            var fileName = RandomName(); // method to generate a random name.
                            var fileExtension = Path.GetExtension(ImageUrlUpdate.FileName);
                            string mimeType = GetMimeType(ImageUrlUpdate.FileName);
                            string fileSize = ImageUrlUpdate.Length.ToString();
                            var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await ImageUrlUpdate.CopyToAsync(fileStream);
                            }
                            devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                            devNews.FileMimeType = mimeType;
                            devNews.FileSize = fileSize;
                        }
                        if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
                        {
                            var sec = devNews.Sector.Split(',')[0];
                            devNews.ImageUrl = "/images/sector/all_sectors.jpg"; // SelectDefaultImage(sec);
                            devNews.FileMimeType = "image/jpg";
                            devNews.FileSize = "88,651";
                        }

                        if (!String.IsNullOrEmpty(ChooseImage))
                        {
                            devNews.ImageUrl = ChooseImage;
                            // Find Image in Old Image Gallery
                            var findimage = db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
                            if (findimage != null)
                            {
                                // Saved Image in New Image Gallery
                                string imgcopyright = "";
                                if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                                {
                                    imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                                }
                                ImageGallery fileobj = new ImageGallery()
                                {
                                    Title = findimage.Title,
                                    ImageUrl = findimage.FileUrl,
                                    ImageCopyright = imgcopyright,
                                    Caption = "",
                                    FileMimeType = findimage.FileMimeType,
                                    FileSize = findimage.FileSize,
                                    Sector = findimage.FileFor,
                                    Tags = "",
                                    UseCount = 1,
                                };
                                db.ImageGalleries.Add(fileobj);
                                db.SaveChanges();
                                // Remove from old gallery
                                db.UserFiles.Remove(findimage);
                                db.SaveChanges();
                            }
                        }
                        if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                        {
                            devNews.PublishedOn = DateTime.UtcNow;
                        }
                        devNews.ModifiedOn = DateTime.UtcNow;
                        db.DevNews.Update(devNews);
                        db.SaveChanges();
                        if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                        {
                            string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                            SaveNews(devNews.Id, devNews.Title, description);
                            CreateLog(devNews.Title + " Event ", devNews.Title + " has been updated", devNews.Creator, userManager.GetUserId(User), "/Article/Index/" + devNews.NewsId);
                        }
                        dbContextTransaction.Commit();
                        return RedirectToAction("EventList");
                    }
                    catch (Exception ex) { dbContextTransaction.Rollback(); }
                }
            }
            ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }
        // Send mail to subscriber
        public void SaveNews(Guid id, string title, string description)
        {
            SubscribeNews obj = new SubscribeNews
            {
                ItemId = id,
                Title = title,
                Description = description
            };
            db.SubscribeNews.Add(obj);
            db.SaveChanges();
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
            db.ActivityLogs.Add(logs);
            db.SaveChanges();
        }
        public ActionResult IRU()
        {
            return View();
        }
        public PartialViewResult GetEventNews(int skip = 0, int take = 0)
        {
            var search = db.DevNews.Where(a => a.AdminCheck == true && (a.Category.StartsWith("21,") || a.Category.EndsWith(",21") || a.Category.Contains(",21,") || a.Category == "21")).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(take).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels });
            return PartialView("_getEventNews", search.ToList());
        }
        public ActionResult SSAP()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult WorldRoadCongress()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult GlobalWarming()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult PlasticFreeWorldConference()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult BlockchainSingapore()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult TestconBanglore()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult TestconSingapore()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public PartialViewResult EventNews(string id, int skip = 0, int take = 0)
        {
            var search = db.DevNews.Where(a => a.AdminCheck == true && (a.Category.StartsWith(id + ",") || a.Category.EndsWith("," + id) || a.Category.Contains("," + id + ",") || a.Category == id)).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(take).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels });
            return PartialView("_eventNews", search.ToList());
        }
        public PartialViewResult WRCEventNews(int skip = 0, int take = 0)
        {
            var search = db.DevNews.Where(a => a.AdminCheck == true && (a.Category.StartsWith("21,") || a.Category.EndsWith(",21") || a.Category.Contains(",21,") || a.Category == "21")).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(take).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels });
            return PartialView("_wrcEventNews", search.ToList());
        }
        public ActionResult BusinessManagement()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult WomenHealth()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult GreenUrbanism()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult GoGreen()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult WEC24()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        /// <summary>
        /// FiNext Tech Awards
        /// </summary>
        /// <returns></returns>
        public ActionResult Finext()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        /// <summary>
        /// Southern Africa Power Summit 2019
        /// </summary>
        /// <returns></returns>
        public ActionResult SAPS()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult PowerWeekAfrica()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult AfricaOilWeek()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult AnnualSmartProcurementWorldIndaba()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult SAIPEC()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult AfricaOilAndPower()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult MOC()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult AfricaReEnergyExpo()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult AfricaEnergyIndaba()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult OffShoreIndiaCongress()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult WorldNuclearIndustryCongress()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult WorldGasLngConference()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult OffshoreNorthSeaEuropeCongress()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult DiversityInEnergySummit()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult BigFiveBoardAwards()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult ImpactMobility()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult Napec()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult JapanWindEnergy()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult GlobalndFinancialSkills()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult PrcEurope()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult Finextcon()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult OmanDesignBuild()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult OmanEnergyWater()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult INPSC()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult PowerGen()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult SCEWC()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult OmanEnergyWaterEvent()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult EgyptEducation()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult IOTWorld()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult changeNow()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult EUBCE()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult LNGCongress()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult TOGC()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult FutureEnergyAsia()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult AfricanUtilityWeek()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult LebanonSustainability()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult CybertechGlobal()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult Gogla()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult InternationalTrafficSafety()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult CentralAsiaSummit()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult InterEcoForum()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult WasteManagementExpo()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult CanadaGas()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult SaudiEntertainment()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }
            return View();
        }

        public ActionResult WasteManagementSeriesSummit()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }

            return View();
        }

        public ActionResult AfricanEnergyForum()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie ?? "Global Edition";
            }

            return View();
        }

        static string GetMimeType(string fileName)
        {
            string mimeType;

            // Using FileExtensionContentTypeProvider to get MIME types
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out mimeType))
            {
                // If MIME type is not found, set a default value or handle as needed
                mimeType = "application/octet-stream"; // Default MIME type for unknown files
            }

            return mimeType;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
