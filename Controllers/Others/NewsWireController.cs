using Devdiscourse.Hubs;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ContributorModels;
using Devdiscourse.Models.Others;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Devdiscourse.Data;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;

namespace DevDiscourse.Controllers.Others
{
    public class NewsWireController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _environment;
        public NewsWireController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment _environment)
        {
            this.db = db;
            this.userManager = userManager;
            this._environment = _environment;
        }
        // GET: NewsWire
        public ActionResult Index()
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

        [Authorize]
        public async Task<ActionResult> NewsList(DateTime? bfd, DateTime? afd, int? page = 1, string region = "", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false)
        {
            ViewBag.status = GetAutoAssignStatus();
            ViewBag.label = label;
            ViewBag.sector = sector;
            ViewBag.category = category;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            ViewBag.uid = uid;
            ViewBag.source = source;
            ViewBag.editorPick = editorPick;
            ViewBag.loginId = userManager.GetUserId(User);
            DateTime fifteenDay = DateTime.Today.AddDays(-15);
            // User List
            List<AssignedRoleView> result = new List<AssignedRoleView>();
            List<string> userList = new List<string>();
            var users = await userManager.GetUsersInRoleAsync("PressRelease");
            foreach (var item in users)
            {
                userList.Add(item.Id);
                result.Add(UserList(item.Id, item.FirstName + " " + item.LastName));
            }
            ViewBag.usrList = result;
            IQueryable<NewsListView> devNews;
            if (string.IsNullOrEmpty(text))
            {
                devNews = db.DevNews.Where(a => a.Type == "News" && userList.Contains(a.Creator) && a.CreatedOn > fifteenDay && a.EditorPick == editorPick).Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Category = a.Category, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage, ViewCount = a.ViewCount });
            }
            else
            {
                devNews = db.DevNews.Where(a => a.Type == "News" && userList.Contains(a.Creator) && a.EditorPick == editorPick).Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Category = a.Category, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage, ViewCount = a.ViewCount });
            }
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
            if (region != "Global Edition" && region != "")
            {
                devNews = devNews.Where(a => a.Region.Contains(region));
            }
            if (!string.IsNullOrEmpty(country))
            {
                devNews = devNews.Where(a => a.Country.Contains(country));
            }
            if (!string.IsNullOrEmpty(source))
            {
                devNews = devNews.Where(a => a.Source.Contains(source));
            }
            if (!string.IsNullOrEmpty(text))
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
            if (!string.IsNullOrEmpty(uid))
            {
                devNews = devNews.Where(a => a.Creator == uid);
            }
            return View(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }
        [Authorize(Roles = "SuperAdmin,Admin,Press Release Manager")]
        public ActionResult ContentList(int? page = 1)
        {
            var search = db.NewsWireModels.Where(a => a.Status == ContentStage.Pending).Select(a => new ViewContent { Id = a.Id, Title = a.Title, Description = a.Description, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.IsVideo }).ToList();
            return View(search.OrderByDescending(a => a.ModifiedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize(Roles = "SuperAdmin,Admin,Press Release Manager")]
        public ActionResult PublishContent(int? page = 1)
        {
            var search = (from a in db.NewsWireModels
                          where a.Status == ContentStage.Publish
                          join n in db.DevNews on a.Id equals n.ReferenceId
                          orderby a.ModifiedOn descending
                          select new ViewContent
                          {
                              Id = a.Id,
                              Title = a.Title,
                              Description = a.Description,
                              CreatedOn = a.CreatedOn,
                              ModifiedOn = a.ModifiedOn,
                              Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName,
                              IsVideo = a.IsVideo,
                              NewsId = n.Id,
                              Type = n.Type
                          }).ToPagedList((page ?? 1), 20);
            return View(search);
        }
        [Authorize(Roles = "SuperAdmin,Admin,Press Release Manager")]
        public ActionResult RejectContent(int? page = 1)
        {
            var search = db.NewsWireModels.Where(a => a.Status == ContentStage.Reject).Select(a => new ViewContent { Id = a.Id, Title = a.Title, Description = a.Description, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.IsVideo }).ToList();
            return View(search.OrderByDescending(a => a.ModifiedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize]
        public ActionResult Dashboard()
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
            var userId = userManager.GetUserId(User);
            var search = db.NewsWireModels.Where(a => a.Creator == userId).ToList();
            ViewBag.pending = search.Count(a => a.Status == ContentStage.Pending);
            ViewBag.publish = search.Count(a => a.Status == ContentStage.Publish);
            ViewBag.reject = search.Count(a => a.Status == ContentStage.Reject);
            return View();
        }
        public void UpdateOldNewsWire()
        {
            var search = (from a in db.NewsWireModels
                          where a.Status == ContentStage.Publish
                          join n in db.DevNews on a.Title equals n.Title
                          orderby a.ModifiedOn descending
                          select new
                          {
                              a.Id,
                              NewsId = n.Id
                          }).ToList();
            foreach (var item in search)
            {
                var news = db.DevNews.Find(item.NewsId);
                news.ReferenceId = item.Id;
                db.Entry(news).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        [Authorize]
        public ActionResult Create()
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
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title");
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Sector,City,Region,Country,Source,ImageUrl,VideoUrl,ImageCopyright,PublishedDate,Tags,AuthorImage")] NewsWireModel newswire, IFormFile? ImageUrl, IFormFile? VideoUrl, string? ChooseImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = RandomName();
                    var fileExtension = Path.GetExtension(ImageUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(ImageUrl.FileName);
                    string mimeType = GetMimeType(ImageUrl.FileName);
                    string fileSize = ImageUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await ImageUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);
                        newswire.ImageUrl = blob.Uri.ToString();
                        newswire.FileMimeType = mimeType;
                    }
                }
                if (VideoUrl != null && VideoUrl.Length > 0)
                {
                    var fileName = RandomName();
                    var fileExtension = Path.GetExtension(VideoUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(VideoUrl.FileName);
                    string mimeType = GetMimeType(VideoUrl.FileName);
                    string fileSize = VideoUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await VideoUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobVideoContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);
                        newswire.VideoUrl = blob.Uri.ToString();
                        newswire.IsVideo = true;
                    }
                }
                if (!string.IsNullOrEmpty(ChooseImage))
                {
                    newswire.ImageUrl = ChooseImage;
                }
                newswire.Status = ContentStage.Pending;
                newswire.Creator = userManager.GetUserId(User);
                db.NewsWireModels.Add(newswire);
                db.SaveChanges();
                var searchPressReleaseManager = db.Users.Where(a => a.isPRManager == true).Select(a => new { a.FirstName, a.LastName, a.Email }).ToList();
                if (searchPressReleaseManager.Any())
                {
                    foreach (var item in searchPressReleaseManager)
                    {
                        var callbackUrl = Url.Action("ContentList", "NewsWire", null, protocol: Request.Scheme);
                        EmailController emailObj = new EmailController();
                        string FilePath = Path.Combine(_environment.WebRootPath, "Content", "email-templates", "PressReleaseEmail.html");
                        string Emailbody;
                        using (var sr = new StreamReader(FilePath))
                        {
                            Emailbody = sr.ReadToEnd();
                        }
                        Emailbody = Emailbody.Replace("{0}", item.FirstName + " " + item.LastName);
                        Emailbody = Emailbody.Replace("{1}", callbackUrl);
                        await emailObj.SendEmailAsync(item.Email, Emailbody, "New Press Release on Devdiscourse dashboard");
                        //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        //context.Clients.All.SendPRNewsNotification("New Press Release on Devdiscourse dashboard");
                    }
                }
                return RedirectToAction("Dashboard");
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", newswire.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", newswire.Region);
            return View(newswire);
        }
        public void sendNotice()
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //context.Clients.All.SendPRNewsNotification("New Press Release on Devdiscourse dashboard");
        }
        [Authorize(Roles = "SuperAdmin,Admin,Press Release Manager")]
        public ActionResult Edit(long? id)
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
            if (id == null)
            {
                return BadRequest();
            }
            NewsWireModel? newswire = db.NewsWireModels.Find(id);
            if (newswire == null)
            {
                return NotFound();
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", newswire.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", newswire.Region);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", newswire.Sector);
            return View(newswire);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Sector,City,Region,Country,Source,ImageUrl,ImageCopyright,VideoUrl,IsVideo,PublishedDate,Tags,Creator,CreatedOn,ReasonofReject,AuthorImage")] NewsWireModel newswire, IFormFile? ImageUrlUpdate, IFormFile? VideoUrlUpdate, string? ChooseImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName();
                    var fileExtension = Path.GetExtension(ImageUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(ImageUrlUpdate.FileName);
                    string mimeType = GetMimeType(ImageUrlUpdate.FileName);
                    string fileSize = ImageUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await ImageUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);
                        newswire.ImageUrl = blob.Uri.ToString();
                        newswire.FileMimeType = mimeType;
                    }
                }
                if (VideoUrlUpdate != null && VideoUrlUpdate.Length > 0)
                {
                    var fileName = RandomName();
                    var fileExtension = Path.GetExtension(VideoUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(VideoUrlUpdate.FileName);
                    string mimeType = GetMimeType(VideoUrlUpdate.FileName);
                    string fileSize = VideoUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await VideoUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobVideoContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);
                        newswire.VideoUrl = blob.Uri.ToString();
                        newswire.IsVideo = true;
                    }
                }
                if (!string.IsNullOrEmpty(ChooseImage))
                {
                    newswire.ImageUrl = ChooseImage;
                }
                if (!string.IsNullOrEmpty(Request.Form["publishbtn"]))
                {
                    string type = "";
                    string subtype = "";
                    string newssector = "0";
                    if (newswire.Type == "News")
                    {
                        type = newswire.Type;
                    }
                    else
                    {
                        type = "Blog";
                        subtype = newswire.Type;
                    }
                    if (!string.IsNullOrEmpty(newswire.Sector))
                    {
                        newssector = newswire.Sector;
                    }
                    newswire.Status = ContentStage.Publish;
                    DevNews obj = new DevNews()
                    {
                        Id = Guid.NewGuid(),
                        Title = newswire.Title,
                        SubTitle = newswire.SubTitle,
                        Sector = newssector,
                        Description = newswire.Description,
                        NewsLabels = newswire.NewsLabels,
                        Region = newswire.Region,
                        Country = newswire.Country,
                        SourceUrl = newswire.City,
                        ImageUrl = newswire.ImageUrl,
                        ImageCopyright = newswire.ImageCopyright,
                        FileMimeType = newswire.FileMimeType,
                        Tags = newswire.Tags,
                        Author = newswire.Source,
                        Source = newswire.Source,
                        CreatedOn = newswire.CreatedOn,
                        PublishedOn = DateTime.UtcNow,
                        ReferenceId = newswire.Id,
                        AdminCheck = true,
                        IsGlobal = false,
                        IsSponsored = false,
                        EditorPick = false,
                        IsInfocus = false,
                        IsVideo = false,
                        IsStandout = false,
                        Type = type,
                        SubType = subtype,
                        ViewCount = 0,
                        LikeCount = 0,
                        WorkStage = "",
                        Creator = newswire.Creator,
                        Themes = newswire.AuthorImage
                    };
                    db.DevNews.Add(obj);
                    db.SaveChanges();
                }
                else if (!string.IsNullOrEmpty(Request.Form["rejectbtn"]))
                {
                    newswire.Status = ContentStage.Reject;
                }
                newswire.ModifiedOn = DateTime.UtcNow;
                db.Entry(newswire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ContentList");
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", newswire.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", newswire.Region);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", newswire.Sector);
            return View(newswire);
        }
        public PartialViewResult GetContent(string fl = "")
        {
            string UserId = userManager.GetUserId(User);
            List<NewsWireView> ResultList = new List<NewsWireView>();
            ResultList = db.NewsWireModels.Where(a => a.Creator == UserId).Select(a => new NewsWireView { Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn, Status = a.Status, Country = a.Country, NewsLabels = a.NewsLabels }).ToList();
            if (fl == "pending")
            {
                ResultList = ResultList.Where(a => a.Status == ContentStage.Pending).ToList();
            }
            else if (fl == "publish")
            {
                ResultList = ResultList.Where(a => a.Status == ContentStage.Publish).ToList();
            }
            else if (fl == "reject")
            {
                ResultList = ResultList.Where(a => a.Status == ContentStage.Reject).ToList();
            }
            return PartialView("_getContent", ResultList.OrderByDescending(a => a.ModifiedOn));
        }
        public PartialViewResult GetUserInfo()
        {
            string UserId = userManager.GetUserId(User);
            ViewBag.userInfo = db.Users.Find(UserId);
            // Total Stories Count
            ViewBag.totalStories = db.NewsWireModels.Count(a => a.Creator == UserId);
            return PartialView("_getUserInfo");
        }
        public PartialViewResult Details(long id)
        {
            var newswire = db.NewsWireModels.Find(id);
            return PartialView("_details", newswire);
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
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
            CloudBlobContainer container = blobClient.GetContainerReference("newswire");
            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return container;
        }
        private async Task<CloudBlobContainer> GetCloudBlobVideoContainer()
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            string connectionString = config.GetConnectionString("devdiscourse_AzureStorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("newswirevideo");
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}