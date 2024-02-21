using System.Net;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using System.Text.RegularExpressions;
using Devdiscourse.Models.ViewModel;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Devdiscourse.Hubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Devdiscourse.Models.Others;
using Newtonsoft.Json;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nancy.Json;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.Main
{
    public class DevNewsController : BaseController, IDisposable
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> context;

        public DevNewsController(UserManager<ApplicationUser> userManager,
            ApplicationDbContext _db,
            IWebHostEnvironment webHostEnvironment,
            IHubContext<ChatHub> context)
        {
            this.userManager = userManager;
            this._db = _db;
            _webHostEnvironment = webHostEnvironment;
            this.context = context;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        // GET: DevNews
        public ActionResult Index(DateTime? bfd, DateTime? afd, int? page = 1, string region = "", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false)
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
            DateTime fifteenDay = DateTime.Today.AddDays(-5);
            DateTime threeMonth = DateTime.Today.AddDays(-28);
            IQueryable<NewsListView> devNews = (from a in _db.DevNews
                                                where a.Type == "News"
                                                select new NewsListView
                                                {
                                                    Id = a.Id,
                                                    NewsId = a.NewsId,
                                                    Label = a.NewsLabels,
                                                    Sector = a.Sector,
                                                    Category = a.Category,
                                                    Title = a.Title,
                                                    SubTitle = a.SubTitle,
                                                    Creator = a.Creator,
                                                    CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName,
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
                                                    ViewCount = a.ViewCount,
                                                    ModifiedOn = a.ModifiedOn
                                                });
            if (string.IsNullOrEmpty(text))
            {
                devNews = devNews.Where(a => a.CreatedOn > threeMonth);
            }
            else
            {
                devNews = devNews.Where(a => a.Title.Contains(text) && a.CreatedOn > fifteenDay);
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
        public bool GetAutoAssignStatus()
        {
            bool status = false;
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwrootPath, "Content", "status.txt");
            if (System.IO.File.Exists(filePath))
            {
                string statusText = System.IO.File.ReadAllText(filePath);
                status = bool.Parse(statusText);
            }
            return status;
        }
        public JsonResult UpdateAutoAssignStatus(bool status)
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwrootPath, "Content", "status.txt");
            System.IO.File.WriteAllText(filePath, status.ToString());
            var data = new
            {
                message = "success"
            };
            return Json(data);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Blog(DateTime? bfd, DateTime? afd, int? page = 1, string region = "", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false, string type = "")
        {
            ViewBag.label = label;
            ViewBag.sector = sector;
            ViewBag.category = category;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            ViewBag.text = text;
            ViewBag.uid = uid;
            ViewBag.source = source;
            ViewBag.type = type;
            ViewBag.editorPick = editorPick;
            ViewBag.loginId = userManager.GetUserId(User);
            var devNews = _db.DevNews.Where(a => a.Type == "Blog").Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Title = a.Title, SubTitle = a.SubType, Creator = a.Creator, CreatorName = a.Author, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage, ViewCount = a.ViewCount });
            if (editorPick == true)
            {
                devNews = devNews.Where(a => a.EditorPick == true);
            }
            if (!string.IsNullOrEmpty(type))
            {
                devNews = devNews.Where(a => a.SubTitle == type);
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
            devNews = devNews.Where(a => a.CreatedOn > DateTime.Today.AddDays(-28));
            return View(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        // GET: DevNews/Details/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            return View(devNews);
        }
        // GET: DevNews/Create
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Create(string ret = "")
        {
            ViewBag.ret = ret;
            TempData["ret"] = ret;
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title");
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title");
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
            return View();
        }
        // POST: DevNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,AlternateHeadline,SubTitle,Description,NewsLabels,Category,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,SourceUrl,EditorPick,ReferenceId,IsSponsored")] DevNews devNews, IFormFile? ImageUrl, string? ChooseImage, List<string> FileTitle, List<string> FilePath, List<string> MimeType, List<string> FileSize, List<string> FileCaption, List<string> FileThumbUrl, List<string> FileDuration)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(fileStream);
                        }
                        devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        string imgcopyright = "";
                        if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        {
                            imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        }
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = devNews.ImageUrl,
                            ImageCopyright = imgcopyright,
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = devNews.Sector,
                            Tags = "",
                            UseCount = 1,
                        };
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();
                    }
                }
                else if (String.IsNullOrEmpty(devNews.ImageUrl) && !String.IsNullOrEmpty(devNews.Sector))
                {
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg";
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                devNews.Id = Guid.NewGuid();
                devNews.AdminCheck = false;
                devNews.IsSponsored = false;
                devNews.IsInfocus = false;
                devNews.IsVideo = false;
                devNews.IsStandout = false;
                devNews.IsGlobal = false;
                devNews.IsIndexed = false;
                devNews.Author = "";
                devNews.Type = "News";
                devNews.SubType = "";
                devNews.ViewCount = 0;
                devNews.LikeCount = 0;
                devNews.WorkStage = "";
                devNews.OriginalSource = devNews.Source;
                devNews.Creator = userManager.GetUserId(User);
                //devNews.NewsId = 111;
                _db.DevNews.Add(devNews);
                _db.SaveChanges();
                //var sectorId = devNews.Sector.Split(',').Select(id => int.Parse(id.Trim()).ToString()).ToList();
                //foreach(var item in sectorId)
                //{
                //    var sectorMapping = new SectorMapping
                //    {
                //        SectorId = int.Parse(item),
                //        NewsId = devNews.Id
                //    };
                //    _db.SectorMappings.Add(sectorMapping);
                //    _db.SaveChanges();
                //}
                var edition = ML_Edition(devNews.Description);
                List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
                if (edition.Any())
                {
                    foreach (var item in edition)
                    {
                        var regObj = new RegionNewsRanking()
                        {
                            RegionId = item.RegionId,
                            NewsId = devNews.Id,
                            Ranking = item.Ranking
                        };
                        newsRankingList.Add(regObj);
                    };
                    _db.RegionNewsRankings.AddRange(newsRankingList);
                    _db.SaveChanges();
                }

                if (FileTitle != null)
                {
                    List<UserNewsFile> userFileList = new List<UserNewsFile>();
                    foreach (var item in FileTitle)
                    {
                        var index = FileTitle.IndexOf(item);
                        var filepath = FilePath[index];
                        var mimetype = MimeType[index];
                        var fileSize = FileSize[index];
                        var caption = FileCaption[index];
                        var thumbUrl = FileThumbUrl[index];
                        var fDuration = FileDuration[index];
                        var newsid = devNews.Id;
                        UserNewsFile obj = new UserNewsFile();
                        obj.Title = item;
                        obj.FilePath = filepath;
                        obj.FileMimeType = mimetype;
                        obj.FileSize = fileSize;
                        obj.FileCaption = caption;
                        obj.FileThumbUrl = thumbUrl;
                        obj.Duration = fDuration;
                        obj.NewsId = newsid;
                        userFileList.Add(obj);
                    }
                    if (userFileList.Count > 0)
                    {
                        _db.UserNewsFiles.AddRange(userFileList);
                        _db.SaveChanges();
                    }
                }

                if (TempData["ret"].ToString() == "auth")
                {
                    return RedirectToAction("NewsList");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ret = TempData["ret"].ToString();
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }
        public List<NewsRankingViewModel> ML_Edition(string content)
        {
            List<NewsRankingViewModel> newsRankingList = new List<NewsRankingViewModel>();
            try
            {
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    reqparm.Add("content", content);
                    byte[] responsebytes = client.UploadValues("https://devdiscourseml.azurewebsites.net/prediction", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    newsRankingList = JsonConvert.DeserializeObject<List<NewsRankingViewModel>>(responsebody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return newsRankingList;
        }
        // Blog Create
        // GET: DevNews/Create
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult CreateBlog()
        {
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title");
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title");
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
            return View();
        }
        // POST: DevNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBlog([Bind("Id,Title,SubTitle,Description,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,SourceUrl,EditorPick,ReferenceId,IsSponsored")] DevNews devNews, IFormFile? ImageUrl, string? ChooseImage, List<string> FileTitle, List<string> FilePath, List<string> MimeType, List<string> FileSize, List<string> FileCaption, List<string> FileThumbUrl, List<string> FileDuration)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await ImageUrl.CopyToAsync(fileStream);
                        //}
                        //devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.ImageUrl = blob.Uri.ToString();
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        string imgcopyright = "";
                        if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        {
                            imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        }
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = devNews.ImageUrl,
                            ImageCopyright = imgcopyright,
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = devNews.Sector,
                            Tags = "",
                            UseCount = 1,
                        };
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                    }
                }

                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();
                    }
                }
                else if (String.IsNullOrEmpty(devNews.ImageUrl) && !String.IsNullOrEmpty(devNews.Sector))
                {
                    var sec = devNews.Sector.Split(',')[0];
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg"; // SelectDefaultImage(sec);
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                devNews.Id = Guid.NewGuid();
                devNews.AdminCheck = false;
                devNews.IsSponsored = false;
                devNews.IsInfocus = false;
                devNews.IsVideo = false;
                devNews.IsStandout = false;
                devNews.IsGlobal = false;
                devNews.IsIndexed = false;
                devNews.Type = "Blog";
                devNews.ViewCount = 0;
                devNews.LikeCount = 0;
                devNews.WorkStage = "";
                devNews.OriginalSource = devNews.Source;
                devNews.Creator = userManager.GetUserId(User);
                //devNews.NewsId = 111;
                _db.DevNews.Add(devNews);
                _db.SaveChanges();
                var edition = ML_Edition(devNews.Description);
                List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
                if (edition.Any())
                {
                    foreach (var item in edition)
                    {
                        var regObj = new RegionNewsRanking()
                        {
                            RegionId = item.RegionId,
                            NewsId = devNews.Id,
                            Ranking = item.Ranking
                        };
                        newsRankingList.Add(regObj);
                    };
                    _db.RegionNewsRankings.AddRange(newsRankingList);
                    _db.SaveChanges();
                }
                if (FileTitle != null)
                {
                    List<UserNewsFile> userFileList = new List<UserNewsFile>();
                    foreach (var item in FileTitle)
                    {
                        var index = FileTitle.IndexOf(item);
                        var filepath = FilePath[index];
                        var mimetype = MimeType[index];
                        var fileSize = FileSize[index];
                        var caption = FileCaption[index];
                        var thumbUrl = FileThumbUrl[index];
                        var fDuration = FileDuration[index];
                        var newsid = devNews.Id;
                        UserNewsFile obj = new UserNewsFile();
                        obj.Title = item;
                        obj.FilePath = filepath;
                        obj.FileMimeType = mimetype;
                        obj.FileSize = fileSize;
                        obj.NewsId = newsid;
                        obj.FileCaption = caption;
                        obj.FileThumbUrl = thumbUrl;
                        obj.Duration = fDuration;
                        userFileList.Add(obj);
                    }
                    if (userFileList.Count > 0)
                    {
                        _db.UserNewsFiles.AddRange(userFileList);
                        _db.SaveChanges();
                    }
                }
                return RedirectToAction("Blog");
            }
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title");
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }

        // GET: DevNews/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public async Task<ActionResult> Edit(Guid? id, string ret)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            ViewBag.ret = ret;
            TempData["AdminCheck"] = devNews.AdminCheck;
            TempData["IsStandout"] = devNews.IsStandout;
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            string userId = userManager.GetUserId(User);
            var user = _db.Users.Find(userId);
            var userWork = _db.UserWorks.FirstOrDefault(a => a.UserId == userId);

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (userWork != null && userWork.WorkStage != "Image Change")
            {
                devNews.WorkStage = userWork.UserName + " - " + userWork.WorkStage + "," + userWork.ColorCode;
                _db.DevNews.Update(devNews);
                _db.Entry(devNews).Property(n => n.NewsId).IsModified = false;
                _db.SaveChanges();
              await  context.Clients.All.SendAsync("NewsOpenNotification",devNews.NewsId, userWork.UserName + " - " + userWork.WorkStage, userWork.ColorCode);
            }
            else
            {
                UserWork obj = new UserWork
                {
                    WorkStage = "Editing",
                    ColorCode = "bg-red",
                    UserName = user.FirstName + " " + user.LastName,
                    UserId = userId,
                    CreatedOn = DateTime.UtcNow
                };
                _db.UserWorks.Add(obj);
                _db.SaveChanges();
              await  context.Clients.All.SendAsync("NewsOpenNotification",devNews.NewsId, user.FirstName + " " + user.LastName + " - " + "Editing", "bg-red");
            }
            return View(devNews);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,NewsId,Title,AlternateHeadline,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,PublishedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount,WorkStage,ReferenceId")] DevNews devNews, IFormFile? ImageUrlUpdate, string? ChooseImage, string? ret, List<string> FileTitle, List<string> FilePath, List<string> MimeType, List<string> FileVideoSize, List<string> FileDelete, List<string> FileCaption, List<string> FileThumbUrl, List<string> FileDuration)
        {
            ViewBag.ret = ret;
            if (ModelState.IsValid)
            {
                if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrlUpdate.CopyToAsync(fileStream);
                        }
                        devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        string imgcopyright = "";
                        if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        {
                            imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        }
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = devNews.ImageUrl,
                            ImageCopyright = imgcopyright,
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = devNews.Sector,
                            Tags = "",
                            UseCount = 1,
                        };
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();

                    }
                }
                else if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
                {
                    var sec = devNews.Sector.Split(',')[0];
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg"; // SelectDefaultImage(sec);                    
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                {
                    devNews.PublishedOn = DateTime.UtcNow;
                }
                //devNews.PublishedOn = DateTime.UtcNow;
                //devNews.CreatedOn = DateTime.UtcNow;
                devNews.ModifiedOn = DateTime.UtcNow;
                _db.DevNews.Update(devNews);
                _db.Entry(devNews).Property(x => x.ViewCount).IsModified = false;
                _db.Entry(devNews).Property(x => x.NewsId).IsModified = false;
                _db.SaveChanges();

                if (FileTitle != null)
                {
                    foreach (var item in FileTitle)
                    {
                        var index = FileTitle.IndexOf(item);
                        var filepath = FilePath[index];
                        var mimetype = MimeType[index];
                        var fileSize = FileVideoSize[index];
                        var caption = FileCaption[index];
                        var thumbUrl = FileThumbUrl[index];
                        var fDuration = FileDuration[index];
                        var newsid = devNews.Id;

                        UserNewsFile obj = new UserNewsFile();
                        obj.Title = item;
                        obj.FilePath = filepath;
                        obj.FileMimeType = mimetype;
                        obj.FileSize = fileSize;
                        obj.FileCaption = caption;
                        obj.FileThumbUrl = thumbUrl;
                        obj.Duration = fDuration;
                        obj.NewsId = newsid;
                        _db.UserNewsFiles.Add(obj);
                        _db.SaveChanges();
                    }
                }


                if (FileDelete != null)
                {
                    foreach (var item in FileDelete)
                    {
                        var guidItem = Guid.Parse(item);
                        UserNewsFile obj = _db.UserNewsFiles.Find(guidItem);
                        _db.UserNewsFiles.Remove(obj);
                        _db.SaveChanges();
                    }
                }


                if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    if (description.Length > 1000)
                    {
                        description = description.Substring(0, 1000) + "...";
                    }
                    SaveNews(devNews.Id, devNews.Title, description);
                    //TwitterAutoPost(devNews.Title + " " + "http://devdiscourse.com/Home/Detail/" + devNews.Id);
                    CreateLog(devNews.Title + " News ", devNews.Title + " has been Updated", devNews.Creator, userManager.GetUserId(User), "/Article/Index/" + devNews.NewsId);
                }
                if (TempData["IsStandout"].ToString() == "False" && devNews.IsStandout == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    if (description.Length > 200)
                    {
                        description = description.Substring(0, 200) + "...";
                    }
                    await SendMessage(devNews.Title, description, devNews.NewsId.ToString(), devNews.ImageUrl);
                }
                if (devNews.Category == "35")
                {
                    Livediscourse livediscourse = new Livediscourse();
                    livediscourse.Title = devNews.Title;
                    livediscourse.Description = "";
                    livediscourse.CreatedOn = DateTime.UtcNow;
                    livediscourse.ModifiedOn = DateTime.UtcNow;
                    livediscourse.Sector = devNews.Sector;
                    livediscourse.ParentId = 1337;
                    livediscourse.AdminCheck = false;
                    livediscourse.PublishedOn = DateTime.UtcNow;
                    livediscourse.Region = devNews.Region;
                    livediscourse.Country = devNews.Country;
                    livediscourse.ViewCount = 0;
                    livediscourse.LikeCount = 0;
                    livediscourse.ParentStoryLink = "/article/" + devNews.NewsLabels + "/" + devNews.GenerateSecondSlug();
                    livediscourse.Author = devNews.Author;
                    var dataTags = (devNews.Tags ?? "").Split(',').Where(item => item != "").Select(s => s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase);
                    livediscourse.Tags = string.Join(", ", dataTags.Take(5)).Trim();
                    livediscourse.AdminCheck = true;
                    livediscourse.Creator = devNews.Creator;
                    _db.Livediscourses.Add(livediscourse);
                    _db.SaveChanges();
                    if (livediscourse.ParentId != 0)
                    {
                        var parent = _db.Livediscourses.Find(livediscourse.ParentId);
                        if (parent != null)
                        {
                            parent.ModifiedOn = DateTime.UtcNow;
                            _db.Livediscourses.Update(parent);
                            _db.SaveChanges();
                        }
                    }
                }
                if (devNews.Category == "37")
                {
                    Livediscourse livediscourse = new Livediscourse();
                    livediscourse.Title = devNews.Title;
                    livediscourse.Description = devNews.Description;
                    livediscourse.ImageUrl = devNews.ImageUrl;
                    livediscourse.ImageCaption = devNews.ImageCaption;
                    livediscourse.ImageCopyright = devNews.ImageCopyright;
                    livediscourse.CreatedOn = DateTime.UtcNow;
                    livediscourse.ModifiedOn = DateTime.UtcNow;
                    livediscourse.Sector = devNews.Sector;
                    livediscourse.ParentId = 1048;
                    livediscourse.AdminCheck = false;
                    livediscourse.PublishedOn = DateTime.UtcNow;
                    livediscourse.Region = devNews.Region;
                    livediscourse.Country = devNews.Country;
                    livediscourse.ViewCount = 0;
                    livediscourse.LikeCount = 0;
                    livediscourse.ParentStoryLink = "/article/" + devNews.NewsLabels + "/" + devNews.GenerateSecondSlug();
                    livediscourse.Author = devNews.Author;
                    var dataTags = (devNews.Tags ?? "").Split(',').Where(item => item != "").Select(s => s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase);
                    livediscourse.Tags = string.Join(", ", dataTags.Take(5)).Trim();
                    livediscourse.AdminCheck = true;
                    livediscourse.Creator = devNews.Creator;
                    _db.Livediscourses.Add(livediscourse);
                    _db.SaveChanges();
                    if (livediscourse.ParentId != 0)
                    {
                        var parent = _db.Livediscourses.Find(livediscourse.ParentId);
                        if (parent != null)
                        {
                            parent.ModifiedOn = DateTime.UtcNow;
                            _db.Livediscourses.Update(parent);
                            _db.SaveChanges();
                        }
                    }
                }
                if (ret == "assign")
                {
                    return RedirectToAction("AssignedNews", "PTI");
                }
                else if (ret == "newswire")
                {
                    return RedirectToAction("PublishContent", "NewsWire");
                }
                else if (ret == "auth")
                {
                    return RedirectToAction("NewsList");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }

        //GET: DevNews/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public async Task<ActionResult> EditBlog(Guid? id, string ret)
        {
            ViewBag.ret = ret;
            if (id == null)
            {
                return StatusCode(400);
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devNews.AdminCheck;
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.SubType);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            string userId = userManager.GetUserId(User);
            var userWork = _db.UserWorks.FirstOrDefault(a => a.UserId == userId);

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (userWork != null && userWork.WorkStage != "Image Change")
            {
                devNews.WorkStage = userWork.UserName + " - " + userWork.WorkStage + "," + userWork.ColorCode;
                _db.DevNews.Update(devNews);
                _db.Entry(devNews).Property(n => n.NewsId).IsModified = false;
                _db.SaveChanges();
              await  context.Clients.All.SendAsync("NewsOpenNotification",devNews.NewsId, userWork.UserName + " - " + userWork.WorkStage, userWork.ColorCode);
            }
            return View(devNews);
        }

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBlog([Bind("Id,NewsId,Title,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,PublishedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount,WorkStage,ReferenceId")] DevNews devNews, IFormFile? ImageUrlUpdate, string? ChooseImage, string? ret, List<string> FileTitle, List<string> FilePath, List<string> MimeType, List<string> FileVideoSize, List<string> FileDelete, List<string> FileCaption, List<string> FileThumbUrl, List<string> FileDuration)
        {
            ViewBag.ret = ret;
            if (ModelState.IsValid)
            {
                if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await ImageUrl.CopyToAsync(fileStream);
                        //}
                        //devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.ImageUrl = blob.Uri.ToString();
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        string imgcopyright = "";
                        if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        {
                            imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        }
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = devNews.ImageUrl,
                            ImageCopyright = imgcopyright,
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = devNews.Sector,
                            Tags = "",
                            UseCount = 1,
                        };
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();
                    }
                }
                else if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
                {
                    var sec = devNews.Sector.Split(',')[0];
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg"; // SelectDefaultImage(sec);
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                {
                    devNews.PublishedOn = DateTime.UtcNow;
                }
                devNews.ModifiedOn = DateTime.UtcNow;
                _db.DevNews.Update(devNews);
                _db.Entry(devNews).Property(x => x.ViewCount).IsModified = false;
                _db.Entry(devNews).Property(x => x.NewsId).IsModified = false;
                _db.SaveChanges();
                if (FileTitle != null)
                {
                    List<UserNewsFile> userFileList = new List<UserNewsFile>();
                    foreach (var item in FileTitle)
                    {
                        var index = FileTitle.IndexOf(item);
                        var filepath = FilePath[index];
                        var mimetype = MimeType[index];
                        var fileSize = FileVideoSize[index];
                        var caption = FileCaption[index];
                        var thumbUrl = FileThumbUrl[index];
                        var fDuration = FileDuration[index];
                        var newsid = devNews.Id;
                        UserNewsFile obj = new UserNewsFile();
                        obj.Title = item;
                        obj.FilePath = filepath;
                        obj.FileMimeType = mimetype;
                        obj.FileSize = fileSize;
                        obj.NewsId = newsid;
                        obj.FileCaption = caption;
                        obj.FileThumbUrl = thumbUrl;
                        obj.Duration = fDuration;
                        userFileList.Add(obj);
                    }
                    if (userFileList.Count > 0)
                    {
                        _db.UserNewsFiles.AddRange(userFileList);
                        _db.SaveChanges();
                    }
                }
                if (FileDelete != null)
                {
                    foreach (var item in FileDelete)
                    {
                        var guidItem = Guid.Parse(item);
                        UserNewsFile obj = _db.UserNewsFiles.Find(guidItem);
                        _db.UserNewsFiles.Remove(obj);
                        _db.SaveChanges();
                    }
                }
                if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    SaveNews(devNews.Id, devNews.Title, description);
                    CreateLog(devNews.Title + " Blog ", devNews.Title + " has been updated", devNews.Creator, userManager.GetUserId(User), "/Article/Index/" + devNews.NewsId);
                }
                if (ret == "newswire")
                {
                    return RedirectToAction("PublishContent", "NewsWire");
                }
                return RedirectToAction("Blog");
            }
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }

        // GET: DevNews/Clone/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Clone(Guid? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devNews.AdminCheck;
            TempData["IsStandout"] = devNews.IsStandout;
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            return View(devNews);
        }

        // POST: DevNews/Clone/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Clone([Bind("Id,NewsId,Title,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount")] DevNews devNews, IFormFile? ImageUrlUpdate, string? ChooseImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await ImageUrl.CopyToAsync(fileStream);
                        //}
                        //devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.ImageUrl = blob.Uri.ToString();
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        //string imgcopyright = "";
                        //if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        //{
                        //    imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        //}
                        //ImageGallery fileobj = new ImageGallery()
                        //{
                        //    Title = actName,
                        //    ImageUrl = devNews.ImageUrl,
                        //    ImageCopyright = imgcopyright,
                        //    Caption = "",
                        //    FileMimeType = mimeType,
                        //    FileSize = fileSize,
                        //    Sector = devNews.Sector,
                        //    Tags = "",
                        //    UseCount = 1,
                        //};
                        //_db.ImageGalleries.Add(fileobj);
                        //_db.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();
                    }
                }
                if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
                {
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg";
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                devNews.Id = Guid.NewGuid();
                devNews.IsIndexed = false;
                devNews.ViewCount = 0;
                devNews.LikeCount = 0;
                devNews.Creator = userManager.GetUserId(User);
                devNews.CreatedOn = DateTime.UtcNow;
                devNews.ModifiedOn = DateTime.UtcNow;
                devNews.PublishedOn = DateTime.UtcNow;
                //devNews.NewsId = 111;
                _db.DevNews.Add(devNews);
                _db.SaveChanges();
                if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    if (description.Length > 1000)
                    {
                        description = description.Substring(0, 1000) + "...";
                    }
                }
                if (TempData["IsStandout"].ToString() == "False" && devNews.IsStandout == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    if (description.Length > 200)
                    {
                        description = description.Substring(0, 200) + "...";
                    }
                    await SendMessage(devNews.Title, description, devNews.NewsId.ToString(), devNews.ImageUrl);
                }
                return RedirectToAction("Index");
            }
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }

        // GET: DevNews/Clone/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult BlogClone(Guid? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            TempData["AdminCheck"] = devNews.AdminCheck;
            TempData["IsStandout"] = devNews.IsStandout;
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

            return View(devNews);
        }

        // POST: DevNews/BlogClone/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BlogClone([Bind("Id,NewsId,Title,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount")] DevNews devNews, IFormFile? ImageUrlUpdate, string? ChooseImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrlUpdate != null && ImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await ImageUrl.CopyToAsync(fileStream);
                        //}
                        //devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.ImageUrl = blob.Uri.ToString();
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        //string imgcopyright = "";
                        //if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        //{
                        //    imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        //}
                        //ImageGallery fileobj = new ImageGallery()
                        //{
                        //    Title = actName,
                        //    ImageUrl = devNews.ImageUrl,
                        //    ImageCopyright = imgcopyright,
                        //    Caption = "",
                        //    FileMimeType = mimeType,
                        //    FileSize = fileSize,
                        //    Sector = devNews.Sector,
                        //    Tags = "",
                        //    UseCount = 1,
                        //};
                        //_db.ImageGalleries.Add(fileobj);
                        //_db.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();
                    }
                }
                if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
                {
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg";
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                devNews.Id = Guid.NewGuid();
                devNews.IsIndexed = false;
                devNews.ViewCount = 0;
                devNews.LikeCount = 0;
                devNews.CreatedOn = DateTime.UtcNow;
                devNews.ModifiedOn = DateTime.UtcNow;
                devNews.PublishedOn = DateTime.UtcNow;
                //devNews.NewsId = 111;
                _db.DevNews.Add(devNews);
                _db.SaveChanges();
                if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    if (description.Length > 1000)
                    {
                        description = description.Substring(0, 1000) + "...";
                    }
                }
                if (TempData["IsStandout"].ToString() == "False" && devNews.IsStandout == true)
                {
                    string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
                    if (description.Length > 200)
                    {
                        description = description.Substring(0, 200) + "...";
                    }
                    await SendMessage(devNews.Title, description, devNews.NewsId.ToString(), devNews.ImageUrl);
                }
                return RedirectToAction("Blog");
            }
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }

        // GET: DevNews/BreakingNews
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult BreakingNews()
        {
            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title");
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title");
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
            return View();
        }

        // POST: DevNews/BreakingNews
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BreakingNews([Bind("Id,Title,SubTitle,Description,NewsLabels,Category,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,SourceUrl,EditorPick")] DevNews devNews, IFormFile? ImageUrl, string? ChooseImage)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
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

                        //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "AdminFiles", "NewsImages", fileName + fileExtension);

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await ImageUrl.CopyToAsync(fileStream);
                        //}
                        //devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
                        devNews.ImageUrl = blob.Uri.ToString();
                        devNews.FileMimeType = mimeType;
                        devNews.FileSize = fileSize;
                        // Saved Image in Image Gallery
                        string imgcopyright = "";
                        if (!string.IsNullOrEmpty(devNews.ImageCopyright))
                        {
                            imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
                        }
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = devNews.ImageUrl,
                            ImageCopyright = imgcopyright,
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = devNews.Sector,
                            Tags = "",
                            UseCount = 1,
                        };
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                    }
                }
                if (!String.IsNullOrEmpty(ChooseImage))
                {
                    devNews.ImageUrl = ChooseImage;
                    // Find Image in Old Image Gallery
                    var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
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
                        _db.ImageGalleries.Add(fileobj);
                        _db.SaveChanges();
                        // Remove from old gallery
                        _db.UserFiles.Remove(findimage);
                        _db.SaveChanges();
                    }
                }
                else if (String.IsNullOrEmpty(devNews.ImageUrl) && !String.IsNullOrEmpty(devNews.Sector))
                {
                    devNews.ImageUrl = "/images/sector/all_sectors.jpg";
                    devNews.FileMimeType = "image/jpg";
                    devNews.FileSize = "88,651";
                }
                devNews.Id = Guid.NewGuid();
                devNews.AdminCheck = true;
                devNews.IsSponsored = false;
                devNews.IsInfocus = false;
                devNews.IsVideo = false;
                devNews.IsStandout = false;
                devNews.IsGlobal = false;
                devNews.IsIndexed = false;
                devNews.Author = "";
                devNews.Type = "News";
                devNews.SubType = "";
                devNews.ViewCount = 0;
                devNews.LikeCount = 0;
                devNews.OriginalSource = devNews.Source;
                devNews.Creator = userManager.GetUserId(User);
                //devNews.NewsId = 111;
                _db.DevNews.Add(devNews);
                _db.SaveChanges();
                var edition = ML_Edition(devNews.Description);
                List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
                if (edition.Any())
                {
                    foreach (var item in edition)
                    {
                        var regObj = new RegionNewsRanking()
                        {
                            RegionId = item.RegionId,
                            NewsId = devNews.Id,
                            Ranking = item.Ranking
                        };
                        newsRankingList.Add(regObj);
                    };
                    _db.RegionNewsRankings.AddRange(newsRankingList);
                    _db.SaveChanges();
                }
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
            ViewBag.Category = new SelectList(_db.Categories.Where(s => s.IsActive == true).OrderByDescending(o => o.SrNo), "Id", "Title", devNews.Category);
            ViewBag.NewsLabels = new SelectList(_db.Labels, "Slug", "Title", devNews.NewsLabels);
            ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
            return View(devNews);
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
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
        // GET: DevNews/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            DevNews? devNews = _db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }
            return View(devNews);
        }
        //POST: DevNews/Delete/5
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
        public JsonResult UpdateIndexed(Guid id)
        {
            var search = _db.DevNews.Find(id);
            search.IsIndexed = true;
            _db.DevNews.Update(search);
            _db.SaveChanges();
            var data = new
            {
                message = "Success"
            };
            return Json(data);
        }    
        public JsonResult DataSearch(string sector)
        {
            var sectorList = sector.Split(',').ToList();
            var search1 = from w in _db.DevNews
                          from l in sectorList
                          where w.Sector.StartsWith(l + ",") || w.Sector.Contains("," + l + ",") || w.Sector.EndsWith("," + l) || w.Sector == l
                          select new { w.Sector };
            var data = new
            {
                message = search1.Distinct().Count()
            };
            return Json(data);
        }
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        // GET: DevNews
        public ActionResult Analytic()
        {
            return View();
        }
        public PartialViewResult GetAnalytic(DateTime? bfd, DateTime? afd, int skip = 0, string reg = "Global Edition", string sec = "0", string country = "", string text = "")
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true).OrderByDescending(a => a.ModifiedOn).Select(a => new AnalyticView { Id = a.NewsId, Title = a.Title, Region = a.Region, Country = a.Country, Sector = a.Sector, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn });
            if (sec != "0")
            {
                search = search.Where(a => a.Sector.StartsWith(sec + ",") || a.Sector.Contains("," + sec + ",") || a.Sector.EndsWith("," + sec) || a.Sector == sec);
            }
            if (reg != "Global Edition")
            {
                search = search.Where(a => a.Region.Contains(reg));
            }
            if (!String.IsNullOrEmpty(country))
            {
                search = search.Where(a => a.Country.Contains(country));
            }
            if (!String.IsNullOrEmpty(text))
            {
                search = search.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
            }
            if (bfd != null)
            {
                DateTime filterDate = DateTime.Parse(bfd.ToString());
                search = search.Where(a => a.CreatedOn < filterDate);
            }
            if (afd != null)
            {
                DateTime filterDate2 = DateTime.Parse(afd.ToString());
                search = search.Where(a => a.CreatedOn > filterDate2);
            }
            return PartialView("_getAnalytic", search.Skip(skip).Take(10).ToList());
        }
        public JsonResult SendWeeklyReport(long[] idArr)
        {
            //    EmailController email = new EmailController();
            //    //Get Today News
            //    var idList = idArr.ToList();
            //    var EmailTime = DateTime.Today.ToString("dd.MM.yyyy");
            //    var subscriber = new List<string>();
            //    var newsData = await _db.DevNews.Where(a => idList.Contains(a.NewsId) && a.AdminCheck == true).Select(n => new { n.Id, n.Title, n.Description, n.Sector, n.ImageUrl }).ToListAsync();

            //    var subscriberUser = await _db.Subscriptions.Select(a => a.Email).ToListAsync();
            //    //Email to Registered User
            //    var RegisteredUsers = await _db.Users.Where(a => a.EmailConfirmed).Select(a => a.Email).ToListAsync();
            //    if (RegisteredUsers.Count() > 0)
            //    {
            //        subscriber = subscriberUser.Union(RegisteredUsers).Distinct().ToList();
            //    }
            //    else
            //    {
            //        subscriber = subscriberUser.Distinct().ToList();
            //    }
            //    List<string> listofNews = new List<string>();
            //    if (newsData.Any())
            //    {
            //        foreach (var item in newsData)
            //        {
            //            var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });
            //            listofNews.Add(string.Format("<tr>" +
            //                "<td width=\"100\" rowspan=\"2\" style=\"padding:10px;\">" +
            //                "<a href=\"\" target=\"_blank\" title=\"This external link will open in a new window\">" +
            //                "<img alt=\"News\" src=\"https://www.devdiscourse.com/" + item.ImageUrl + "?w=100\"></a></td>" +
            //                "<td style=\"padding:10px;\">" +
            //                "<a href=\"https://www.devdiscourse.com" + newsUrl + "\" style=\"color:##0090d4;text-decoration:none;\" target =\"_blank\" title=\"This external link will open in a new window\">" + item.Title + "</a>" +
            //                "</td></tr><tr><td style=\"padding:10px;\">" + TruncateDescription(item.Description) +
            //                "</td></tr>"));
            //        }

            //            if (newsData.Count() > 0)
            //            {
            //                string combindedString = string.Join("", listofNews.ToArray());
            //                string emailData = string.Format("<img src=\"https://www.devdiscourse.com/AdminFiles/Logo/Mail_Banner.jpg\" style=\"width:100%;max-width:800px;\"/>" +
            //                    "<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
            //                    "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
            //                    "<tr><td  align=\"right\" colspan=\"2\">Sent date: " + EmailTime + "</td></tr>" +
            //                    "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Dear colleague, <br><br> Latest " + newsData.Count() + " News posted today on <a href=\"http://www.devdiscourse.com/\" target =\"_blank\" title=\"This external link will open in a new window\">Devdiscourse</a> are listed below. To view all the News, <a href=\"http://www.devdiscourse.com/Home/Search\" target=\"_blank\" title=\"This external link will open in a new window\">click here. </a></td></tr>" +
            //                    "<tr><td colspan=\"2\"><table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; width: 100%; color: #555555; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\"><tbody>" +
            //                    combindedString +
            //                    "</tbody></table></td></tr>" +
            //                    "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Thank you, <br> The Devdiscourse team</td></tr>" +
            //                    "<tr style=\"background-color:#e1e1e1;font-size:12px;\"><td style=\"padding: 30px 0 30px 30px; width: 85%; vertical-align: bottom;\">" +
            //                    "<div>If you have any questions or concerns, <a href=\"https://www.devdiscourse.com/AboutUs#meet\" target =\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
            //                    "<br><div style=\"color: #696969;\"> © Copyright 2017 <a href=\"http://www.visionri.com/\" style =\"color:#222;text-decoration:unset;\"> VisionRI</a></div></td>" +
            //                    "<td style=\"padding: 0 30px 30px 0; width: 15%; vertical-align: bottom;\" ><div style= \"width: 100%; text-align: center;\" > Follow us:</div>" +
            //                    "<br><div style =\"width: 100%; text-align: center;\">" +
            //                    "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
            //                    "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
            //                    "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
            //                    "</div></td></tr>" +
            //                    "</table>" +
            //                    "</div>");
            //                email.SendMail("nmehta@visionri.com", emailData, "Devdiscourse News");
            //        }
            //}
            var data = new
            {
                message = "Successfull"
            };
            return Json(data);
        }
        public JsonResult SendNotification(long[] idArr, string description)
        {
            //EmailController email = new EmailController();
            ////Get Today News
            //var idList = idArr.ToList();
            //var EmailTime = DateTime.Today.ToString("dd.MM.yyyy");
            //var subscriber = new List<string>();
            //var newsData = await _db.DevNews.Where(a => idList.Contains(a.NewsId) && a.AdminCheck == true).Select(n => new { n.Id, n.Title, n.Description, n.Sector, n.ImageUrl }).ToListAsync();

            //var subscriberUser = await _db.Subscriptions.Select(a => a.Email).ToListAsync();
            //subscriber.Add("ankit.tvite@gmail.com");
            //List<string> listofNews = new List<string>();
            //if (newsData.Any())
            //{
            //    foreach (var item in newsData)
            //    {
            //        var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });
            //        listofNews.Add(string.Format("<tr>" +
            //            "<td width=\"100\" rowspan=\"2\" style=\"padding:10px;\">" +
            //            "<a href=\"\" target=\"_blank\" title=\"This external link will open in a new window\">" +
            //            "<img alt=\"News\" src=\"http://www.devdiscourse.com/" + item.ImageUrl + "?w=100\"></a></td>" +
            //            "<td style=\"padding:10px;\">" +
            //            "<a href=\"http://www.devdiscourse.com" + newsUrl + "\" style=\"color:##0090d4;text-decoration:none;\" target =\"_blank\" title=\"This external link will open in a new window\">" + item.Title + "</a>" +
            //            "</td></tr><tr><td style=\"padding:10px;\">" + TruncateDescription(item.Description) +
            //            "</td></tr>"));
            //    }
            //    if (subscriber.Any())
            //    {
            //        foreach (var item in subscriber)
            //        {
            //            if (!String.IsNullOrEmpty(item) && newsData.Count() > 0)
            //            {
            //                string combindedString = string.Join("", listofNews.ToArray());
            //                string emailData = string.Format("<img src=\"http://www.devdiscourse.com/AdminFiles/Logo/Mail_Banner.jpg\" style=\"width:100%;max-width:800px;\"/>" +
            //                    "<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
            //                    "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
            //                    "<tr><td  align=\"right\" colspan=\"2\">Sent date: " + EmailTime + "</td></tr>" +
            //                    "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Dear colleague, <br><br> " + description + " <br><br> To view all the News, <a href=\"http://www.devdiscourse.com/Home/Search\" target=\"_blank\" title=\"This external link will open in a new window\">click here. </a></td></tr>" +
            //                    "<tr><td colspan=\"2\"><table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; width: 100%; color: #555555; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\"><tbody>" +
            //                    combindedString +
            //                    "</tbody></table></td></tr>" +
            //                    "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Thank you, <br> The Devdiscourse team</td></tr>" +
            //                    "<tr style=\"background-color:#e1e1e1;font-size:12px;\"><td style=\"padding: 30px 0 30px 30px; width: 85%; vertical-align: bottom;\">" +
            //                    "<div>If you have any questions or concerns, <a href=\"http://www.devdiscourse.com/AboutUs#meet\" target =\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
            //                    "<br><div style=\"color: #696969;\"> © Copyright 2017 <a href=\"http://www.visionri.com/\" style =\"color:#222;text-decoration:unset;\"> VisionRI</a></div></td>" +
            //                    "<td style=\"padding: 0 30px 30px 0; width: 15%; vertical-align: bottom;\" ><div style= \"width: 100%; text-align: center;\" > Follow us:</div>" +
            //                    "<br><div style =\"width: 100%; text-align: center;\">" +
            //                    "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
            //                    "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
            //                    "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"http://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
            //                    "</div></td></tr>" +
            //                    "</table>" +
            //                    "</div>");
            //                email.SendMail(item, emailData, "Devdiscourse News");
            //            }
            //        }
            //    }
            //}
            var data = new
            {
                message = "Successfull"
            };
            return Json(data);
        }
        public string TruncateDescription(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);
            htmlString = htmlString.Replace("&ldquo;", "\"");
            htmlString = htmlString.Replace("&rdquo;", "\"");
            htmlString = htmlString.Replace("&lsquo;", "'");
            htmlString = htmlString.Replace("&rsquo;", "'");

            if (htmlString.Length <= 100)
            {
                return htmlString;
            }
            else
            {
                return htmlString.Substring(0, 100) + "...";
            }

        }
        public async Task SendMessage(string title, string desc, string newsId, string ImageUrl)
        {
            try
            {
                var applicationID = "AAAAS31kOEA:APA91bGZqeQdyNHF4kXD1VzfimttW6BiJywDm-x74Fesf_6cRKSR2_Jm7aFTrzszZpn6weqFkbV5sq53X8Qkpinm5TdEKpepkwzjOaKP37DKoi95OB1YSFpCQ7WeoekFZufR-klVb6BV";
                var senderId = "324226267200";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = "/topics/news",
                    notification = new
                    {
                        title = title,
                        body = desc,
                        icon = newsId
                    },
                    data = new
                    {
                        newsid = newsId,
                        description = desc,
                        newsImage = ImageUrl
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            Content(sResponseFromServer, "text/plain", System.Text.Encoding.UTF8);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Content(ex.Message, "text/plain", System.Text.Encoding.UTF8);
            }
        }
        public PartialViewResult GetLabels()
        {
            var search = _db.Labels.ToList();
            return PartialView("_getLabels", search);
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
        public JsonResult AssignNewsToUser(long id, string user)
        {
            var search = _db.DevNews.FirstOrDefault(a => a.NewsId == id);
            if (search == null) { return Json("Invalid NewsId"); }
            AssignNews obj = new AssignNews()
            {
                UserId = user,
                NewsId = id,
                CreatedOn = DateTime.UtcNow,
                Creator = userManager.GetUserId(User),
            };
            _db.AssignNews.Add(obj);
            _db.SaveChanges();

            search.IsIndexed = true;
            _db.Entry(search).Property("IsIndexed").IsModified = true;
            _db.SaveChanges();
            return Json("Success");
        }
        public string GetShiftUser()
        {
            var users = _db.Users.Where(a => a.isActive == true).OrderByDescending(o => o.CreatedOn).Select(s => s.Id).ToArray();
            int counter = 0;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Content", "counter.txt");
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
        [Authorize]
        public JsonResult AutoAssign(long id)
        {
            var user = GetShiftUser();
            if (user == "No User")
            {
                return Json("NoUser");
            }
            var search = _db.DevNews.FirstOrDefault(a => a.NewsId == id);
            if (search == null) { return Json("Invalid NewsId"); }
            AssignNews obj = new AssignNews()
            {
                UserId = user,
                NewsId = id,
                CreatedOn = DateTime.UtcNow,
                Creator = userManager.GetUserId(User),
            };
            _db.AssignNews.Add(obj);
            _db.SaveChanges();

            search.IsIndexed = true;
            _db.Entry(search).Property("IsIndexed").IsModified = true;
            _db.SaveChanges();
            return Json("Success");
        }
        [Authorize]
        public async Task<JsonResult> AutoAssignWithAlert(long id)
        {
            var user = GetShiftUser();
            if (user == "No User")
            {
                return Json("NoUser");
            }
            var search = _db.DevNews.FirstOrDefault(a => a.NewsId == id);
            if (search == null) { return Json("Invalid NewsId"); }
            AssignNews obj = new AssignNews()
            {
                UserId = user,
                NewsId = id,
                CreatedOn = DateTime.UtcNow,
                Creator = userManager.GetUserId(User),
            };
            _db.AssignNews.Add(obj);
            _db.SaveChanges();

            search.IsIndexed = true;
            _db.Entry(search).Property("IsIndexed").IsModified = true;
            _db.SaveChanges();

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
           await context.Clients.All.SendAsync("NewsAssignNotification","New news assign to you", user);

            return Json("Success");
        }
        // Author News NewsList
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        // GET: DevNews
        public ActionResult NewsList(DateTime? bfd, DateTime? afd, int? page = 1, string region = "Global Edition", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false)
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
            string userId = userManager.GetUserId(User);
            ViewBag.loginId = userId;
            DateTime fifteenDay = DateTime.Today.AddDays(-115);
            IQueryable<NewsListView> devNews;
            if (string.IsNullOrEmpty(text))
            {
                devNews = _db.DevNews.Where(a => a.Type == "News" && a.Creator == userId && a.CreatedOn > fifteenDay).Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Category = a.Category, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage });
            }
            else
            {
                devNews = _db.DevNews.Where(a => a.Type == "News" && a.Creator == userId).Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Category = a.Category, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage });
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
            if (region != "Global Edition")
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
        private async Task<CloudBlobContainer> GetCloudBlobImageContainer()
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            string connectionString = config.GetConnectionString("devdiscourse_AzureStorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("imagegallery");
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
            if (disposing && _db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
