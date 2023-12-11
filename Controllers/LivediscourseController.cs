
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Devdiscourse.Data;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;
using PartialViewResult = System.Web.Mvc.PartialViewResult;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using ValidateAntiForgeryTokenAttribute = Microsoft.AspNetCore.Mvc.ValidateAntiForgeryTokenAttribute;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;


namespace Devdiscourse.Controllers
{
    public class LivediscourseController : Controller, IDisposable
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public LivediscourseController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {

            this._db = _db;
            this.userManager = userManager;
        }
        // GET: Livediscourse
        [Authorize(Roles = "SuperAdmin,Admin,Upfront")]
        // GET: DevNews
        public ActionResult Index(DateTime? bfd, DateTime? afd, int? page = 1, string region = "", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false)
        {
            ViewBag.sector = sector ?? "";
            ViewBag.region = region ?? "";
            ViewBag.country = country ?? "";
            ViewBag.text = text ?? "";
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            DateTime fifteenDay = DateTime.Today.AddDays(-115);
            var livediscourse = _db.Livediscourses.Where(a => a.Title.Contains(text) && a.Region.Contains(region) && a.Country.Contains(country) && a.ParentId == 0).AsEnumerable();

            if (sector != "0")
            {
                livediscourse = livediscourse.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
            }
            if (bfd != null)
            {
                DateTime filterDate = DateTime.Parse(bfd.ToString());
                livediscourse = livediscourse.Where(a => a.CreatedOn < filterDate);
            }
            if (afd != null)
            {
                DateTime filterDate2 = DateTime.Parse(afd.ToString());
                livediscourse = livediscourse.Where(a => a.CreatedOn > filterDate2);
            }
            return View(livediscourse.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        //[Authorize]
        //public async Task<ActionResult> Create(long? id)
        //{
        //    ViewBag.id = id ?? 0;
        //    if (id == null && (!User.IsInRole("SuperAdmin") && !User.IsInRole("Admin")))
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.IsModerator = false;
        //    if (id != null)
        //    {
        //        var parent = await _db.Livediscourses.FindAsync(id);
        //        ViewBag.parent = parent;
        //        var user = userManager.GetUserId(User);
        //        ViewBag.IsModerator = await IsModerator(parent.Id, user);
        //    }
        //    ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
        //    ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,Title,SubTitle,Description,Sector,ImageUrl,ParentId,Author,IsPublic,ImageCopyright,ImageCaption,LivediscourseIndex,Region,Country,Tags,Close_Date,Status")] Livediscourse livediscourse, string ChooseImage, List<string> FileTitle, List<string> FilePath, List<string> MimeType, List<string> FileSize, List<string> FileCaption, List<string> FileThumbUrl, List<string> FileDuration)
        //{
        //    var user = userManager.GetUserId(User);
        //    var isModerator = await IsModerator(livediscourse.ParentId, user);
        //    ViewBag.IsModerator = isModerator;
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
        //                var actName = Path.GetFileNameWithoutExtension(file.FileName);
        //                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        //                string fileSize = file.ContentLength.ToString();
        //                var fileKey = Request.Files.Keys[i];
        //                if (fileKey == "ImageUrl")
        //                {
        //                    //var path = Path.Combine(Server.MapPath("~/AdminFiles/NewsImages/"), fileName + fileExtension);
        //                    //file.SaveAs(path);
        //                    //devNews.ImageUrl = "/AdminFiles/NewsImages/" + fileName + fileExtension;

        //                    CloudBlobContainer blobContainer = GetCloudBlobContainer();
        //                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
        //                    blob.UploadFromStream(file.InputStream);
        //                    livediscourse.ImageUrl = blob.Uri.ToString();
        //                    // Saved Image in Image Gallery
        //                    string imgcopyright = "";
        //                    if (!string.IsNullOrEmpty(livediscourse.ImageCopyright))
        //                    {
        //                        imgcopyright = livediscourse.ImageCopyright.Replace("Image Credit: ", "") ?? "";
        //                    }
        //                    ImageGallery fileobj = new ImageGallery()
        //                    {
        //                        Title = actName,
        //                        ImageUrl = livediscourse.ImageUrl,
        //                        ImageCopyright = imgcopyright,
        //                        Caption = "",
        //                        FileMimeType = mimeType,
        //                        FileSize = fileSize,
        //                        Sector = livediscourse.Sector,
        //                        Tags = "",
        //                        UseCount = 1,
        //                    };
        //                    _db.ImageGalleries.Add(fileobj);
        //                    await _db.SaveChangesAsync();
        //                }
        //            }
        //        }
        //        if (!String.IsNullOrEmpty(ChooseImage))
        //        {
        //            livediscourse.ImageUrl = ChooseImage;
        //            // Find Image in Old Image Gallery
        //            var findimage = _db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
        //            if (findimage != null)
        //            {
        //                // Saved Image in New Image Gallery
        //                string imgcopyright = "";
        //                if (!string.IsNullOrEmpty(livediscourse.ImageCopyright))
        //                {
        //                    imgcopyright = livediscourse.ImageCopyright.Replace("Image Credit: ", "") ?? "";
        //                }
        //                ImageGalleries fileobj = new ImageGalleries()
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
        //                _db.ImageGalleries.Add(fileobj);
        //                await _db.SaveChangesAsync();
        //                // Remove from old gallery
        //                _db.UserFiles.Remove(findimage);
        //                await _db.SaveChangesAsync();
        //            }
        //        }
        //        else if (String.IsNullOrEmpty(livediscourse.ImageUrl) && !String.IsNullOrEmpty(livediscourse.Sector))
        //        {
        //            livediscourse.ImageUrl = "/images/sector/all_sectors.jpg";
        //        }
        //        livediscourse.AdminCheck = false;
        //        livediscourse.PublishedOn = DateTime.UtcNow;
        //        livediscourse.Region = livediscourse.Region ?? "";
        //        livediscourse.Country = livediscourse.Country ?? "";
        //        livediscourse.ViewCount = 0;
        //        livediscourse.LikeCount = 0;
        //        livediscourse.ParentStoryLink = "";
        //        livediscourse.Creator = userManager.GetUserId(User);
        //        _db.Livediscourses.Add(livediscourse);
        //        await _db.SaveChangesAsync();
        //        if (livediscourse.ParentId != 0)
        //        {
        //            var parent = await _db.Livediscourses.FindAsync(livediscourse.ParentId);
        //            if (parent != null)
        //            {
        //                parent.ModifiedOn = DateTime.UtcNow;
        //                _db.Entry(parent).State = EntityState.Modified;
        //                await _db.SaveChangesAsync();
        //            }
        //        }

        //        if (FileTitle != null)
        //        {
        //            List<LivediscourseVideo> userFileList = new List<LivediscourseVideo>();
        //            foreach (var item in FileTitle)
        //            {
        //                var index = FileTitle.IndexOf(item);
        //                var filepath = FilePath[index];
        //                var mimetype = MimeType[index];
        //                var fileSize = FileSize[index];
        //                var caption = FileCaption[index];
        //                var thumbUrl = FileThumbUrl[index];
        //                var fDuration = FileDuration[index];
        //                var livediscourseId = livediscourse.Id;
        //                LivediscourseVideo obj = new LivediscourseVideo();
        //                obj.Title = item;
        //                obj.FilePath = filepath;
        //                obj.FileMimeType = mimetype;
        //                obj.FileSize = fileSize;
        //                obj.FileCaption = caption;
        //                obj.FileThumbUrl = thumbUrl;
        //                obj.Duration = fDuration;
        //                obj.LivediscourseId = livediscourseId;
        //                userFileList.Add(obj);
        //            }
        //            if (userFileList.Count > 0)
        //            {
        //                _db.LivediscourseVideos.AddRange(userFileList);
        //                await _db.SaveChangesAsync();
        //            }
        //        }

        //        if (livediscourse.ParentId == 0 && (User.IsInRole("SuperAdmin") || User.IsInRole("Admin")))
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        if (livediscourse.ParentId != 0 && (User.IsInRole("SuperAdmin") || User.IsInRole("Admin") || isModerator == true))
        //        {
        //            return RedirectToAction("Details", new { id = livediscourse.ParentId });
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    ViewBag.id = livediscourse.ParentId;
        //    if (livediscourse.ParentId != 0)
        //    {
        //        var parent = await _db.Livediscourses.FindAsync(livediscourse.ParentId);
        //        ViewBag.parent = parent;

        //    }
        //    ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", livediscourse.Sector);
        //    ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", livediscourse.Region);
        //    return View(livediscourse);
        //}
        //[Authorize]
        //public async Task<ActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Livediscourse livediscourse = await _db.Livediscourses.FindAsync(id);
        //    if (livediscourse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    TempData["AdminCheck"] = livediscourse.AdminCheck;
        //    var user = userManager.GetUserId(User);
        //    var isModerator = await IsModerator(livediscourse.ParentId, user);
        //    ViewBag.IsModerator = isModerator;

        //    ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", livediscourse.Sector);
        //    ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", livediscourse.Region);
        //    if (livediscourse.Creator == user || isModerator == true || User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
        //    {
        //        return View(livediscourse);
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }

        //}

        //// POST: DevNews/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Title,SubTitle,Description,Sector,ImageUrl,ParentId,ImageCopyright,Author,ImageCaption,Tags,AdminCheck,Region,Country,Creator,LivediscourseIndex,IsPublic,CreatedOn,ModifiedOn,PublishedOn,ViewCount,LikeCount,Close_Date,Status")] Livediscourse livediscourse, string ChooseImage, List<string> FileTitle, List<string> FilePath, List<string> MimeType, List<string> FileVideoSize, List<long> FileDelete, List<string> FileCaption, List<string> FileThumbUrl, List<string> FileDuration)
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
        //                var actName = Path.GetFileNameWithoutExtension(file.FileName);
        //                string mimeType = MimeMapping.GetMimeMapping(file.FileName);
        //                string fileSize = file.ContentLength.ToString();
        //                var fileKey = Request.Files.Keys[i];

        //                if (fileKey == "ImageUrlUpdate")
        //                {
        //                    CloudBlobContainer blobContainer = GetCloudBlobContainer();
        //                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
        //                    blob.UploadFromStream(file.InputStream);
        //                    livediscourse.ImageUrl = blob.Uri.ToString();
        //                    // Saved Image in Image Gallery
        //                    string imgcopyright = "";
        //                    if (!string.IsNullOrEmpty(livediscourse.ImageCopyright))
        //                    {
        //                        imgcopyright = livediscourse.ImageCopyright.Replace("Image Credit: ", "") ?? "";
        //                    }
        //                    ImageGalleries fileobj = new ImageGalleries()
        //                    {
        //                        Title = actName,
        //                        ImageUrl = livediscourse.ImageUrl,
        //                        ImageCopyright = imgcopyright,
        //                        Caption = "",
        //                        FileMimeType = mimeType,
        //                        FileSize = fileSize,
        //                        Sector = livediscourse.Sector,
        //                        Tags = "",
        //                        UseCount = 1,
        //                    };
        //                    _db.ImageGalleries.Add(fileobj);
        //                    await _db.SaveChangesAsync();
        //                }
        //            }
        //        }
        //        if (!String.IsNullOrEmpty(ChooseImage))
        //        {
        //            livediscourse.ImageUrl = ChooseImage;
        //            // Find Image in Old Image Gallery
        //            var findimage = await _db.UserFiles.FirstOrDefaultAsync(a => a.FileUrl == ChooseImage);
        //            if (findimage != null)
        //            {
        //                // Saved Image in New Image Gallery
        //                string imgcopyright = "";
        //                if (!string.IsNullOrEmpty(livediscourse.ImageCopyright))
        //                {
        //                    imgcopyright = livediscourse.ImageCopyright.Replace("Image Credit: ", "") ?? "";
        //                }
        //                ImageGalleries fileobj = new ImageGalleries()
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
        //                _db.ImageGalleries.Add(fileobj);
        //                await _db.SaveChangesAsync();
        //                // Remove from old gallery
        //                _db.UserFiles.Remove(findimage);
        //                await _db.SaveChangesAsync();
        //            }
        //        }
        //        else if ((String.IsNullOrEmpty(livediscourse.ImageUrl) || livediscourse.ImageUrl == "/images/defaultImage.jpg") && !String.IsNullOrEmpty(livediscourse.Sector))
        //        {
        //            var sec = livediscourse.Sector.Split(',')[0];
        //            livediscourse.ImageUrl = "/images/sector/all_sectors.jpg"; // SelectDefaultImage(sec);
        //        }
        //        if (TempData["AdminCheck"].ToString() == "False" && livediscourse.AdminCheck == true)
        //        {
        //            livediscourse.PublishedOn = DateTime.UtcNow;
        //        }
        //        livediscourse.ModifiedOn = DateTime.UtcNow;
        //        livediscourse.Region = livediscourse.Region ?? "";
        //        livediscourse.Country = livediscourse.Country ?? "";
        //        _db.Entry(livediscourse).State = EntityState.Modified;
        //        await _db.SaveChangesAsync();
        //        if (FileTitle != null)
        //        {
        //            foreach (var item in FileTitle)
        //            {
        //                var index = FileTitle.IndexOf(item);
        //                var filepath = FilePath[index];
        //                var mimetype = MimeType[index];
        //                var fileSize = FileVideoSize[index];
        //                var caption = FileCaption[index];
        //                var thumbUrl = FileThumbUrl[index];
        //                var fDuration = FileDuration[index];
        //                var newsid = livediscourse.Id;

        //                LivediscourseVideo obj = new LivediscourseVideo();
        //                obj.Title = item;
        //                obj.FilePath = filepath;
        //                obj.FileMimeType = mimetype;
        //                obj.FileSize = fileSize;
        //                obj.FileCaption = caption;
        //                obj.FileThumbUrl = thumbUrl;
        //                obj.Duration = fDuration;
        //                obj.LivediscourseId = newsid;
        //                _db.LivediscourseVideos.Add(obj);
        //                _db.SaveChanges();
        //            }
        //        }


        //        if (FileDelete != null)
        //        {
        //            foreach (var item in FileDelete)
        //            {
        //                LivediscourseVideo obj = _db.LivediscourseVideos.Find(item);
        //                _db.LivediscourseVideos.Remove(obj);
        //                _db.SaveChanges();
        //            }
        //        }
        //        if (livediscourse.ParentId == 0)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return RedirectToAction("Details", new { id = livediscourse.ParentId });
        //        }
        //    }
        //    ViewBag.id = livediscourse.ParentId;
        //    if (livediscourse.ParentId != 0)
        //    {
        //        var parent = await _db.Livediscourses.FindAsync(livediscourse.ParentId);
        //        ViewBag.parent = parent;
        //        var user = userManager.GetUserId(User);
        //        ViewBag.IsModerator = await IsModerator(parent.Id, user);
        //    }
        //    ViewBag.Sector = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title", livediscourse.Sector);
        //    ViewBag.Region = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title", livediscourse.Region);
        //    return View(livediscourse);
        //}
        //[Authorize]
        //public async Task<ActionResult> Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Livediscourse livediscourse = await _db.Livediscourses.FindAsync(id);
        //    if (livediscourse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var user = userManager.GetUserId(User);
        //    ViewBag.IsModerator = await IsModerator(livediscourse.Id, user);
        //    return View(livediscourse);
        //}
        //public async Task<bool> IsModerator(long id, string user)
        //{
        //    var isModerator = await _db.FollowLivediscourses.AnyAsync(a => a.LivediscourseId == id && a.Livediscourses.IsPublic == true && a.FollowBy == user && a.IsModerator == true);
        //    return isModerator;
        //}
        //[Authorize(Roles = "SuperAdmin")]
        //public async Task<ActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Livediscourse livediscourse = await _db.Livediscourses.FindAsync(id);
        //    if (livediscourse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(livediscourse);
        //}

        //// POST: LiveBlogs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(long id)
        //{
        //    Livediscourse livediscourse = await _db.Livediscourses.FindAsync(id);
        //    _db.Livediscourses.Remove(livediscourse);
        //    await _db.SaveChangesAsync();
        //    if (livediscourse.ParentId == 0)
        //    {
        //        var searchChild = await _db.Livediscourses.Where(a => a.ParentId == livediscourse.Id).ToListAsync();
        //        if (searchChild.Any())
        //        {
        //            foreach (var child in searchChild)
        //            {
        //                Livediscourse childlivediscourse = await _db.Livediscourses.FindAsync(child.Id);
        //                _db.Livediscourses.Remove(childlivediscourse);
        //                await _db.SaveChangesAsync();
        //            }
        //        }
        //    }
        //    if (livediscourse.ParentId != 0)
        //    {
        //        return RedirectToAction("Details", "Livediscourse", new { id = livediscourse.ParentId });
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //public async Task<ActionResult> LivediscourseAmp(long? id)
        //{
        //    var search = await _db.Livediscourses.FirstOrDefaultAsync(a => a.Id == id);
        //    if (search != null)
        //    {
        //        var converter = new HtmlToAmpConverter();
        //        converter.WithSanitizers(
        //            new HashSet<ISanitizer>
        //            {
        //            new InstagramSanitizer(),
        //            new TwitterSanitizer(),
        //            new AudioSanitizer(),
        //            new HrefJavaScriptSanitizer(),
        //            new ImageSanitizer(),
        //            new JavaScriptRelatedAttributeSanitizer(),
        //            new StyleAttributeSanitizer(),
        //            new ScriptElementSanitizer(),
        //            new TargetAttributeSanitizer(),
        //            new XmlAttributeSanitizer(),
        //            new YouTubeVideoSanitizer(),
        //            new AmpIFrameSanitizer()
        //            });
        //        string ampHtml = converter.ConvertFromHtml(search.Description).AmpHtml;
        //        ViewBag.ampHtml = ampHtml;
        //    }
        //    else
        //    {
        //        return RedirectToAction("PageNotFound", "Error");
        //    }
        //    return View(search);
        //}

        //public PartialViewResult GetAmpLivediscourseUpdates(long id)
        //{
        //    var search = _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new LiveblogViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl, Description = s.Description, CreatedOn = s.CreatedOn }).ToList();
        //    return PartialView("AmpLivediscourseUpdates", search);
        //}

        ////public PartialViewResult GetDiscourseIndex(long id)
        ////{
        ////    var search = _db.DiscourseIndex.Where(a => a.LivediscourseId == id).OrderByDescending(o => o.CreatedOn).ToList();
        ////    return PartialView("_ampDiscourseIndex", search);
        ////}

        //public JsonResult GetDiscourseIndexItems(long id, string __amp_source_origin, int? moreItemsPageIndex)
        //{
        //    var search = _db.Livediscourses.Where(a => a.LivediscourseIndex == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(b => new DiscourseItemAmpViewModel { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn }).Take(3).ToList();
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
        //    }
        //    int pageSize = 10;
        //    int pageNumber = (moreItemsPageIndex ?? 1);
        //    var resultData = search.Select(b => new { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn.ToString(), Url = "/live-discourse" + "/" + b.GenerateSecondSlug().ToString() }).ToPagedList(pageNumber, pageSize);
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("devdiscourse_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("devnews");
            //if (container.CreateIfNotExists())
            //{
            //    container.SetPermissions(new BlobContainerPermissions
            //    {
            //        PublicAccess = BlobContainerPublicAccessType.Blob
            //    });
            //}
            return container;
        }
        public ActionResult Home()
        {
            string ? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            return View();
        }
        public async Task<ActionResult> Article(long? id)
        {
            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            Livediscourse livediscourse = await _db.Livediscourses.FindAsync(id);
            if (livediscourse == null)
            {
                return NotFound();
            }
            //if (!Request.Browser.Crawler)//do it later
            //{
            //    livediscourse.ViewCount = livediscourse.ViewCount + 1;
            //    _db.Entry(livediscourse).State = EntityState.Modified;
            //    await _db.SaveChangesAsync();
            //}
            if (User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserId(User);
                var searchFollow = await _db.FollowLivediscourses.FirstOrDefaultAsync(a => a.FollowBy == user && a.LivediscourseId == id);
                ViewBag.searchFollow = searchFollow;
                ViewBag.isModerstor = await IsModerator(long.Parse(id.ToString()), user);
            }
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
                    break;
            }
            var blogUpdates = await _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(m => m.CreatedOn).Take(10).ToListAsync();
            ViewBag.BlogUpdates = blogUpdates;
            ViewBag.FirstBlogUpdates = blogUpdates.Take(1);
            ViewBag.HasUpdates = blogUpdates.Any();
            return View(livediscourse);
        }
        public async Task<bool> IsModerator(long id, string user)
        {
            var isModerator = await _db.FollowLivediscourses.AnyAsync(a => a.LivediscourseId == id && a.Livediscourses.IsPublic == true && a.FollowBy == user && a.IsModerator == true);
            return isModerator;
        }
        //public async Task<ActionResult> MobileArticle(long? id, string user)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("PageNotFound", "Error");
        //    }
        //    var livediscourse = await _db.Livediscourses.FirstOrDefaultAsync(a => a.Id == id);
        //    if (livediscourse != null)
        //    {
        //        string description = Regex.Replace(livediscourse.Description, "style[^>]*", "");
        //        description = Regex.Replace(description, "<img", "<amp-img layout='responsive'");
        //        description = Regex.Replace(description, "<iframe", "<amp-iframe layout='responsive' sandbox=\"allow-scripts allow-same-origin\"");
        //        description = Regex.Replace(description, "width=\"100%\"", "width=\"300\"");
        //        description = Regex.Replace(description, "height=\"480\"", "height=\"200\"");
        //        ViewBag.AmpDescriptoion = description;
        //        if (!Request.Browser.Crawler)
        //        {
        //            livediscourse.ViewCount = livediscourse.ViewCount + 1;
        //            _db.Entry(livediscourse).State = EntityState.Modified;
        //            await _db.SaveChangesAsync();
        //        }
        //        var blogUpdates = await _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).Take(10).ToListAsync();
        //        ViewBag.BlogUpdates = blogUpdates;
        //    }
        //    else
        //    {
        //        return RedirectToAction("PageNotFound", "Error");
        //    }
        //    return View(livediscourse);
        //}
        ////public PartialViewResult GetLiveBlogs(long id)
        ////{
        ////    if (User.Identity.IsAuthenticated)
        ////    {
        ////        string user = userManager.GetUserId(User);
        ////        var search = _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        ////        {
        ////            Title = s.Title,
        ////            Description = s.Description,
        ////            Id = s.Id,
        ////            CreatedOn = s.CreatedOn,
        ////            PublishedOn = s.PublishedOn,
        ////            ImageUrl = s.ImageUrl,
        ////            ImageCopyright = s.ImageCopyright,
        ////            ModifiedOn = s.ModifiedOn,
        ////            DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        ////            CommentCount = s.CommentCount,
        ////            LikeCount = s.LikeCount,
        ////            DislikeCount = s.DislikeCount,
        ////            Tags = s.Tags,
        ////            ParentStoryLink = s.ParentStoryLink,
        ////            Reacted = _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog) == null ? ReactType.None : _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog).ReactType
        ////        }).Take(10).ToList();
        ////        return PartialView("_getLiveBlogs", search);
        ////    }
        ////    else
        ////    {
        ////        var search = _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        ////        {
        ////            Title = s.Title,
        ////            Description = s.Description,
        ////            Id = s.Id,
        ////            CreatedOn = s.CreatedOn,
        ////            ImageUrl = s.ImageUrl,
        ////            PublishedOn = s.PublishedOn,
        ////            ImageCopyright = s.ImageCopyright,
        ////            ModifiedOn = s.ModifiedOn,
        ////            DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        ////            CommentCount = s.CommentCount,
        ////            LikeCount = s.LikeCount,
        ////            DislikeCount = s.DislikeCount,
        ////            Tags = s.Tags,
        ////            ParentStoryLink = s.ParentStoryLink,
        ////            Reacted = ReactType.None
        ////        }).Take(10).ToList();
        ////        return PartialView("_getLiveBlogs", search);
        ////    }
        ////}
        //public PartialViewResult GetLivediscourseamp(long id)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        string user = userManager.GetUserId(User);
        //        var search = _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        //        {
        //            Title = s.Title,
        //            Description = s.Description,
        //            PublishedOn = s.PublishedOn,
        //            Id = s.Id,
        //            CreatedOn = s.CreatedOn,
        //            ImageUrl = s.ImageUrl,
        //            ImageCopyright = s.ImageCopyright,
        //            ModifiedOn = s.ModifiedOn,
        //            DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        //            CommentCount = s.CommentCount,
        //            LikeCount = s.LikeCount,
        //            DislikeCount = s.DislikeCount,
        //            Tags = s.Tags,
        //            Reacted = _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog) == null ? ReactType.None : _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog).ReactType
        //        }).ToList();
        //        return PartialView("_getLivediscourseamp", search);
        //    }
        //    else
        //    {
        //        var search = _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        //        {
        //            Title = s.Title,
        //            Description = s.Description,
        //            Id = s.Id,
        //            CreatedOn = s.CreatedOn,
        //            PublishedOn = s.PublishedOn,
        //            ImageUrl = s.ImageUrl,
        //            ImageCopyright = s.ImageCopyright,
        //            ModifiedOn = s.ModifiedOn,
        //            DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        //            CommentCount = s.CommentCount,
        //            LikeCount = s.LikeCount,
        //            DislikeCount = s.DislikeCount,
        //            Tags = s.Tags,
        //            Reacted = ReactType.None
        //        }).ToList();
        //        return PartialView("_getLivediscourseamp", search);
        //    }
        //}

