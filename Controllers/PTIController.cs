using Devdiscourse.Hubs;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using X.PagedList;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.StaticFiles;

namespace DevDiscourse.Controllers
{
    public class PTIController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> context;
        public PTIController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager, IWebHostEnvironment _environment, IHubContext<ChatHub> context)
        {
            this._db = _db;
            this.userManager = userManager;
            this._environment = _environment;
            this.context = context;
        }
        public string GetShiftUser()
        {
            var users = _db.Users.Where(a => a.isActive == true).OrderByDescending(o => o.CreatedOn).Select(s => s.Id).ToArray();
            int counter = 0;

            var filePath = Path.Combine(_environment.WebRootPath, "Content", "counter.txt");
            if (System.IO.File.Exists(filePath))
            {
                string noOfVisitors = System.IO.File.ReadAllText(filePath);
                counter = int.TryParse(noOfVisitors, out int parsedCounter) ? parsedCounter : 0;
            }
            else
            {
                System.IO.File.Create(filePath);
            }

            counter++;
            System.IO.File.WriteAllText(filePath, counter.ToString());

            if (users.Length > 0)
            {
                var userLength = users.Length;
                var userIndex = counter % userLength;
                counter = userIndex;
                System.IO.File.WriteAllText(filePath, counter.ToString());
                return users[userIndex].ToString();
            }
            return "No User";
        }
        public bool GetAutoAssignStatus()
        {
            bool status = false;
            var filePath = Path.Combine(_environment.WebRootPath, "Content", "status.txt");
            if (System.IO.File.Exists(filePath))
            {
                string statusText = System.IO.File.ReadAllText(filePath);
                status = bool.Parse(statusText);
            }
            return status;
        }
        [HttpPost]
        public async Task<string> ReutersNews(SourceNewsView obj)
        {
            string description = obj.Description;
            bool AutoAssign = GetAutoAssignStatus();
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    DevNews newsObj = new DevNews
                    {
                        Title = obj.Title,
                        SubTitle = "",
                        Description = description,
                        Sector = "0",
                        Region = "Global Edition",
                        Source = "Reuters",
                        OriginalSource = "Reuters",
                        Tags = obj.Tags,
                        AdminCheck = bool.Parse(obj.AdminCheck),
                        IsGlobal = true,
                        ImageUrl = "/images/newstheme.jpg",
                        FileMimeType = "image/jpg",
                        FileSize = "88,651",
                        IsSponsored = false,
                        EditorPick = false,
                        IsInfocus = false,
                        IsVideo = false,
                        IsStandout = false,
                        IsIndexed = AutoAssign,
                        Author = "Reuters",
                        Type = "News",
                        ViewCount = 0,
                        LikeCount = 0,
                        WorkStage = "",
                        SourceUrl = obj.Origin,
                        Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
                    };
                    _db.DevNews.Add(newsObj);
                    _db.SaveChanges();
                    //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
                    // Assign To User
                    var newsId = newsObj.NewsId;
                    var userId = GetShiftUser();
                    if (userId != "No User" && AutoAssign == true)
                    {
                        AssignedNews(userId, newsId);
                    }
                    dbContextTransaction.Commit();
                    return "success";
                }
                catch (Exception ex) { dbContextTransaction.Rollback(); return ex.Message; }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public string UpdateKeywords(UpdateObj obj)
        {
            long NewsId = obj.NewsId;
            string Keywords = obj.Keywords;
            var news = _db.DevNews.FirstOrDefault(a => a.NewsId == NewsId);
            if (news == null) { return "Not Found"; }
            news.Tags = Keywords;
            news.AdminCheck = true;
            _db.Entry(news).State = EntityState.Modified;
            _db.SaveChanges();
            return "Success";
        }

        [AllowAnonymous]
        public string AdminCheck(long NewsId)
        {
            var news = _db.DevNews.FirstOrDefault(a => a.NewsId == NewsId);
            if (news == null) { return "Not Found"; }
            news.AdminCheck = true;
            _db.Entry(news).State = EntityState.Modified;
            _db.SaveChanges();
            return "Success";
        }
        [HttpPost]
        public async Task<string> IANSNews(SourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            bool AutoAssign = GetAutoAssignStatus();
            if (obj.Category.ToUpper() == "INTERNATIONAL")
            {
                region = "Global Edition";
            }
            else
            {
                region = "South Asia";
                country = "India";
            }
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    DevNews newsObj = new DevNews
                    {
                        Title = obj.Title,
                        SubTitle = "",
                        Description = description,
                        Sector = "0",
                        Region = region,
                        Country = country,
                        Source = "IANS",
                        OriginalSource = "IANS",
                        Tags = obj.Tags,
                        AdminCheck = bool.Parse(obj.AdminCheck),
                        IsGlobal = false,
                        ImageUrl = "/images/newstheme.jpg",
                        FileMimeType = "image/jpg",
                        FileSize = "88,651",
                        IsSponsored = false,
                        EditorPick = false,
                        IsInfocus = false,
                        IsVideo = false,
                        IsStandout = false,
                        IsIndexed = AutoAssign,
                        Author = "Press Trust of India",
                        Type = "News",
                        ViewCount = 0,
                        LikeCount = 0,
                        WorkStage = "",
                        SourceUrl = obj.Origin,
                        Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
                    };
                    _db.DevNews.Add(newsObj);
                    _db.SaveChanges();
                    //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
                    // Assign To User
                    var newsId = newsObj.NewsId;
                    var userId = GetShiftUser();

                    if (userId != "No User" && AutoAssign == true)
                    {
                        AssignedNews(userId, newsId);
                    }
                    dbContextTransaction.Commit();
                    return "success";
                }
                catch (Exception ex) { dbContextTransaction.Rollback(); return ex.Message; }
            }
        }
        [HttpPost]
        public async Task<JsonResult> PTINews(SourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            bool AutoAssign = GetAutoAssignStatus();
            if (obj.Category.ToUpper() == "INTERNATIONAL")
            {
                region = "Global Edition";
            }
            else
            {
                region = "South Asia";
                country = "India";
            }
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    DevNews newsObj = new DevNews
                    {
                        Title = obj.Title,
                        SubTitle = "",
                        Description = description,
                        Sector = "0",
                        Region = region,
                        Country = country,
                        Source = "PTI",
                        OriginalSource = "PTI",
                        Tags = obj.Tags,
                        AdminCheck = bool.Parse(obj.AdminCheck),
                        IsGlobal = false,
                        ImageUrl = "/images/newstheme.jpg",
                        FileMimeType = "image/jpg",
                        FileSize = "88,651",
                        IsSponsored = false,
                        EditorPick = false,
                        IsInfocus = false,
                        IsVideo = false,
                        IsStandout = false,
                        IsIndexed = AutoAssign,
                        Author = "Press Trust of India",
                        Type = "News",
                        ViewCount = 0,
                        LikeCount = 0,
                        WorkStage = "",
                        SourceUrl = obj.Origin,
                        Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
                    };
                    _db.DevNews.Add(newsObj);
                    _db.SaveChanges();
                    //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
                    // Assign To User
                    var newsId = newsObj.NewsId;
                    var userId = GetShiftUser();
                    if (userId != "No User" && AutoAssign == true)
                    {
                        AssignedNews(userId, newsId);
                    }
                    dbContextTransaction.Commit();
                    return Json("Success");
                }
                catch (Exception ex) { dbContextTransaction.Rollback(); return Json(ex.Message); }
            }
        }
        public void AssignedNews(string userId, long NewsId)
        {
            AssignNews obj = new AssignNews()
            {
                UserId = userId,
                NewsId = NewsId,
                CreatedOn = DateTime.UtcNow,
                Creator = userManager.GetUserId(User),
            };
            _db.AssignNews.Add(obj);
            _db.SaveChanges();
        }
        // GET: PTI
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index(DateTime? bfd, DateTime? afd, int? page = 1, string region = "Global Edition", string sector = "0", string category = "0", string country = "", string text = "", string uid = "")
        {
            ViewBag.sector = sector;
            ViewBag.category = category;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            ViewBag.uid = uid;
            ViewBag.loginId = userManager.GetUserId(User);
            var devNews = _db.DevNews.Where(a => a.Type == "News" && a.Source == "PTI").Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Sector = a.Sector, Category = a.Category, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage });
            if (sector != "0")
            {
                devNews = devNews.Where(a => a.Sector.Contains(sector));
            }
            if (category != "0")
            {
                devNews = devNews.Where(a => a.Category.Contains(category));
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

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Upfront")]
        public ActionResult AssignedNews(int? page = 1, string region = "", string label = "0", string sector = "0", string country = "", string source = "", string text = "")
        {
            ViewBag.label = label;
            ViewBag.sector = sector;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.source = source;
            string userId = userManager.GetUserId(User);
            ViewBag.loginId = userId;
            DateTime fiveDay = DateTime.Today.AddDays(-5);
            DateTime oneday = DateTime.UtcNow.AddDays(-1);
            var assignNews = _db.AssignNews.Where(a => a.UserId == userId && a.CreatedOn > oneday).Select(a => a.NewsId).ToList();
            if (!assignNews.Any())
            {
                return View(new List<NewsListView>().ToPagedList((page ?? 1), 10));
            }
            // Fetching DevNews data into memory matching the filtering conditions
            var filteredDevNews = _db.DevNews
                .Where(a => a.Type == "News" && a.CreatedOn > fiveDay && a.Title.Contains(text))
                .ToList(); // Fetch filtered data into memory

            // Performing join and projection in memory
            var devNews = filteredDevNews
                .Join(
                    assignNews,
                    a => a.NewsId,
                    s => s,
                    (a, s) => new NewsListView
                    {
                        Id = a.Id,
                        NewsId = a.NewsId,
                        Label = a.NewsLabels,
                        Sector = a.Sector,
                        Category = a.Category,
                        Title = a.Title,
                        SubTitle = a.SubTitle,
                        Creator = a.Creator,
                        CreatorName = a.ApplicationUsers?.FirstName + " " + a.ApplicationUsers?.LastName,
                        Region = a.Region,
                        Country = a.Country,
                        ImageUrl = a.ImageUrl,
                        Source = a.Source,
                        SourceUrl = a.SourceUrl,
                        AdminCheck = a.AdminCheck,
                        IsInfocus = a.IsInfocus,
                        EditorPick = a.EditorPick,
                        IsGlobal = a.IsGlobal,
                        IsFifa = a.IsStandout,
                        IsIndex = a.IsIndexed,
                        CreatedOn = a.CreatedOn,
                        WorkStage = a.WorkStage,
                        ViewCount = a.ViewCount
                    })
                .AsQueryable();

            text = text ?? "";

            if (label != "0")
            {
                devNews = devNews.Where(a => a.Label.Contains("," + label + ",") || a.Label.StartsWith("," + label) || a.Label.EndsWith(label + ",") || a.Label.Equals(label));
            }
            if (sector != "0")
            {
                devNews = devNews.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
            }
            if (region != "Global Edition" && region != "")
            {
                devNews = devNews.Where(a => a.Region.Contains(region));
            }
            if (!String.IsNullOrEmpty(country))
            {
                devNews = devNews.Where(a => a.Country.Contains(country));
            }
            if (!String.IsNullOrEmpty(source))
            {
                devNews = devNews.Where(a => a.Source.Contains(source));
            }
            if (!String.IsNullOrEmpty(text))
            {
                devNews = devNews.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
            }
            return View(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Upfront")]
        public ActionResult AssignedNewsList(int? page = 1, string userId = "", string region = "", string sector = "0", string country = "", string source = "", string text = "")
        {
            ViewBag.userId = userId;
            ViewBag.sector = sector;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.source = source;
            //DateTime today = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime fiveDay = DateTime.Today.AddDays(-7);
            DateTime oneday = DateTime.Today.AddDays(-3);
            var assignNews = _db.AssignNews.Where(a => a.CreatedOn > oneday).Select(a => a.NewsId).ToList();
            if (!String.IsNullOrEmpty(userId))
            {
                assignNews = _db.AssignNews.Where(a => a.UserId == userId && a.CreatedOn > oneday).Select(a => a.NewsId).ToList();
            }
            if (!assignNews.Any())
            {
                return View(new List<NewsListView>().ToPagedList((page ?? 1), 10));
            }
            // Fetching DevNews data into memory matching the filtering conditions
            var filteredDevNews = _db.DevNews
                 .Where(a => a.Type == "News" && a.CreatedOn > fiveDay && a.Title.Contains(text))
                .ToList(); // Fetch filtered data into memory

            // Performing join and projection in memory
            var devNews = filteredDevNews
                .Join(
                    assignNews,
                    a => a.NewsId,
                    s => s,
                    (a, s) => new NewsListView
                    {
                        Id = a.Id,
                        NewsId = a.NewsId,
                        Label = a.NewsLabels,
                        Sector = a.Sector,
                        Category = a.Category,
                        Title = a.Title,
                        SubTitle = a.SubTitle,
                        Creator = a.Creator,
                        CreatorName = a.ApplicationUsers?.FirstName + " " + a.ApplicationUsers?.LastName,
                        Region = a.Region,
                        Country = a.Country,
                        ImageUrl = a.ImageUrl,
                        Source = a.Source,
                        SourceUrl = a.SourceUrl,
                        AdminCheck = a.AdminCheck,
                        IsInfocus = a.IsInfocus,
                        EditorPick = a.EditorPick,
                        IsGlobal = a.IsGlobal,
                        IsFifa = a.IsStandout,
                        IsIndex = a.IsIndexed,
                        CreatedOn = a.CreatedOn,
                        WorkStage = a.WorkStage,
                        ViewCount = a.ViewCount
                    })
                .AsQueryable();

            text = text ?? "";

            if (sector != "0")
            {
                devNews = devNews.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
            }
            if (!string.IsNullOrEmpty(region))
            {
                devNews = devNews.Where(a => a.Region.Contains(region));
            }
            if (!String.IsNullOrEmpty(source))
            {
                devNews = devNews.Where(a => a.Source.Contains(source));
            }
            return View(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        public AssignedRoleView UserList(string id, string user)
        {
            AssignedRoleView obj = new AssignedRoleView
            {
                Id = id,
                User = user,
            };
            return obj;
        }
        // GET: DevNews/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            return View(devNews);
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    using (var streamReader = new StreamReader(file.OpenReadStream()))
                    {
                        string text = streamReader.ReadToEnd();
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(text);
                        string json = JsonConvert.SerializeXmlNode(doc);
                        var response = JsonConvert.DeserializeObject<dynamic>(json);
                        string title = (string)response["rss"]["channel"]["item"]["title"];
                        string description = (string)response["rss"]["channel"]["item"]["description"]["#cdata-section"];
                        var paragraphArray = description.Split('\n');
                        StringBuilder sb = new StringBuilder();
                        foreach (var line in paragraphArray)
                        {
                            sb.Append("<p>" + line.Trim() + "</p>");
                        }
                        var newDesc = sb.ToString();
                        using (var dbContextTransaction = _db.Database.BeginTransaction())
                        {
                            try
                            {
                                DevNews obj = new DevNews
                                {
                                    Title = title,
                                    SubTitle = title,
                                    Description = newDesc,
                                    Sector = "1",
                                    Region = "South Asia",
                                    Country = "India",
                                    Source = "PTI",
                                    AdminCheck = true,
                                    IsGlobal = false,
                                    ImageUrl = "/images/defaultImage.jpg",
                                    FileMimeType = "image/jpg",
                                    FileSize = "88,651",
                                    IsSponsored = false,
                                    EditorPick = false,
                                    IsInfocus = false,
                                    IsVideo = false,
                                    IsStandout = false,
                                    IsIndexed = false,
                                    Author = "Devdiscourse News Desk",
                                    Type = "News",
                                    ViewCount = 0,
                                    LikeCount = 0,
                                    WorkStage = "",
                                    Creator = userManager.GetUserId(User),
                                };
                                _db.DevNews.Add(obj);
                                _db.SaveChanges();
                                dbContextTransaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                dbContextTransaction.Rollback();
                                ViewBag.Message = "File upload failed!!";
                                return View();
                            }
                        }
                    }
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
        // GET: DevNews/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devNews.AdminCheck;
            ViewBag.Creator = new SelectList(_db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(_db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(_db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            string userId = userManager.GetUserId(User);
            var userWork = _db.UserWorks.FirstOrDefault(a => a.UserId == userId);

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (userWork != null && userWork.WorkStage != "Image Change")
            {
                devNews.WorkStage = userWork.UserName + " - " + userWork.WorkStage + "," + userWork.ColorCode;
                _db.Entry(devNews).State = EntityState.Modified;
                _db.Entry(devNews).Property(x => x.NewsId).IsModified = false;
                _db.SaveChanges();
                await context.Clients.All.SendAsync("NewsOpenNotification", devNews.NewsId, userWork.UserName + " - " + userWork.WorkStage, userWork.ColorCode);
            }
            return View(devNews);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount,WorkStage")] DevNews devNews, IFormFile? ImageUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                        {
                            var fileName = RandomName();
                            var fileExtension = Path.GetExtension(ImageUrlUpdate.FileName);
                            var actName = Path.GetFileNameWithoutExtension(ImageUrlUpdate.FileName);
                            var mimeType = GetMimeType(ImageUrlUpdate.FileName);
                            var fileSize = ImageUrlUpdate.Length.ToString();

                            CloudBlobContainer blobContainer;
                            CloudBlockBlob blob;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await ImageUrlUpdate.CopyToAsync(ms);
                                ms.Position = 0;

                                blobContainer = await GetCloudBlobContainer();
                                blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                                await blob.UploadFromStreamAsync(ms);

                                devNews.ImageUrl = blob.Uri.ToString();
                                devNews.FileMimeType = mimeType;
                                devNews.FileSize = fileSize;
                            }
                        }
                        devNews.ModifiedOn = DateTime.UtcNow;
                        _db.DevNews.Update(devNews);
                        _db.Entry(devNews).Property(n => n.NewsId).IsModified = false;
                        _db.SaveChanges();
                        if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                        {
                            string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                            if (description.Length > 1000)
                            {
                                description = description.Substring(0, 1000) + "...";
                            }
                            SaveNews(devNews.Id, devNews.Title, description);
                            CreateLog(devNews.Title + " News ", devNews.Title + " has been Updated", devNews.Creator, userManager.GetUserId(User), "/Article/Index/" + devNews.NewsId);
                        }
                        dbContextTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex) { dbContextTransaction.Rollback(); }
                }
            }
            ViewBag.Creator = new SelectList(_db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(_db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(_db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }
        public async Task<ActionResult> EditNews(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devNews.AdminCheck;
            ViewBag.Creator = new SelectList(_db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(_db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(_db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            string userId = userManager.GetUserId(User);
            var userWork = _db.UserWorks.FirstOrDefault(a => a.UserId == userId);

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (userWork != null && userWork.WorkStage != "Image Change")
            {
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        devNews.WorkStage = userWork.UserName + " - " + userWork.WorkStage + "," + userWork.ColorCode;
                        _db.DevNews.Update(devNews);
                        _db.Entry(devNews).Property(n => n.NewsId).IsModified = false;
                        _db.SaveChanges();
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
        public async Task<ActionResult> EditNews([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount,WorkStage")] DevNews devNews, IFormFile? ImageUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                        {
                            var fileName = RandomName();
                            var fileExtension = Path.GetExtension(ImageUrlUpdate.FileName);
                            var actName = Path.GetFileNameWithoutExtension(ImageUrlUpdate.FileName);
                            var mimeType = GetMimeType(ImageUrlUpdate.FileName);
                            var fileSize = ImageUrlUpdate.Length.ToString();

                            CloudBlobContainer blobContainer;
                            CloudBlockBlob blob;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await ImageUrlUpdate.CopyToAsync(ms);
                                ms.Position = 0;

                                blobContainer = await GetCloudBlobContainer();
                                blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                                await blob.UploadFromStreamAsync(ms);

                                devNews.ImageUrl = blob.Uri.ToString();
                                devNews.FileMimeType = mimeType;
                                devNews.FileSize = fileSize;
                            }
                        }
                        devNews.ModifiedOn = DateTime.UtcNow;
                        _db.DevNews.Update(devNews);
                        _db.Entry(devNews).Property(n => n.NewsId).IsModified = false;
                        _db.SaveChanges();
                        if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                        {
                            string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                            if (description.Length > 1000)
                            {
                                description = description.Substring(0, 1000) + "...";
                            }
                            SaveNews(devNews.Id, devNews.Title, description);
                            CreateLog(devNews.Title + " News ", devNews.Title + " has been Updated", devNews.Creator, userManager.GetUserId(User), "/Article/Index/" + devNews.NewsId);
                        }
                        dbContextTransaction.Commit();
                        return RedirectToAction("AssignedNews");
                    }
                    catch (Exception ex) { dbContextTransaction.Rollback(); }
                }
            }
            ViewBag.Creator = new SelectList(_db.Users, "Id", "Email", devNews.Creator);
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Themes = new SelectList(_db.DevThemes, "Id", "Title", devNews.Themes);
            ViewBag.Category = new SelectList(_db.Categories, "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }
        // GET: DevNews/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            return View(devNews);
        }

        // POST: DevNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            _db.DevNews.Remove(devNews);
            _db.SaveChanges();
            CreateLog(devNews.Title + "" + devNews.Type, devNews.Title + " has been deleted", devNews.Creator, userManager.GetUserId(User), "/Home/NewsDetail/" + devNews.NewsId);
            return RedirectToAction("Index");
        }
        [OutputCache(Duration = 60)]
        public ActionResult News(int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            DateTime dayBefore = DateTime.Today.AddDays(-30);
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > dayBefore && a.Type == "News" && a.Source == "PTI").Select(a => new PublisherView { ModifiedOn = a.CreatedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn);
            return View(search.ToPagedList((page ?? 1), 20));
        }
        public JsonResult PTIAmpNews(int? moreItemsPageIndex, string __amp_source_origin)
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.Type == "News" && a.Source == "PTI").Select(a => new
            {
                a.CreatedOn,
                a.Title,
                Url = "/article/" + a.NewsId.ToString(),
                defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                ImageUrl = $"/Experiment/Img?imageUrl={a.ImageUrl}",
                a.Country
            }).OrderByDescending(a => a.CreatedOn);
            if (!string.IsNullOrEmpty(__amp_source_origin))
            {
                // Inside your controller action or middleware where you have access to HttpContext
                HttpContext.Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
            }
            int pageSize = 10;
            int pageNumber = (moreItemsPageIndex ?? 1);
            var resultData = search.ToPagedList(pageNumber, pageSize);
            return Json(new { items = resultData, hasMorePages = resultData.Any() });
        }
        [OutputCache(Duration = 60)]
        public ActionResult DevdiscourseNews(int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.Type == "News" && a.Source == "Devdiscourse News Desk").Select(a => new PublisherView { ModifiedOn = a.ModifiedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).AsNoTracking();
            return View(search.ToPagedList((page ?? 1), 20));
        }
        public JsonResult DevdiscourseAmpNews(int? moreItemsPageIndex, string __amp_source_origin)
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.Type == "News" && a.Source == "DEVDISCOURSE NEWS DESK").Select(a => new
            {
                a.ModifiedOn,
                a.Title,
                Url = "/Article/" + a.NewsId.ToString(),
                a.Country,
                defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                ImageUrl = $"/Experiment/Img?imageUrl={a.ImageUrl}"
            }).OrderByDescending(a => a.ModifiedOn);
            if (!string.IsNullOrEmpty(__amp_source_origin))
            {
                // Inside your controller action or middleware where you have access to HttpContext
                HttpContext.Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
            }
            int pageSize = 10;
            int pageNumber = (moreItemsPageIndex ?? 1);
            var resultData = search.ToPagedList(pageNumber, pageSize);
            return Json(new { items = resultData, hasMorePages = resultData.Any() });
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
                ActivityDate = DateTime.UtcNow,
                IsRead = false
            };
            _db.ActivityLogs.Add(logs);
            _db.SaveChanges();
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
            _db.SubscribeNews.Add(obj);
            _db.SaveChanges();
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
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
        private async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .Build();
            string connectionString = config.GetConnectionString("devdiscourse_AzureStorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("devnews");
            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return container;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
        //public JsonResult GetPTINewsItems(string __amp_source_origin, int? moreItemsPageIndex)
        //{
        //    var search = _db.DevNews.Where(a => a.AdminCheck == true && a.Type == "News" && a.Source=="PTI").Select(a => new { a.Region, a.IsGlobal, a.Sector, a.ModifiedOn, a.Title, Url = "/Article/" + a.NewsId.ToString(), a.ImageUrl, a.Country });
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);

        //    }
        //    var result = search.OrderByDescending(m => m.ModifiedOn).Select(b => new { b.Title, b.Url, b.ImageUrl, b.Country });
        //    int pageSize = 10;
        //    int pageNumber = (moreItemsPageIndex ?? 1);
        //    var resultData = result.ToPagedList(pageNumber, pageSize);
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}
    }
}