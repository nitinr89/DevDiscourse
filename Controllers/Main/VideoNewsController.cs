
using System.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devdiscourse.Data;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Models.VideoNewsModels;
using X.PagedList;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;

namespace Devdiscourse.Controllers.Main
{
    public class VideoNewsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public VideoNewsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        // GET: VideoNews
        public ActionResult Index(Guid? region, int? sector = 0, int? page = 1, string label = "0", string country = "", string source = "", string text = "", bool editorPick = false)
        {
            ViewBag.label = label;
            ViewBag.sector = sector;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.source = source;
            ViewBag.editorPick = editorPick;
            DateTime twoMonth = DateTime.Today.AddDays(-120);
            IQueryable<VideoNews> videoNews;
            if (string.IsNullOrEmpty(text))
            {
                videoNews = db.VideoNews.Where(a => a.CreatedOn > twoMonth);
            }
            else
            {
                videoNews = db.VideoNews.Where(a => a.Title.Contains(text) && a.CreatedOn > twoMonth);
            }
            if (label != "0")
            {
                videoNews = videoNews.Where(a => a.Label == label);
            }
            if (sector != 0)
            {
                videoNews = videoNews.Where(a => a.VideoNewsSectors.Any(s => s.SectorId == sector));
            }
            if (region != new Guid() && region != null)
            {
                videoNews = videoNews.Where(a => a.VideoNewsRegions.Any(r => r.EditionId == region));
            }
            if (!string.IsNullOrEmpty(country))
            {
                videoNews = videoNews.Where(a => a.Country.Contains(country));
            }
            if (!string.IsNullOrEmpty(source))
            {
                videoNews = videoNews.Where(a => a.Source.Contains(source));
            }
            var result = videoNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10);
            return View(result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Create()
        {
            ViewBag.Sector = db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo).ToList();
            ViewBag.Label = new SelectList(db.Labels, "Slug", "Title");
            ViewBag.Region = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,AlternateHeadline,Description,Country,City,Source,Label,ViewCount,Creator,AdminCheck,EditorPick,VideoName,BlobName,VideoUrl,MimeType,Size,Caption,Copyright,VideoThumbUrl,Duration,Author,AuthorImage")] VideoNews videoNews, IFormFile? VideoThumbUrl, List<Guid> VideoEdition, List<int> VideoSector, List<string> Tags, string? AutoVideoThumbUrl)
        {

            if (ModelState.IsValid)
            {
                if (VideoThumbUrl != null && VideoThumbUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(VideoThumbUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(VideoThumbUrl.FileName);
                    string mimeType = GetMimeType(VideoThumbUrl.FileName);
                    string fileSize = VideoThumbUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await VideoThumbUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);
                        videoNews.VideoThumbUrl = blob.Uri.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(AutoVideoThumbUrl))
                {
                    videoNews.VideoThumbUrl = AutoVideoThumbUrl;
                }
                videoNews.AdminCheck = false;
                videoNews.Author = "";
                videoNews.AuthorImage = "";
                videoNews.ViewCount = 0;
                videoNews.Creator = userManager.GetUserId(User);
                db.VideoNews.Add(videoNews);
                db.SaveChanges();

                if (VideoSector != null)
                {
                    List<VideoNewsSector> VideoNewsSectors = new List<VideoNewsSector>();
                    foreach (var sector in VideoSector)
                    {
                        VideoNewsSectors.Add(new VideoNewsSector
                        {
                            SectorId = sector,
                            VideoNewsId = videoNews.Id
                        });
                    }
                    db.VideoNewsSectors.AddRange(VideoNewsSectors);
                    db.SaveChanges();
                }
                if (VideoEdition != null)
                {
                    List<VideoNewsRegion> VideoNewsRegions = new List<VideoNewsRegion>();
                    foreach (var edition in VideoEdition)
                    {
                        VideoNewsRegions.Add(new VideoNewsRegion
                        {
                            EditionId = edition,
                            VideoNewsId = videoNews.Id
                        });
                    }
                    db.VideoNewsRegions.AddRange(VideoNewsRegions);
                    db.SaveChanges();
                }
                if (Tags != null)
                {
                    List<VideoNewsTag> VideoNewsTags = new List<VideoNewsTag>();
                    foreach (var tag in Tags)
                    {
                        var tagId = GetTag(tag);
                        VideoNewsTags.Add(new VideoNewsTag
                        {
                            TagId = tagId,
                            VideoNewsId = videoNews.Id
                        });
                    }
                    db.VideoNewsTags.AddRange(VideoNewsTags);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.Sector = db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo).ToList();
            ViewBag.Label = new SelectList(db.Labels, "Slug", "Title");
            ViewBag.Region = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo).ToList();
            return View(videoNews);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var videoNews = db.VideoNews.Find(id);
            if (videoNews == null)
            {
                return NotFound();
            }
            ViewBag.Sector = db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo).ToList();
            ViewBag.Label = new SelectList(db.Labels, "Slug", "Title");
            ViewBag.Region = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo).ToList();
            return View(videoNews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,AlternateHeadline,Description,Country,City,Source,Label,ViewCount,Creator,AdminCheck,EditorPick,VideoName,BlobName,VideoUrl,MimeType,Size,Caption,Copyright,VideoThumbUrl,Duration,Author,AuthorImage,PublishedOn,ModifiedOn,CreatedOn")] VideoNews videoNews, IFormFile? UpdateVideoThumbUrl, List<Guid> VideoEdition, List<int> VideoSector, List<string> Tags, string? AutoVideoThumbUrl, List<long> DeleteTagList, List<Guid> OldVideoEdition, List<int> OldVideoSector)
        {

            if (ModelState.IsValid)
            {
                if (UpdateVideoThumbUrl != null && UpdateVideoThumbUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(UpdateVideoThumbUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(UpdateVideoThumbUrl.FileName);
                    string mimeType = GetMimeType(UpdateVideoThumbUrl.FileName);
                    string fileSize = UpdateVideoThumbUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await UpdateVideoThumbUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);
                        videoNews.VideoThumbUrl = blob.Uri.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(AutoVideoThumbUrl))
                {
                    videoNews.VideoThumbUrl = AutoVideoThumbUrl;
                }
                if (videoNews.PublishedOn == null && videoNews.AdminCheck == true)
                {
                    videoNews.PublishedOn = DateTime.UtcNow;
                }
                videoNews.ModifiedOn = DateTime.UtcNow;
                db.Entry(videoNews).State = EntityState.Modified;
                db.Entry(videoNews).Property(x => x.ViewCount).IsModified = false;
                db.SaveChanges();
                OldVideoEdition = OldVideoEdition == null ? new List<Guid>() : OldVideoEdition;
                VideoEdition = VideoEdition == null ? new List<Guid>() : VideoEdition;
                OldVideoSector = OldVideoSector == null ? new List<int>() : OldVideoSector;
                VideoSector = VideoSector == null ? new List<int>() : VideoSector;
                if (VideoSector.Count() > 0 || OldVideoSector.Count() > 0)
                {
                    List<VideoNewsSector> VideoNewsSectors = new List<VideoNewsSector>();
                    foreach (var sector in VideoSector.Where(s => !OldVideoSector.Any(o => o == s)))
                    {
                        VideoNewsSectors.Add(new VideoNewsSector
                        {
                            SectorId = sector,
                            VideoNewsId = videoNews.Id
                        });
                    }
                    if (VideoNewsSectors.Any())
                    {
                        db.VideoNewsSectors.AddRange(VideoNewsSectors);
                        db.SaveChanges();
                    }
                    var deletedSector = OldVideoSector.Where(s => !VideoSector.Any(o => o == s));
                    var removeSectorList = db.VideoNewsSectors
                                           .Where(m => m.VideoNewsId == videoNews.Id && deletedSector.Contains(m.SectorId))
                                           .ToList();
                    if (removeSectorList.Count > 0)
                    {
                        db.VideoNewsSectors.RemoveRange(removeSectorList);
                        db.SaveChanges();
                    }
                }
                if (VideoEdition.Count() > 0 || OldVideoEdition.Count() > 0)
                {
                    List<VideoNewsRegion> VideoNewsRegions = new List<VideoNewsRegion>();
                    foreach (var edition in VideoEdition.Where(s => !OldVideoEdition.Any(o => o == s)))
                    {
                        VideoNewsRegions.Add(new VideoNewsRegion
                        {
                            EditionId = edition,
                            VideoNewsId = videoNews.Id
                        });
                    }
                    if (VideoNewsRegions.Any())
                    {
                        db.VideoNewsRegions.AddRange(VideoNewsRegions);
                        db.SaveChanges();
                    }
                    var deletedEdition = OldVideoEdition.Where(s => !VideoEdition.Any(o => o == s));
                    var removeEditionList = db.VideoNewsRegions
                                           .Where(m => m.VideoNewsId == videoNews.Id && deletedEdition.Contains(m.EditionId))
                                           .ToList();
                    if (removeEditionList.Count > 0)
                    {
                        db.VideoNewsRegions.RemoveRange(removeEditionList);
                        db.SaveChanges();
                    }
                }
                if (Tags != null)
                {
                    List<VideoNewsTag> VideoNewsTags = new List<VideoNewsTag>();
                    foreach (var tag in Tags)
                    {
                        var tagId = GetTag(tag);
                        VideoNewsTags.Add(new VideoNewsTag
                        {
                            TagId = tagId,
                            VideoNewsId = videoNews.Id
                        });
                    }
                    db.VideoNewsTags.AddRange(VideoNewsTags);
                    db.SaveChanges();
                }
                if (DeleteTagList != null)
                {
                    var deleteTag = db.VideoNewsTags
                                           .Where(m => DeleteTagList.Contains(m.Id))
                                           .ToList();
                    if (deleteTag.Count > 0)
                    {
                        db.VideoNewsTags.RemoveRange(deleteTag);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.Sector = db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo).ToList();
            ViewBag.Label = new SelectList(db.Labels, "Slug", "Title");
            ViewBag.Region = db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo).ToList();
            return View(videoNews);
        }
        public long GetTag(string tag)
        {
            var search = db.Tagstb.Where(a => a.TagTitle == tag).SingleOrDefault();
            if (search != null)
            {
                return search.Id;
            }
            else
            {
                Tagstb tags = new Tagstb();
                tags.TagTitle = tag;
                db.Tagstb.Add(tags);
                db.SaveChanges();
                return tags.Id;
            }
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var videoNews = db.VideoNews.Find(id);
            if (videoNews == null)
            {
                return NotFound();
            }
            return View(videoNews);
        }

        // POST: DevNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var videoNews = db.VideoNews.Find(id);
            db.VideoNews.Remove(videoNews);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public PartialViewResult GetVideo(long id)
        {
            var videoNews = db.VideoNews.Find(id);
            return PartialView("_getVideo", videoNews);
        }
        private async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .Build();
            string connectionString = config.GetConnectionString("devdiscourse_AzureStorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("videoblob");
            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return container;
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
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }
        public ActionResult Embed(long id)
        {
            var videoNews = db.VideoNews.Find(id);
            if (videoNews == null)
            {
                return NotFound();
            }
            return View(new EmbedVideoViewModel { Id = videoNews.Id, VideoThumbUrl = videoNews.VideoThumbUrl, Title = videoNews.Title, VideoNewsTags = string.Join(", ", videoNews.VideoNewsTags.Select(a => a.Tagstb.TagTitle)) });
        }
        public ActionResult EmbedMedia(Guid id)
        {
            var videoNews = db.UserNewsFiles.Find(id);
            if (videoNews == null)
            {
                return NotFound();
            }
            return View(new EmbedVideoViewModel { FileId = videoNews.Id, Id = videoNews.DevNews.NewsId, VideoThumbUrl = videoNews.FileThumbUrl, Title = videoNews.DevNews.Title, VideoNewsTags = string.Join(", ", videoNews.DevNews.NewsTagstb.Select(a => a.Tagstb.TagTitle)) });
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

