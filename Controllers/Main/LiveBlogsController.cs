using Devdiscourse.Data;
using Devdiscourse.Hubs;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Devdiscourse.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Devdiscourse.Controllers.Main
{
    public class LiveBlogsController : Controller
    {
        private readonly ApplicationDbContext db;

        // private ApplicationDbContext db = new ApplicationDbContext();
        public LiveBlogsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: LiveBlogs
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Index()
        {
            return View(db.LiveBlogs.ToList());
        }

        // GET: LiveBlogs/Details/5
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult Details(long? id, Guid? pid)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    if (pid != null)
        //    {
        //        ViewBag.pid = pid;
        //    }
        //    LiveBlog liveBlog = db.LiveBlogs.Find(id);
        //    if (liveBlog == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(liveBlog);
        //}

        // GET: LiveBlogs/Create
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult Create(Guid? id)
        //{
        //    ViewBag.id = id;
        //    ViewBag.parent = db.DevNews.Find(id);
        //    TempData["nid"] = id;
        //    return View();
        //}

        //// POST: LiveBlogs/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult Create([Bind(Include = "Id,Title,Description,ParentId,ImageUrl,ImageCopyright")] LiveBlog liveBlog, string ChooseImage)
        ////{
        ////    Guid newsId = Guid.Parse(TempData["nid"].ToString());
        ////    if (ModelState.IsValid)
        ////    {
        ////        if (Request.Files.Count > 0)
        ////        {
        ////            for (var i = 0; i < Request.Files.Count; i++)
        ////            {
        ////                var file = Request.Files[i];
        ////                if (file == null || file.ContentLength <= 0) continue;
        ////                var fileName = RandomName();
        ////                var fileExtension = Path.GetExtension(file.FileName);
        ////                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        ////                string fileSize = file.ContentLength.ToString();
        ////                var fileKey = Request.Files.Keys[i];
        ////                if (fileKey == "ImageUrl")
        ////                {
        ////                    var path = Path.Combine(Server.MapPath("~/AdminFiles/NewsImages/"), fileName + fileExtension);
        ////                    file.SaveAs(path);
        ////                    liveBlog.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
        ////                    liveBlog.FileMimeType = mimeType;
        ////                    liveBlog.FileSize = fileSize;
        ////                }
        ////            }
        ////        }
        ////        if (!String.IsNullOrEmpty(ChooseImage))
        ////        {
        ////            liveBlog.ImageUrl = ChooseImage;
        ////        }
        ////        liveBlog.Creator = User.Identity.GetUserId();
        ////        liveBlog.CreatedOn = DateTime.UtcNow;
        ////        liveBlog.ModifiedOn = DateTime.UtcNow;
        ////        db.LiveBlogs.Add(liveBlog);
        ////        db.SaveChanges();
        ////        // Update Live Blog
        ////        var find = db.DevNews.Find(newsId);
        ////        find.ModifiedOn = DateTime.UtcNow;
        ////        db.Entry(find).State = EntityState.Modified;
        ////        db.SaveChanges();
        ////        //
        ////        var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        ////        context.Clients.All.LiveBlogNotification(liveBlog.Id, liveBlog.ParentId, liveBlog.Title, liveBlog.Description, liveBlog.ImageUrl, liveBlog.CreatedOn);
        ////        return RedirectToAction("BlogDetails", "LiveBlogs", new { id = newsId });
        ////    }
        ////    ViewBag.parent = db.DevNews.Find(newsId);
        ////    return View(liveBlog);
        ////}

        //// GET: LiveBlogs/Edit/5
        ////[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        ////public ActionResult Edit(long? id, Guid? pid)
        ////{
        ////    if (id == null)
        ////    {
        ////        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        ////    }
        ////    if (pid != null)
        ////    {
        ////        ViewBag.pid = pid;
        ////        TempData["pid"] = pid;
        ////    }
        ////    LiveBlog liveBlog = db.LiveBlogs.Find(id);
        ////    if (liveBlog == null)
        ////    {
        ////        return HttpNotFound();
        ////    }
        ////    return View(liveBlog);
        ////}

        //// POST: LiveBlogs/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult Edit([Bind(Include = "Id,Title,Description,ParentId,ImageUrl,ImageCopyright,FileMimeType,FileSize,Creator,CreatedOn")] LiveBlog liveBlog, string ChooseImage)
        ////{
        ////    if (ModelState.IsValid)
        ////    {
        ////        if (Request.Files.Count > 0)
        ////        {
        ////            for (var i = 0; i < Request.Files.Count; i++)
        ////            {
        ////                var file = Request.Files[i];
        ////                if (file == null || file.ContentLength <= 0) continue;
        ////                var fileName = RandomName();
        ////                var fileExtension = Path.GetExtension(file.FileName);
        ////                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        ////                string fileSize = file.ContentLength.ToString();
        ////                var fileKey = Request.Files.Keys[i];
        ////                if (fileKey == "ImageUrlUpdate")
        ////                {
        ////                    var path = Path.Combine(Server.MapPath("~/AdminFiles/NewsImages/"), fileName + fileExtension);
        ////                    file.SaveAs(path);
        ////                    liveBlog.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
        ////                    liveBlog.FileMimeType = mimeType;
        ////                    liveBlog.FileSize = fileSize;
        ////                }
        ////            }
        ////        }
        ////        if (!String.IsNullOrEmpty(ChooseImage))
        ////        {
        ////            liveBlog.ImageUrl = ChooseImage;
        ////        }
        ////        liveBlog.ModifiedOn = DateTime.UtcNow;
        ////        db.Entry(liveBlog).State = EntityState.Modified;
        ////        db.SaveChanges();
        ////        Guid pId = Guid.Parse(TempData["pid"].ToString());
        ////        // Update Live Blog
        ////        var find = db.DevNews.Find(pId);
        ////        find.ModifiedOn = DateTime.UtcNow;
        ////        db.Entry(find).State = EntityState.Modified;
        ////        db.SaveChanges();
        ////        return RedirectToAction("BlogDetails", "LiveBlogs", new { id = pId });
        ////    }
        ////    return View(liveBlog);
        ////}

        ////// GET: LiveBlogs/Delete/5
        ////[Authorize(Roles = "SuperAdmin")]
        ////public ActionResult Delete(long? id, Guid? pid)
        ////{
        ////    if (id == null)
        ////    {
        ////        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        ////    }
        ////    if (pid != null)
        ////    {
        ////        ViewBag.pid = pid;
        ////        TempData["pid"] = pid;
        ////    }
        ////    LiveBlog liveBlog = db.LiveBlogs.Find(id);
        ////    if (liveBlog == null)
        ////    {
        ////        return HttpNotFound();
        ////    }
        ////    return View(liveBlog);
        ////}

        //// POST: LiveBlogs/Delete/5
        ////[HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult DeleteConfirmed(long id)
        ////{
        ////    LiveBlog liveBlog = db.LiveBlogs.Find(id);
        ////    db.LiveBlogs.Remove(liveBlog);
        ////    db.SaveChanges();
        ////    if (!String.IsNullOrEmpty(TempData["pid"].ToString()))
        ////    {
        ////        Guid pId = Guid.Parse(TempData["pid"].ToString());
        ////        return RedirectToAction("BlogDetails", "LiveBlogs", new { id = pId });
        ////    }
        ////    else
        ////    {
        ////        return RedirectToAction("BlogList");
        ////    }
        ////}
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult BlogList(DateTime? bfd, DateTime? afd, int? page = 1, string region = "", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false)
        //{
        //    ViewBag.label = label;
        //    ViewBag.sector = sector;
        //    ViewBag.category = category;
        //    ViewBag.region = region;
        //    ViewBag.country = country;
        //    ViewBag.afDate = afd;
        //    ViewBag.bfDate = bfd;
        //    ViewBag.text = text;
        //    ViewBag.uid = uid;
        //    ViewBag.source = source;
        //    ViewBag.editorPick = editorPick;
        //    ViewBag.loginId = User.Identity.GetUserId();
        //    var devNews = db.DevNews.Where(a => (a.Type == "LiveBlog" || a.Type == "Discourse") && a.EditorPick == editorPick).Select(a => new NewsListView { Id = a.Id, NewsId = a.NewsId, Label = a.NewsLabels, Sector = a.Sector, Title = a.Title, SubTitle = a.SubTitle, Creator = a.Creator, CreatorName = a.Author, Region = a.Region, Country = a.Country, ImageUrl = a.ImageUrl, Source = a.Source, SourceUrl = a.SourceUrl, AdminCheck = a.AdminCheck, IsInfocus = a.IsInfocus, EditorPick = a.EditorPick, IsGlobal = a.IsGlobal, IsFifa = a.IsStandout, IsIndex = a.IsIndexed, CreatedOn = a.CreatedOn, WorkStage = a.WorkStage, ViewCount = a.ViewCount });
        //    if (label != "0")
        //    {
        //        devNews = devNews.Where(a => a.Label.Contains("," + label + ",") || a.Label.StartsWith("," + label) || a.Label.EndsWith(label + ",") || a.Label.Equals(label));
        //    }
        //    if (sector != "0")
        //    {
        //        devNews = devNews.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
        //    }
        //    if (category != "0")
        //    {
        //        devNews = devNews.Where(a => a.Category.Contains("," + category + ",") || a.Category.StartsWith("," + category) || a.Category.EndsWith(category + ",") || a.Category.Equals(category));
        //    }
        //    if (region != "Global Edition" && region != "")
        //    {
        //        devNews = devNews.Where(a => a.Region.Contains(region));
        //    }
        //    if (!String.IsNullOrEmpty(country))
        //    {
        //        devNews = devNews.Where(a => a.Country.Contains(country));
        //    }
        //    if (!String.IsNullOrEmpty(source))
        //    {
        //        devNews = devNews.Where(a => a.Source.Contains(source));
        //    }
        //    if (!String.IsNullOrEmpty(text))
        //    {
        //        devNews = devNews.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
        //    }
        //    if (bfd != null)
        //    {
        //        DateTime filterDate = DateTime.Parse(bfd.ToString());
        //        devNews = devNews.Where(a => a.CreatedOn < filterDate);
        //    }
        //    if (afd != null)
        //    {
        //        DateTime filterDate2 = DateTime.Parse(afd.ToString());
        //        devNews = devNews.Where(a => a.CreatedOn > filterDate2);
        //    }
        //    if (!String.IsNullOrEmpty(uid))
        //    {
        //        devNews = devNews.Where(a => a.Creator == uid);
        //    }
        //    return View(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        //}
       
        // GET: LiveBlogs/BlogDetails/5
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult BlogDetails(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DevNews devNews = db.DevNews.Find(id);
        //    if (devNews == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(devNews);
        //}
        //// GET: LiveBlogs/CreateBlog
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult CreateBlog()
        //{
        //    ViewBag.Creator = new SelectList(db.Users, "Id", "Email");
        //    ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
        //    ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title");
        //    ViewBag.Category = new SelectList(db.Categories, "Id", "Title");
        //    ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title");
        //    ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
        //    return View();
        //}

        // POST: LiveBlogs/EditBlog
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateBlog([Bind(Include = "Id,Title,SubTitle,Description,NewsLabels,Category,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Type,Country,Tags,Source,SourceUrl,EditorPick")] DevNews devNews, string ChooseImage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            for (var i = 0; i < Request.Files.Count; i++)
        //            {
        //                var file = Request.Files[i];
        //                if (file == null || file.ContentLength <= 0) continue;
        //                var fileName = RandomName();
        //                var fileExtension = Path.GetExtension(file.FileName);
        //                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        //                string fileSize = file.ContentLength.ToString();
        //                var fileKey = Request.Files.Keys[i];
        //                if (fileKey == "ImageUrl")
        //                {
        //                    var path = Path.Combine(Server.MapPath("~/AdminFiles/NewsImages/"), fileName + fileExtension);
        //                    file.SaveAs(path);
        //                    devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
        //                    devNews.FileMimeType = mimeType;
        //                    devNews.FileSize = fileSize;
        //                }
        //            }
        //        }
        //        if (String.IsNullOrEmpty(devNews.ImageUrl) && !String.IsNullOrEmpty(devNews.Sector))
        //        {
        //            var sec = devNews.Sector.Split(',')[0];
        //            devNews.ImageUrl = "/images/sector/all_sectors.jpg";
        //            devNews.FileMimeType = "image/jpg";
        //            devNews.FileSize = "88,651";
        //        }
        //        if (!String.IsNullOrEmpty(ChooseImage))
        //        {
        //            devNews.ImageUrl = ChooseImage;
        //            // Find Image in Old Image Gallery
        //            var findimage = db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
        //            if (findimage != null)
        //            {
        //                // Saved Image in New Image Gallery
        //                string imgcopyright = "";
        //                if (!string.IsNullOrEmpty(devNews.ImageCopyright))
        //                {
        //                    imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
        //                }
        //                ImageGallery fileobj = new ImageGallery()
        //                {
        //                    Title = findimage.Title,
        //                    ImageUrl = findimage.FileUrl,
        //                    ImageCopyright = imgcopyright,
        //                    Caption = "",
        //                    FileMimeType = findimage.FileMimeType,
        //                    FileSize = findimage.FileSize,
        //                    Sector = findimage.FileFor,
        //                    Tags = "",
        //                    UseCount = 1,
        //                };
        //                db.ImageGallery.Add(fileobj);
        //                db.SaveChanges();
        //                // Remove from old gallery
        //                db.UserFiles.Remove(findimage);
        //                db.SaveChanges();
        //            }
        //        }
        //        devNews.Id = Guid.NewGuid();
        //        devNews.AdminCheck = false;
        //        devNews.IsSponsored = false;
        //        devNews.IsInfocus = false;
        //        devNews.IsVideo = false;
        //        devNews.IsStandout = false;
        //        devNews.IsGlobal = false;
        //        devNews.IsIndexed = false;
        //        devNews.Author = "";
        //        //devNews.Type = "LiveBlog";
        //        devNews.SubType = "";
        //        devNews.ViewCount = 0;
        //        devNews.LikeCount = 0;
        //        devNews.WorkStage = "";
        //        devNews.OriginalSource = devNews.Source;
        //        devNews.Creator = User.Identity.GetUserId();
        //        db.DevNews.Add(devNews);
        //        db.SaveChanges();
        //        return RedirectToAction("BlogList");
        //    }

        //    ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
        //    ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
        //    ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
        //    ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
        //    ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
        //    ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
        //    return View(devNews);
        //}
        //// GET: LiveBlogs/EditBlog/5
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult EditBlog(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DevNews devNews = db.DevNews.Find(id);
        //    if (devNews == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    TempData["AdminCheck"] = devNews.AdminCheck;
        //    ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
        //    ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
        //    ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
        //    ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
        //    ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.SubType);
        //    ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);

        //    string userId = User.Identity.GetUserId();
        //    var userWork = db.UserWorks.FirstOrDefault(a => a.UserId == userId);

        //    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        //    if (userWork != null && userWork.WorkStage != "Image Change")
        //    {
        //        devNews.WorkStage = userWork.UserName + " - " + userWork.WorkStage + "," + userWork.ColorCode;
        //        db.Entry(devNews).State = EntityState.Modified;
        //        db.SaveChanges();
        //        context.Clients.All.NewsOpenNotification(devNews.NewsId, userWork.UserName + " - " + userWork.WorkStage, userWork.ColorCode);
        //    }
        //    return View(devNews);
        //}

        // POST: DevNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditBlog([Bind(Include = "Id,NewsId,Title,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,ImageCaption,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,PublishedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount,WorkStage")] DevNews devNews, string ChooseImage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            for (var i = 0; i < Request.Files.Count; i++)
        //            {
        //                var file = Request.Files[i];
        //                if (file == null || file.ContentLength <= 0) continue;
        //                var fileName = RandomName();
        //                var fileExtension = Path.GetExtension(file.FileName);
        //                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        //                string fileSize = file.ContentLength.ToString();
        //                var fileKey = Request.Files.Keys[i];

        //                if (fileKey == "ImageUrlUpdate")
        //                {
        //                    var path = Path.Combine(Server.MapPath("~/AdminFiles/NewsImages/"), fileName + fileExtension);
        //                    file.SaveAs(path);
        //                    devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
        //                    devNews.FileMimeType = mimeType;
        //                    devNews.FileSize = fileSize;
        //                }
        //            }
        //        }
        //        if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
        //        {
        //            devNews.ImageUrl = "/images/sector/all_sectors.jpg";
        //            devNews.FileMimeType = "image/jpg";
        //            devNews.FileSize = "88,651";
        //        }
        //        if (!String.IsNullOrEmpty(ChooseImage))
        //        {
        //            devNews.ImageUrl = ChooseImage;
        //            // Find Image in Old Image Gallery
        //            var findimage = db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
        //            if (findimage != null)
        //            {
        //                // Saved Image in New Image Gallery
        //                string imgcopyright = "";
        //                if (!string.IsNullOrEmpty(devNews.ImageCopyright))
        //                {
        //                    imgcopyright = devNews.ImageCopyright.Replace("Image Credit: ", "") ?? "";
        //                }
        //                ImageGallery fileobj = new ImageGallery()
        //                {
        //                    Title = findimage.Title,
        //                    ImageUrl = findimage.FileUrl,
        //                    ImageCopyright = imgcopyright,
        //                    Caption = "",
        //                    FileMimeType = findimage.FileMimeType,
        //                    FileSize = findimage.FileSize,
        //                    Sector = findimage.FileFor,
        //                    Tags = "",
        //                    UseCount = 1,
        //                };
        //                db.ImageGallery.Add(fileobj);
        //                db.SaveChanges();
        //                // Remove from old gallery
        //                db.UserFiles.Remove(findimage);
        //                db.SaveChanges();
        //            }
        //        }
        //        if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
        //        {
        //            devNews.PublishedOn = DateTime.UtcNow;
        //        }
        //        devNews.ModifiedOn = DateTime.UtcNow;
        //        db.Entry(devNews).State = EntityState.Modified;
        //        db.SaveChanges();
        //        if (TempData["AdminCheck"].ToString() == "False" && devNews.AdminCheck == true)
        //        {
        //            string description = Regex.Replace(devNews.Description, @"<[^>]+>|&nbsp;", "").Trim();
        //            SaveNews(devNews.Id, devNews.Title, description);
        //            CreateLog(devNews.Title + " LiveBlog ", devNews.Title + " has been updated", devNews.Creator, User.Identity.GetUserId(), "/Article/Index/" + devNews.NewsId);
        //        }
        //        return RedirectToAction("BlogList");
        //    }
        //    ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
        //    ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
        //    ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
        //    ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
        //    ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
        //    ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
        //    return View(devNews);
        //}
        //[Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        //public ActionResult Clone(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DevNews devNews = db.DevNews.Find(id);
        //    if (devNews == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    TempData["AdminCheck"] = devNews.AdminCheck;
        //    TempData["IsStandout"] = devNews.IsStandout;
        //    ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
        //    ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
        //    ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
        //    ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
        //    ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
        //    ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
        //    return View(devNews);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Clone([Bind(Include = "Id,NewsId,Title,SubTitle,Description,Type,NewsLabels,SubType,Category,Author,Sector,Themes,ImageUrl,ImageCopyright,Region,Country,Tags,Source,OriginalSource,SourceUrl,Creator,CreatedOn,ModifiedOn,AdminCheck,IsSponsored,EditorPick,IsInfocus,IsVideo,IsGlobal,IsStandout,IsIndexed,ViewCount")] DevNews devNews)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            for (var i = 0; i < Request.Files.Count; i++)
        //            {
        //                var file = Request.Files[i];
        //                if (file == null || file.ContentLength <= 0) continue;
        //                var fileName = RandomName();
        //                var fileExtension = Path.GetExtension(file.FileName);
        //                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        //                string fileSize = file.ContentLength.ToString();
        //                var fileKey = Request.Files.Keys[i];
        //                if (fileKey == "ImageUrlUpdate")
        //                {
        //                    var path = Path.Combine(Server.MapPath("~/AdminFiles/NewsImages/"), fileName + fileExtension);
        //                    file.SaveAs(path);
        //                    devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;
        //                    devNews.FileMimeType = mimeType;
        //                    devNews.FileSize = fileSize;
        //                }
        //            }
        //        }
        //        if ((String.IsNullOrEmpty(devNews.ImageUrl) || devNews.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(devNews.Sector))
        //        {
        //            devNews.ImageUrl = "/images/sector/all_sectors.jpg";
        //            devNews.FileMimeType = "image/jpg";
        //            devNews.FileSize = "88,651";
        //        }
        //        devNews.IsIndexed = false;
        //        devNews.CreatedOn = DateTime.UtcNow;
        //        devNews.ModifiedOn = DateTime.UtcNow;
        //        db.DevNews.Add(devNews);
        //        db.SaveChanges();
        //        return RedirectToAction("BlogList");
        //    }
        //    ViewBag.Creator = new SelectList(db.Users, "Id", "Email", devNews.Creator);
        //    ViewBag.Sector = new SelectList(db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", devNews.Sector);
        //    ViewBag.Themes = new SelectList(db.DevThemes, "Id", "Title", devNews.Themes);
        //    ViewBag.Category = new SelectList(db.Categories, "Id", "Title", devNews.Category);
        //    ViewBag.NewsLabels = new SelectList(db.Labels, "Slug", "Title", devNews.NewsLabels);
        //    ViewBag.Region = new SelectList(db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", devNews.Region);
        //    return View(devNews);
        //}
        // GET: LiveBlogs/DeleteBlog/5
        //[Authorize(Roles = "SuperAdmin")]
        //public ActionResult DeleteBlog(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DevNews devNews = db.DevNews.Find(id);
        //    if (devNews == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(devNews);
        //}

        //// POST: DevNews/Delete/5
        //[HttpPost, ActionName("DeleteBlogConfirmed")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteBlogConfirmed(Guid id)
        //{
        //    DevNews devNews = db.DevNews.Find(id);
        //    db.DevNews.Remove(devNews);
        //    db.SaveChanges();
        //    CreateLog(devNews.Title + "" + devNews.Type, devNews.Title + " has been deleted", devNews.Creator, User.Identity.GetUserId(), "/Home/NewsDetail/" + devNews.NewsId);
        //    return RedirectToAction("BlogList");
        //}
        //// Send mail to subscriber
        //public void SaveNews(Guid id, string title, string description)
        //{
        //    SubscribeNews obj = new SubscribeNews
        //    {
        //        ItemId = id,
        //        Title = title,
        //        Description = description
        //    };
        //    db.SubscribeNews.Add(obj);
        //    db.SaveChanges();
        //}
        //public void CreateLog(string title, string logFor, string creator, string activityToUser, string activityUrl)
        //{
        //    ActivityLog logs = new ActivityLog
        //    {
        //        LogTitle = title,
        //        LogDescription = title,
        //        CreatorId = creator,
        //        ActivityUserId = activityToUser,
        //        Activity = logFor,
        //        ActivityUrl = activityUrl,
        //        ActivityDate = DateTime.Now,
        //        IsRead = false
        //    };
        //    db.ActivityLogs.Add(logs);
        //    db.SaveChanges();
        //}
        //public string RandomName()
        //{
        //    var time = DateTime.UtcNow.ToLocalTime();
        //    return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        //}
        //public PartialViewResult GetSubBlogs(long id, Guid sid)
        //{
        //    ViewBag.sid = sid;
        //    var search = db.LiveBlogs.Where(a => a.ParentId == id).ToList();
        //    return PartialView("_getSubBlogs", search);
        //}
        //public PartialViewResult GetLiveBlogs(long id)
        //{
        //    var search = db.LiveBlogs.Where(a => a.ParentId == id).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        //    {
        //        Title = s.Title,
        //        Description = s.Description,
        //        Id = s.Id,
        //        CreatedOn = s.CreatedOn,
        //        ImageUrl = s.ImageUrl,
        //        ImageCopyright = s.ImageCopyright,
        //        ModifiedOn = s.ModifiedOn,
        //        DiscourseComments = db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        //        CommentCount = db.DiscourseComments.Count(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false)
        //    }).ToList();
        //    return PartialView("_getLiveBlogs", search);
        //}
        //public PartialViewResult GetLiveBlogsamp(long id)
        //{
        //    var search = db.LiveBlogs.Where(a => a.ParentId == id).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        //    {
        //        Title = s.Title,
        //        Description = s.Description,
        //        Id = s.Id,
        //        CreatedOn = s.CreatedOn,
        //        ImageUrl = s.ImageUrl,
        //        ImageCopyright = s.ImageCopyright,
        //        ModifiedOn = s.ModifiedOn,
        //        DiscourseComments = db.DiscourseComments.Where(a => a.ItemId == s.Id).ToList()
        //    }).ToList();
        //    return PartialView("_getLiveBlogsamp", search);
        //}
        //public JsonResult saveDiscourseComment(long itemId, string commentText, long parentId, long rootParentId, string reply)
        //{
        //    DiscourseComment discourseComment = new DiscourseComment()
        //    {
        //        ItemId = itemId,
        //        CommentText = commentText,
        //        CommentBy = User.Identity.GetUserId(),
        //        CommentOn = DateTime.UtcNow,
        //        RootParentId = rootParentId,
        //        ReplyText = reply,
        //        ParentId = parentId
        //    };
        //    db.DiscourseComments.Add(discourseComment);
        //    db.SaveChanges();
        //    if (parentId != 0)
        //    {
        //        UpdateChildCommentCount(parentId);
        //    }
        //    var comment = db.DiscourseComments.Where(a => a.CommentId == discourseComment.CommentId && a.IsHidden == false).Select(a => new { name = a.ApplicationUser.FirstName + " " + a.ApplicationUser.LastName, commentText = a.CommentText, parentId = a.ParentId, commentId = a.CommentId, itemId = a.ItemId, isHidden = a.IsHidden, childCount = a.ChildCount, rootParentId = a.RootParentId, replyText = a.ReplyText });
        //    return Json(comment, JsonRequestBehavior.AllowGet);
        //}
        //public void UpdateChildCommentCount(long parentId)
        //{
        //    var discourseComment = db.DiscourseComments.Find(parentId);
        //    if (discourseComment != null)
        //    {
        //        discourseComment.ChildCount = db.DiscourseComments.Count(a => a.ParentId == parentId && a.IsHidden == false);
        //        db.Entry(discourseComment).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        //public JsonResult removeComment(long id)
        //{
        //    var discourseComment = db.DiscourseComments.Find(id);
        //    long parentId = 0;
        //    if (discourseComment != null)
        //    {
        //        parentId = discourseComment.ParentId;
        //        discourseComment.IsHidden = true;
        //        db.Entry(discourseComment).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    if (parentId != 0)
        //    {
        //        UpdateChildCommentCount(parentId);
        //    }
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