        //public PartialViewResult GetPageLiveBlogs(long id, string pu, string cu, int page = 1)
        //{
        //    ViewBag.pageUrl = pu;
        //    ViewBag.creator = cu;
        //    var skip = (page - 1) * 10;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        string user = userManager.GetUserId(User);
        //        var search = (_db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        //        {
        //            Title = s.Title,
        //            Description = s.Description,
        //            Id = s.Id,
        //            CreatedOn = s.CreatedOn,
        //            ImageUrl = s.ImageUrl,
        //            ImageCopyright = s.ImageCopyright,
        //            ModifiedOn = s.ModifiedOn,
        //            DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        //            CommentCount = s.CommentCount,
        //            LikeCount = s.LikeCount,
        //            DislikeCount = s.DislikeCount,
        //            Tags = s.Tags,
        //            ParentStoryLink = s.ParentStoryLink,
        //            Reacted = _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog) == null ? ReactType.None : _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog).ReactType
        //        })).Skip(skip).Take(10).ToList();
        //        return PartialView("_getPageLiveBlogs", search);
        //    }
        //    else
        //    {
        //        var search = (_db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
        //        {
        //            Title = s.Title,
        //            Description = s.Description,
        //            Id = s.Id,
        //            CreatedOn = s.CreatedOn,
        //            ImageUrl = s.ImageUrl,
        //            ImageCopyright = s.ImageCopyright,
        //            ModifiedOn = s.ModifiedOn,
        //            DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
        //            CommentCount = s.CommentCount,
        //            LikeCount = s.LikeCount,
        //            DislikeCount = s.DislikeCount,
        //            Tags = s.Tags,
        //            ParentStoryLink = s.ParentStoryLink,
        //            Reacted = ReactType.None
        //        })).Skip(skip).Take(10).ToList();
        //        return PartialView("_getPageLiveBlogs", search);
        //    }
        //}
        //public JsonResult SaveDiscourseComment(long itemId, string commentText, long parentId, long rootParentId, string reply)
        //{
        //    var loginUser = "";
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        loginUser = "16eacbdc-8d99-4633-8aa0-7d100d972211";
        //        //return Json("NotAuthorized", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        loginUser = userManager.GetUserId(User);
        //    }
        //    DiscourseComment discourseComment = new DiscourseComment()
        //    {
        //        ItemId = itemId,
        //        CommentText = commentText,
        //        CommentBy = loginUser,
        //        CommentOn = DateTime.UtcNow,
        //        RootParentId = rootParentId,
        //        ReplyText = reply,
        //        ParentId = parentId
        //    };
        //    _db.DiscourseComments.Add(discourseComment);
        //    _db.SaveChanges();
        //    if (parentId != 0)
        //    {
        //        UpdateChildCommentCount(parentId);
        //    }
        //    else
        //    {
        //        UpdateCommentCount(itemId);
        //    }
        //    var comment = _db.DiscourseComments.Where(a => a.CommentId == discourseComment.CommentId && a.IsHidden == false).Select(a => new { name = a.ApplicationUser.FirstName + " " + a.ApplicationUser.LastName, commentText = a.CommentText, parentId = a.ParentId, commentId = a.CommentId, itemId = a.ItemId, isHidden = a.IsHidden, childCount = a.ChildCount, rootParentId = a.RootParentId, replyText = a.ReplyText });
        //    return Json(comment, JsonRequestBehavior.AllowGet);
        //}
        //public void UpdateChildCommentCount(long parentId)
        //{
        //    var discourseComment = _db.DiscourseComments.Find(parentId);
        //    if (discourseComment != null)
        //    {
        //        discourseComment.ChildCount = _db.DiscourseComments.Count(a => a.ParentId == parentId && a.IsHidden == false);
        //        _db.Entry(discourseComment).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }
        //}
        //public void UpdateCommentCount(long id)
        //{
        //    var discourse = _db.Livediscourses.Find(id);
        //    if (discourse != null)
        //    {
        //        discourse.CommentCount = _db.DiscourseComments.Count(a => a.ItemId == id && a.ParentId == 0 && a.IsHidden == false);
        //        _db.Entry(discourse).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }
        //}
        //public JsonResult RemoveComment(long id)
        //{
        //    var discourseComment = _db.DiscourseComments.Find(id);
        //    long parentId = 0;
        //    long itemId = 0;
        //    if (discourseComment != null)
        //    {
        //        parentId = discourseComment.ParentId;
        //        itemId = discourseComment.ItemId;
        //        discourseComment.IsHidden = true;
        //        _db.Entry(discourseComment).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }
        //    if (parentId != 0)
        //    {
        //        UpdateChildCommentCount(parentId);
        //    }
        //    else if (itemId != 0)
        //    {
        //        UpdateCommentCount(itemId);
        //    }
        //    return Json(discourseComment.ItemId, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult DiscourseFollower(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ViewBag.id = id;
        //    return View();
        //}
        //public string RandomName()
        //{
        //    var time = DateTime.UtcNow.ToLocalTime();
        //    return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        //}
        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        ////public System.Web.Mvc.PartialViewResult GetInfocusLiveDiscourse(string reg)
        ////{
        ////    //reg = reg == "Global Edition" ? "" : reg;
        ////    //var InfocusLiveDiscourse = (from m in _db.Livediscourses
        ////    //                            where m.Region.Contains(reg) && m.ParentId == 0 && m.AdminCheck == true
        ////    //                            orderby m.ModifiedOn descending
        ////    //                            select new DiscourseViewModel
        ////    //                            {
        ////    //                                Id = m.Id,
        ////    //                                Title = m.Title,
        ////    //                                ImageUrl = m.ImageUrl,
        ////    //                                children = _db.Livediscourses.Where(a => a.ParentId == m.Id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new DiscourseChildViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl }).Take(2).ToList()
        ////    //                            }).Take(3);
        ////    //return PartialView("_getInfocusLiveDiscourse", InfocusLiveDiscourse);
        ////    reg = reg == "Global Edition" ? "Universal Edition" : reg;
        ////    var InfocusLiveDiscourse = (from m in _db.LiveDiscourseInfocus
        ////                                where m.Edition == reg
        ////                                join s in _db.Livediscourses on m.LivediscourseId equals s.Id
        ////                                where s.AdminCheck == true && s.ParentId == 0
        ////                                orderby m.SrNo
        ////                                select new DiscourseViewModel
        ////                                {
        ////                                    Id = s.Id,
        ////                                    Title = s.Title,
        ////                                    ImageUrl = s.ImageUrl,
        ////                                    SrNo = m.SrNo,
        ////                                    children = _db.Livediscourses.Where(a => a.ParentId == s.Id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new DiscourseChildViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl }).Take(2).ToList()
        ////                                }).AsNoTracking().Take(3);
        ////    return PartialView("_getInfocusLiveDiscourse", InfocusLiveDiscourse);
        ////}
        //[Authorize]
        //public async Task<JsonResult> CreateLivediscourseInfocus(string editions, long newsId)
        //{
        //    string result = "";
        //    var regions = editions.Split(',').ToList();
        //    foreach (var edition in regions)
        //    {
        //        var newobj = _db.LiveDiscourseInfocus.FirstOrDefault(a => a.LivediscourseId == newsId && a.Edition == edition);
        //        if (newobj == null)
        //        {
        //            var dataForIncement = _db.LiveDiscourseInfocus.Where(a => a.Edition == edition);
        //            if (dataForIncement.Any())
        //            {
        //                foreach (var item in dataForIncement.ToList())
        //                {
        //                    item.SrNo = item.SrNo + 1;
        //                    _db.Entry(item).State = EntityState.Modified;
        //                }
        //                await _db.SaveChangesAsync();
        //            }
        //            LiveDiscourseInfocus obj = new LiveDiscourseInfocus()
        //            {
        //                LivediscourseId = newsId,
        //                SrNo = 1,
        //                ItemType = "",
        //                Edition = edition,
        //                Creator = userManager.GetUserId(User),
        //                CreatedOn = DateTime.UtcNow,
        //            };
        //            _db.LiveDiscourseInfocus.Add(obj);
        //            await _db.SaveChangesAsync();
        //            var livediscourseinfocusData = _db.LiveDiscourseInfocus.Where(a => a.SrNo > 5);
        //            if (livediscourseinfocusData.Any())
        //            {
        //                _db.LiveDiscourseInfocus.RemoveRange(livediscourseinfocusData);
        //                await _db.SaveChangesAsync();
        //            }
        //        }
        //        else
        //        {
        //            result = result != "" ? result + "," + edition : edition;
        //        }
        //    }
        //    return Json(result, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        //}
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
