using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ContributorModels;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Data;
using Microsoft.AspNetCore.StaticFiles;

namespace DevDiscourse.Controllers.Others
{
    public class ContentCreatorController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _environment;
        public ContentCreatorController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment _environment)
        {
            this.db = db;
            this.userManager = userManager;
            this._environment = _environment;
        }
        // GET: ContentCreator
        [Authorize]
        public ActionResult Index()
        {
            string UserId = userManager.GetUserId(User);
            var user = db.Users.Find(UserId);
            ViewBag.userName = user.FirstName + " " + user.LastName;
            var search = db.Contents.Where(a => a.Creator == UserId).ToList();
            ViewBag.draft = search.Count(a => a.ContentStatus == ContentStage.Draft);
            ViewBag.pending = search.Count(a => a.ContentStatus == ContentStage.Pending);
            ViewBag.reject = search.Count(a => a.ContentStatus == ContentStage.Reject);
            ViewBag.publish = search.Count(a => a.ContentStatus == ContentStage.Publish);
            // Total Views on Published Stories
            var find = db.Earnings.Where(a => a.Creator == UserId);
            if (find.Any())
            {
                ViewBag.totalViews = find.Sum(a => a.ViewCount);
                ViewBag.trending = find.OrderByDescending(a => a.ViewCount).Take(1).FirstOrDefault();
            }
            else
            {
                ViewBag.trending = null;
                ViewBag.totalViews = 0;
            }
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
            DateTime fifteenDay = DateTime.Today.AddDays(-115);
            // User List
            List<AssignedRoleView> result = new List<AssignedRoleView>();
            List<string> userList = new List<string>();
            var users = await userManager.GetUsersInRoleAsync("Contributor");
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
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Contents(int? page = 1, string fl = "")
        {
            var search = db.Contents.Where(a => a.ContentStatus == ContentStage.Pending).Select(a => new ViewContent { Id = a.Id, Title = a.Title, Description = a.Description, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.IsVideo }).ToList();
            if (fl == "article")
            {
                search = search.Where(a => a.IsVideo == false).ToList();
            }
            else if (fl == "video")
            {
                search = search.Where(a => a.IsVideo == true).ToList();
            }
            ViewBag.filter = fl;
            return View(search.OrderByDescending(a => a.ModifiedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> PublishContent()
        {
            List<AssignedRoleView> result = new List<AssignedRoleView>();
            var users = await userManager.GetUsersInRoleAsync("Contributor");
            foreach (var item in users)
            {
                result.Add(UserList(item.Id, item.FirstName + " " + item.LastName));
            }
            ViewBag.data = result;
            return View();
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
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult RejectContent(int? page = 1, string fl = "")
        {
            var search = db.Contents.Where(a => a.ContentStatus == ContentStage.Reject).Select(a => new ViewContent { Id = a.Id, Title = a.Title, Description = a.Description, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.IsVideo }).ToList();
            if (fl == "article")
            {
                search = search.Where(a => a.IsVideo == false).ToList();
            }
            else if (fl == "video")
            {
                search = search.Where(a => a.IsVideo == true).ToList();
            }
            ViewBag.filter = fl;
            return View(search.OrderByDescending(a => a.ModifiedOn).ToPagedList((page ?? 1), 20));
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Content newscontent = db.Contents.Find(id);
            if (newscontent == null)
            {
                return NotFound();
            }
            return View(newscontent);
        }
        [Authorize]
        public ActionResult ContentDetails(long? id)
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
            Content newscontent = db.Contents.Find(id);
            if (newscontent == null)
            {
                return NotFound();
            }
            return View(newscontent);
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
        public async Task<ActionResult> Create([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Source,ImageUrl,VideoUrl,ImageCopyright,Region,Country,Tags")] Content content, IFormFile? ImageUrl, IFormFile? VideoUrl, string? ChooseImage)
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
                        content.ImageUrl = blob.Uri.ToString();
                        content.FileMimeType = mimeType;
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
                        content.VideoUrl = blob.Uri.ToString();
                        content.IsVideo = true;
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    content.ImageUrl = ChooseImage;
                }
                if (string.IsNullOrEmpty(content.VideoUrl))
                {
                    content.VideoUrl = "";
                    content.IsVideo = false;
                }
                if (!string.IsNullOrEmpty(Request.Form["createBtn"]))
                {
                    content.ContentStatus = ContentStage.Pending;
                }
                else if (!string.IsNullOrEmpty(Request.Form["draftBtn"]))
                {
                    content.ContentStatus = ContentStage.Draft;
                }
                content.Creator = userManager.GetUserId(User);
                content.ReasonofReject = "";
                db.Contents.Add(content);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", content.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", content.Region);
            return View(content);
        }
        [Authorize]
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
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return NotFound();
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", content.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", content.Region);
            return View(content);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Source,ImageUrl,ImageCopyright,VideoUrl,IsVideo,Region,Country,Tags,ReasonofReject,Creator,CreatedOn")] Content content, IFormFile? ImageUrlUpdate, IFormFile? VideoUrlUpdate, string? ChooseImage)
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
                        content.ImageUrl = blob.Uri.ToString();
                        content.FileMimeType = mimeType;
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
                        content.VideoUrl = blob.Uri.ToString();
                        content.IsVideo = true;
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    content.ImageUrl = ChooseImage;
                }
                content.ContentStatus = ContentStage.Pending;
                content.ModifiedOn = DateTime.UtcNow;
                db.Entry(content).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", content.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", content.Region);
            return View(content);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Approval(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return NotFound();
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", content.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", content.Region);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", content.Sector);
            return View(content);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approval([Bind("Id,Title,SubTitle,Description,Type,NewsLabels,Source,ImageUrl,ImageCopyright,VideoUrl,IsVideo,Region,Country,Sector,Tags,ReasonofReject,Creator,CreatedOn")] Content content, IFormFile? ImageUrlUpdate, string? ChooseImage)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
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
                                content.ImageUrl = blob.Uri.ToString();
                                content.FileMimeType = mimeType;
                            }
                        }
                        if (!String.IsNullOrEmpty(ChooseImage))
                        {
                            content.ImageUrl = ChooseImage;
                        }
                        if (string.IsNullOrEmpty(Request.Form["publishbtn"]))
                        {
                            string type = "";
                            string subtype = "";
                            string newssector = "0";
                            if (content.Type == "News")
                            {
                                type = content.Type;
                            }
                            else
                            {
                                type = "Blog";
                                subtype = content.Type;
                            }
                            if (!String.IsNullOrEmpty(content.Sector))
                            {
                                newssector = content.Sector;
                            }
                            content.ContentStatus = ContentStage.Publish;
                            DevNews obj = new DevNews()
                            {
                                Id = Guid.NewGuid(),
                                Title = content.Title,
                                SubTitle = content.SubTitle,
                                Sector = newssector,
                                Description = content.Description,
                                NewsLabels = content.NewsLabels,
                                Region = content.Region,
                                Country = content.Country,
                                ImageUrl = content.ImageUrl,
                                ImageCopyright = content.ImageCopyright,
                                FileMimeType = content.FileMimeType,
                                Tags = content.Tags,
                                Source = content.Source,
                                CreatedOn = content.CreatedOn,
                                PublishedOn = DateTime.UtcNow,
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
                                Creator = content.Creator,
                            };
                            db.DevNews.Add(obj);
                            db.SaveChanges();
                            // Create Earning
                            Earnings earn = new Earnings()
                            {
                                NewsId = content.Id,
                                ViewCount = 0,
                                Amount = 0,
                                Creator = content.Creator,
                            };
                            db.Earnings.Add(earn);
                            db.SaveChanges();
                        }
                        else if (!string.IsNullOrEmpty(Request.Form["rejectbtn"]))
                        {
                            content.ContentStatus = ContentStage.Reject;
                        }
                        content.ModifiedOn = DateTime.UtcNow;
                        db.Entry(content).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("Contents");
                    }
                    catch (Exception ex) { dbContextTransaction.Rollback(); }
                }
            }
            ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", content.NewsLabels);
            ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", content.Region);
            ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", content.Sector);
            return View(content);
        }
        [Authorize]
        public ActionResult Drafted(int? page = 1)
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
            string UserId = userManager.GetUserId(User);
            var search = db.Contents.Where(a => a.ContentStatus == ContentStage.Draft && a.Creator == UserId).Select(a => new ContentView { Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl, Country = a.Country, NewsLabels = a.NewsLabels, CreatedOn = a.CreatedOn }).ToList();
            // Total Draft Stories Count
            ViewBag.totalDraft = search.Count();
            return View(search.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize]
        public ActionResult Pending(int? page = 1)
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
            string UserId = userManager.GetUserId(User);
            var search = db.Contents.Where(a => a.ContentStatus == ContentStage.Pending && a.Creator == UserId).Select(a => new ContentView { Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl, Country = a.Country, NewsLabels = a.NewsLabels, CreatedOn = a.CreatedOn }).ToList();
            // Total Pending Stories Count
            ViewBag.totalPending = search.Count();
            return View(search.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize]
        public ActionResult Published(int? page = 1, string fl = "")
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
            List<ContentView> resultList = new List<ContentView>();
            string UserId = userManager.GetUserId(User);
            // For Total Published and Total Views on Devdiscourse
            var find = db.Earnings.Where(a => a.Creator == UserId).Select(a => new ContentView { Id = a.NewsId, Title = a.Contents.Title, ImageUrl = a.Contents.ImageUrl, Country = a.Contents.Country, NewsLabels = a.Contents.NewsLabels, CreatedOn = a.CreatedOn, ViewCount = a.ViewCount }).ToList();
            if (fl == "")
            {
                resultList = find.OrderByDescending(a => a.CreatedOn).ToList();
            }
            else
            {
                resultList = find.OrderByDescending(a => a.ViewCount).ToList();
            }
            ViewBag.filter = fl;
            return View(resultList.ToPagedList((page ?? 1), 20));
        }
        [Authorize]
        public ActionResult Rejected(int? page = 1)
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
            string UserId = userManager.GetUserId(User);
            var search = db.Contents.Where(a => a.ContentStatus == ContentStage.Reject && a.Creator == UserId).Select(a => new ContentView { Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl, Country = a.Country, NewsLabels = a.NewsLabels, CreatedOn = a.CreatedOn, ReasonofReject = a.ReasonofReject }).ToList();
            // Total Rejected Strories Count
            ViewBag.totalReject = search.Count();
            return View(search.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize]
        public ActionResult Earnings(int? page = 1, int mm = 0)
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
            ViewBag.month = mm;
            string UserId = userManager.GetUserId(User);
            // Total Views on Published Stories
            var find = db.Earnings.Where(a => a.Creator == UserId);
            if (find.Any())
            {
                ViewBag.trending = find.OrderByDescending(a => a.ViewCount).Take(1).FirstOrDefault();
            }
            else
            {
                ViewBag.trending = null;
            }
            if (mm != 0)
            {
                find = find.Where(a => a.CreatedOn.Month == mm);
            }
            return View(find.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
        }
        [Authorize]
        public ActionResult Settings()
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
            var userid = userManager.GetUserId(User);
            var search = db.Payments.FirstOrDefault(a => a.Creator == userid);
            if (search != null)
            {
                return RedirectToAction("PaymentDetails");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings([Bind("Id,Name,Address,City,State,Country,Pincode,Contact,EmailId,AccountHolderName,BankName,BankAddress,BankCity,BankState,BankCountry,BankPincode,AccountNo,AccountType,IFSCCode,PanCardNo,Creator,CreatedOn")] Payment payment, string UserCountry)
        {
            if (ModelState.IsValid)
            {
                payment.ModifiedOn = DateTime.UtcNow;
                db.Payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserCountry = UserCountry;
            return View(payment);
        }

        [Authorize]
        public ActionResult PaymentDetails()
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
            var userid = userManager.GetUserId(User);
            var search = db.Payments.FirstOrDefault(a => a.Creator == userid);
            return View(search);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentDetails([Bind("Id,Name,Address,City,State,Country,Pincode,Contact,EmailId,AccountHolderName,BankName,BankAddress,BankCity,BankState,BankCountry,BankPincode,AccountNo,AccountType,IFSCCode,PanCardNo,Creator,CreatedOn")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.ModifiedOn = DateTime.UtcNow;
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payment);
        }
        [Authorize]
        public ActionResult History()
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
            string UserId = userManager.GetUserId(User);
            var search = db.Earnings.Where(a => a.Creator == UserId);
            if (search.Any())
            {
                ViewBag.totalEarnings = search.Sum(a => a.Amount);
            }
            else
            {
                ViewBag.totalEarnings = 0;
            }
            return View();
        }
        public PartialViewResult GetUserInfo(string UserId = "")
        {
            if (UserId == "")
            {
                UserId = userManager.GetUserId(User);
            }
            ViewBag.userInfo = db.Users.Find(UserId);
            // Total Stories Count
            ViewBag.totalStories = db.Contents.Count(a => a.Creator == UserId);
            // Total Views on Devdiscourse
            var find = db.Earnings.Where(a => a.Creator == UserId).ToList();
            ViewBag.totalViews = find.Sum(a => a.ViewCount);
            return PartialView("_getUserInfo");
        }
        public PartialViewResult GetPublishedContent(string userId = "", string fl = "")
        {
            List<PublishView> ResultList = new List<PublishView>();
            if (!String.IsNullOrEmpty(userId))
            {
                ResultList = db.Earnings.Where(a => a.Creator == userId).Select(a => new PublishView { Id = a.Id, NewsId = a.NewsId, Title = a.Contents.Title, ViewCount = a.ViewCount, Amount = a.Amount, CreatedOn = a.CreatedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.Contents.IsVideo }).ToList();
            }
            else
            {
                ResultList = db.Earnings.Select(a => new PublishView { Id = a.Id, NewsId = a.NewsId, Title = a.Contents.Title, ViewCount = a.ViewCount, Amount = a.Amount, CreatedOn = a.CreatedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.Contents.IsVideo }).ToList();
            }
            if (fl == "article")
            {
                ResultList = ResultList.Where(a => a.IsVideo == false).ToList();
            }
            else if (fl == "video")
            {
                ResultList = ResultList.Where(a => a.IsVideo == true).ToList();
            }
            return PartialView("_getPublishedContent", ResultList.OrderByDescending(a => a.CreatedOn));
        }
        public PartialViewResult GetBankDetails(string userId)
        {
            Payment? search = db.Payments.FirstOrDefault(a => a.Creator == userId);
            if (search == null)
            {
                return PartialView("_getBankDetails", new Payment());
            }
            return PartialView("_getBankDetails", search);
        }
        public void CreateEarning(long id)
        {
            var search = db.DevNews.FirstOrDefault(a => a.NewsId == id);
            var find = db.Contents.FirstOrDefault(a => a.Title == search.Title && a.Creator == search.Creator);
            Earnings earn = new Earnings()
            {
                NewsId = find.Id,
                ViewCount = search.ViewCount,
                Amount = 0,
                Creator = find.Creator,
            };
            db.Earnings.Add(earn);
            db.SaveChanges();
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
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
            CloudBlobContainer container = blobClient.GetContainerReference("contributor");
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
            CloudBlobContainer container = blobClient.GetContainerReference("contributorvideo");
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