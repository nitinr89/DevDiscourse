using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.Others;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel;

namespace DevDiscourse.Controllers.Others
{
    public class CommonEventController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommonEventController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: CommonEvent        
        public ActionResult Index(long? id, string prefix)
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
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var search = db.CommonEvents.Find(id);
            return View(search);
        }

        public JsonResult getEventRouteData(long? id, string prefix)
        {
            var obj = db.CommonEvents.Select(a => new EventViewModel { Id = a.Id, Tagline = a.EventTagline, Type = a.Type, Name = a.Name, CardUrl = a.CardUrl, StartDate = a.StartDate, EndDate = a.EndDate, Featured = a.Featured, AddressRegion = a.AddressRegion }).ToList();
            var resultData = from a in obj
                             select new
                             {
                                 type = a.Type,
                                 url = "/media-partner-event/" + a.GenerateSlug(),
                                 name = a.Name,
                                 tagline = a.Tagline ?? "",
                                 image = a.CardUrl,
                                 start = a.StartDate.ToString("MM-dd-yyyy"),
                                 end = a.EndDate.ToString("MM-dd-yyyy"),
                                 featured = a.Featured,
                                 location = a.AddressRegion
                             };
            return Json(resultData);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult GetEventsList()
        {
            var commonevents = db.CommonEvents;
            return View(commonevents.ToList());
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CommonEvent commonvent, IFormFile? LogoUrl, IFormFile? BackgroundBannerUrl, IFormFile? CardUrl)
        {
            if (ModelState.IsValid)
            {
                if (LogoUrl != null && LogoUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(LogoUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(LogoUrl.FileName);
                    string mimeType = GetMimeType(LogoUrl.FileName);
                    string fileSize = LogoUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await LogoUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        commonvent.LogoUrl = blob.Uri.ToString();
                    }
                }
                if (BackgroundBannerUrl != null && BackgroundBannerUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(BackgroundBannerUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(BackgroundBannerUrl.FileName);
                    string mimeType = GetMimeType(BackgroundBannerUrl.FileName);
                    string fileSize = BackgroundBannerUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await BackgroundBannerUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        commonvent.BackgroundBannerUrl = blob.Uri.ToString();
                    }
                }
                if (CardUrl != null && CardUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(CardUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(CardUrl.FileName);
                    string mimeType = GetMimeType(CardUrl.FileName);
                    string fileSize = CardUrl.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await CardUrl.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        commonvent.CardUrl = blob.Uri.ToString();
                    }
                }

                db.CommonEvents.Add(commonvent);
                db.SaveChanges();
                return RedirectToAction("GetEventsList");
            }
            return View(commonvent);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Edit(long id = 0)
        {
            CommonEvent? commonnevent = db.CommonEvents.Find(id);
            if (commonnevent == null)
            {
                return NotFound();
            }
            return View(commonnevent);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CommonEvent commonevent, IFormFile? LogoUrlUpdate, IFormFile? BannerUrlUpdate, IFormFile? CardUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                if (LogoUrlUpdate != null && LogoUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(LogoUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(LogoUrlUpdate.FileName);
                    string mimeType = GetMimeType(LogoUrlUpdate.FileName);
                    string fileSize = LogoUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await LogoUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        commonevent.LogoUrl = blob.Uri.ToString();
                    }
                }
                if (BannerUrlUpdate != null && BannerUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(BannerUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(BannerUrlUpdate.FileName);
                    string mimeType = GetMimeType(BannerUrlUpdate.FileName);
                    string fileSize = BannerUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await BannerUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        commonevent.BackgroundBannerUrl = blob.Uri.ToString();
                    }
                }
                if (CardUrlUpdate != null && CardUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(CardUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(CardUrlUpdate.FileName);
                    string mimeType = GetMimeType(CardUrlUpdate.FileName);
                    string fileSize = CardUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await CardUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        commonevent.CardUrl = blob.Uri.ToString();
                    }
                }

                db.CommonEvents.Update(commonevent);
                db.SaveChanges();
                return RedirectToAction("GetEventsList");
            }
            return View(commonevent);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(long id = 0)
        {
            CommonEvent? commonevent = db.CommonEvents.Find(id);
            var search = db.EventNavLinks.Where(a => a.EventId == id).ToList();
            ViewBag.NavLinks = search;
            if (commonevent == null)
            {
                return NotFound();
            }

            return View(commonevent);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Delete(long id = 0)
        {
            CommonEvent? commonevent = db.CommonEvents.Find(id);
            if (commonevent == null)
            {
                return NotFound();
            }

            return View(commonevent);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            CommonEvent? commonevent = db.CommonEvents.Find(id);
            if (commonevent == null)
            {
                return NotFound();
            }
            db.CommonEvents.Remove(commonevent);
            db.SaveChanges();
            return RedirectToAction("GetEventsList");
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
            CloudBlobContainer container = blobClient.GetContainerReference("category");

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