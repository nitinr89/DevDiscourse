using Devdiscourse.Models.BasicModels;
using X.PagedList;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using MetadataExtractor;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;

namespace DevDiscourse.Controllers.Main
{
    public class UserFilesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        public UserFilesController(ApplicationDbContext db, IWebHostEnvironment _environment)
        {
            this.db = db;
            this._environment = _environment;
        }

        // GET: UserFiles
        public ActionResult Index(int? page = 1, string text = "")
        {
            ViewBag.text = text;
            ViewBag.page = page;
            var search = db.UserFiles.ToList();
            if (!String.IsNullOrEmpty(text))
            {
                search = search.Where(a => a.Title.ToUpper().Contains(text.ToUpper())).ToList();
            }
            return View(search.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 20));
        }

        // GET: UserFiles/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            UserFiles? userFiles = db.UserFiles.Find(id);
            if (userFiles == null)
            {
                return NotFound();
            }
            return View(userFiles);
        }

        // GET: UserFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,FileUrl,FileFor")] UserFiles userFiles, IFormFile? FileUrl)
        {
            if (ModelState.IsValid)
            {
                if (FileUrl != null && FileUrl.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(FileUrl.FileName);
                    var actName = Path.GetFileNameWithoutExtension(FileUrl.FileName);
                    string mimeType = GetMimeType(FileUrl.FileName);
                    string fileSize = FileUrl.Length.ToString();

                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "UserFiles", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileUrl.CopyToAsync(fileStream);
                    }

                    UserFiles newObj = new UserFiles();
                    newObj.FileUrl = "/AdminFiles/UserFiles/" + fileName + fileExtension;
                    newObj.FileMimeType = mimeType;
                    newObj.FileSize = fileSize;

                    newObj.Id = Guid.NewGuid();
                    if (string.IsNullOrEmpty(userFiles.Title))
                    {
                        newObj.Title = actName;
                    }
                    else
                    {
                        newObj.Title = userFiles.Title;
                    }
                    newObj.FileFor = userFiles.FileFor;
                    db.UserFiles.Add(newObj);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(userFiles);
        }
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }

        // GET: UserFiles/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            UserFiles? userFiles = db.UserFiles.Find(id);
            if (userFiles == null)
            {
                return NotFound();
            }
            return View(userFiles);
        }

        // POST: UserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,FileUrl,FileFor,CreatedOn")] UserFiles userFiles, IFormFile? FileUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                if (FileUrlUpdate != null && FileUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(FileUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(FileUrlUpdate.FileName);
                    string mimeType = GetMimeType(FileUrlUpdate.FileName);
                    string fileSize = FileUrlUpdate.Length.ToString();

                    var filePath = Path.Combine(_environment.WebRootPath, "AdminFiles", "UserFiles", fileName + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileUrlUpdate.CopyToAsync(fileStream);
                    }
                    userFiles.FileUrl = "/AdminFiles/UserFiles/" + fileName + fileExtension;
                    userFiles.FileMimeType = mimeType;
                    userFiles.FileSize = fileSize;
                }

                db.UserFiles.Update(userFiles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userFiles);
        }
        // GET: UserFiles/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(Guid? id, int? page = 1)
        {
            TempData["page"] = page;
            if (id == null)
            {
                return BadRequest();
            }
            UserFiles? userFiles = db.UserFiles.Find(id);
            if (userFiles == null)
            {
                return NotFound();
            }
            return View(userFiles);
        }

        // POST: UserFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            UserFiles? userFiles = db.UserFiles.Find(id);
            if (userFiles == null)
            {
                return NotFound();
            }
            db.UserFiles.Remove(userFiles);
            db.SaveChanges();
            int page = Int32.Parse(TempData["page"].ToString());
            return RedirectToAction("Index", new { page = page });
        }
        public async Task<JsonResult> UploadAuthorImage()
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                string fname = "";
                string furl = "";
                string mimeType = "";
                string fileSize = "";
                for (var i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[i];

                    if (file != null && file.Length > 0)
                    {
                        var fileName = RandomName();
                        var fileExtension = Path.GetExtension(file.FileName);
                        var actName = Path.GetFileNameWithoutExtension(file.FileName);
                        mimeType = GetMimeType(file.FileName);
                        fileSize = file.Length.ToString();

                        CloudBlobContainer blobContainer;
                        CloudBlockBlob blob;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            ms.Position = 0;

                            blobContainer = await GetCloudBlobImageContainer();
                            blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                            await blob.UploadFromStreamAsync(ms);
                            furl = blob.Uri.ToString();
                        }

                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = fname,
                            ImageUrl = furl,
                            ImageCopyright = "",
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = "0",
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGalleries.Add(fileobj);
                        db.SaveChanges();
                    }
                }
                return Json(furl);
            }
            return Json("Error");
        }
        public async Task<JsonResult> UploadFile()
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                string fname = "";
                string furl = "";
                var headline = "";
                var tags = "";
                var caption = "";
                string mimeType = "";
                string fileSize = "";

                for (var i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[i];

                    if (file != null && file.Length > 0)
                    {
                        var fileName = RandomName();
                        var fileExtension = Path.GetExtension(file.FileName);
                        var actName = Path.GetFileNameWithoutExtension(file.FileName);
                        mimeType = GetMimeType(file.FileName);
                        fileSize = file.Length.ToString();

                        CloudBlobContainer blobContainer;
                        CloudBlockBlob blob;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            ms.Position = 0;

                            blobContainer = await GetCloudBlobImageContainer();
                            blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                            await blob.UploadFromStreamAsync(ms);
                            furl = blob.Uri.ToString();
                        }
                        var fileUrl = Path.Combine(_environment.WebRootPath, "images", fileName + fileExtension);

                        using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(fileUrl);
                        foreach (var directory in directories.Where(a => a.Name == "IPTC"))
                            foreach (var tag in directory.Tags)
                            {
                                if (tag.Name == "Headline" || tag.Name == "headline")
                                {
                                    headline = tag.Description;
                                }
                                if (tag.Name.Contains("Caption"))
                                {
                                    caption = tag.Description;
                                }
                                if (tag.Name == "Keywords" || tag.Name == "keywords")
                                {
                                    tags = tag.Description;
                                }
                            }
                        if (System.IO.File.Exists(fileUrl))
                        {
                            System.IO.File.Delete(fileUrl);
                        }
                        string imgUrl = blob.Uri.ToString();
                    }
                    if (headline != "")
                    {
                        fname = headline;
                    }
                }
                var returnUrl = JsonConvert.SerializeObject(new { FileName = fname, FileUrl = furl, Tags = tags, Caption = caption, MimeType = mimeType, FileSize = fileSize });
                return Json(returnUrl);
            }
            return Json("Error");
        }
        public JsonResult SaveGalleryImage(string title, string url, string sector, string caption, string copyright, string tags, string mimetype, string filesize)
        {
            ImageGallery obj = new ImageGallery()
            {
                Title = title,
                ImageUrl = url,
                ImageCopyright = copyright,
                Caption = caption,
                FileSize = filesize,
                FileMimeType = mimetype,
                Sector = sector,
                UseCount = 0,
                Tags = tags
            };
            db.ImageGalleries.Add(obj);
            db.SaveChanges();
            return Json("Success");
        }
        public JsonResult GetImages(int skip)
        {
            var find = from m in db.DevNews
                       where m.ImageUrl != null && m.ImageUrl != "" && m.ImageUrl != "/images/defaultImage.jpg" && m.ImageUrl != "/images/sector/all_sectors.jpg" && m.ImageUrl != "/images/fifa/fifa-default.png"
                       select new
                       {
                           m.ImageUrl,
                           m.CreatedOn,
                           m.FileMimeType,
                           m.FileSize,
                           m.Sector
                       };
            foreach (var item in find.OrderBy(a => a.CreatedOn).Skip(skip).Take(5000))
            {
                UserFiles newobj = new UserFiles()
                {
                    Title = "",
                    FileUrl = item.ImageUrl,
                    FileFor = item.Sector,
                    FileMimeType = item.FileMimeType,
                    FileSize = item.FileSize,
                    CreatedOn = item.CreatedOn
                };
                db.UserFiles.Add(newobj);
            }
            db.SaveChanges();
            return Json("Ok");
        }
        [Authorize]
        public ActionResult Images(int page = 1, string text = "")
        {
            ViewBag.text = text;
            ViewBag.page = page;
            var search = from ig in db.ImageGalleries select ig;
            if (!String.IsNullOrWhiteSpace(text))
            {
                search = search.Where(a => a.Title.ToUpper().Contains(text.ToUpper()) || a.Caption.ToUpper().Contains(text.ToUpper()));
            }
            var result = search.OrderByDescending(a => a.CreatedOn).ToPagedList(page, 20);
            return View(result);
        }
        [Authorize]
        public ActionResult UploadImages()
        {
            return View();
        }
        public async Task<JsonResult> UploadImagesToBlog()
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                string fname = "";
                string furl = "";
                var headline = "";
                var tags = "";
                var caption = "";
                string mimeType = "";
                string fileSize = "";
                for (var i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[i];

                    if (file != null && file.Length > 0)
                    {
                        var fileName = RandomName();
                        var fileExtension = Path.GetExtension(file.FileName);
                        var actName = Path.GetFileNameWithoutExtension(file.FileName);
                        mimeType = GetMimeType(file.FileName);
                        fileSize = file.Length.ToString();

                        CloudBlobContainer blobContainer;
                        CloudBlockBlob blob;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            ms.Position = 0;

                            blobContainer = await GetCloudBlobImageContainer();
                            blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                            await blob.UploadFromStreamAsync(ms);
                            furl = blob.Uri.ToString();
                        }
                        var fileUrl = Path.Combine(_environment.WebRootPath, "images", fileName + fileExtension);

                        using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(fileUrl);
                        foreach (var directory in directories.Where(a => a.Name == "IPTC"))
                            foreach (var tag in directory.Tags)
                            {
                                if (tag.Name == "Headline" || tag.Name == "headline")
                                {
                                    headline = tag.Description;
                                }
                                if (tag.Name.Contains("Caption"))
                                {
                                    caption = tag.Description;
                                }
                                if (tag.Name == "Keywords" || tag.Name == "keywords")
                                {
                                    tags = tag.Description;
                                }
                            }
                        if (System.IO.File.Exists(fileUrl))
                        {
                            System.IO.File.Delete(fileUrl);
                        }
                        string imgUrl = blob.Uri.ToString();
                    }
                    if (headline != "")
                    {
                        fname = headline;
                    }
                }
                var returnUrl = JsonConvert.SerializeObject(new { FileName = fname, FileUrl = furl, Tags = tags, Caption = caption, MimeType = mimeType, FileSize = fileSize });
                return Json(returnUrl);
            }
            return Json("Error");
        }
        public JsonResult CreateImage(string title, string sector, string imageurl, string copyright, string caption, string tags, string mimeType, string fileSize)
        {
            ImageGallery obj = new ImageGallery()
            {
                Title = title,
                Sector = sector,
                ImageUrl = imageurl,
                ImageCopyright = copyright,
                Caption = caption,
                Tags = tags,
                FileSize = fileSize,
                FileMimeType = mimeType,
                UseCount = 0
            };
            db.ImageGalleries.Add(obj);
            db.SaveChanges();
            return Json("Success");
        }
        public ActionResult EditImage(Guid? id, int? page = 1)
        {
            TempData["page"] = page;
            if (id == null)
            {
                return BadRequest();
            }
            ImageGallery? imageGallery = db.ImageGalleries.Find(id);
            if (imageGallery == null)
            {
                return NotFound();
            }
            return View(imageGallery);
        }

        // POST: UserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditImage([Bind("Id,Title,ImageUrl,FileMimeType,FileSize,Sector,ImageCopyright,Caption,UseCount,Tags,CreatedOn")] ImageGallery imageGallery, IFormFile? ImageUrlUpdate)
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

                        blobContainer = await GetCloudBlobImageContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        imageGallery.ImageUrl = blob.Uri.ToString();
                        imageGallery.FileMimeType = mimeType;
                        imageGallery.FileSize = fileSize;
                    }
                }

                imageGallery.ModifiedOn = DateTime.UtcNow;
                db.ImageGalleries.Update(imageGallery);
                db.SaveChanges();
                int page = Int32.Parse(TempData["page"].ToString());
                return RedirectToAction("Images", new { page = page });
            }
            return View(imageGallery);
        }
        // GET: UserFiles/DeleteFile/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteFile(Guid? id, int? page = 1)
        {
            TempData["page"] = page;
            if (id == null)
            {
                return BadRequest();
            }
            ImageGallery? imageGallery = db.ImageGalleries.Find(id);
            if (imageGallery == null)
            {
                return NotFound();
            }
            return View(imageGallery);
        }

        // POST: UserFiles/DeleteFile/5
        [HttpPost, ActionName("DeleteFile")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFileConfirmed(Guid id)
        {
            ImageGallery? imageGallery = db.ImageGalleries.Find(id);
            if (imageGallery == null)
            {
                return NotFound();
            }
            db.ImageGalleries.Remove(imageGallery);
            db.SaveChanges();
            int page = Int32.Parse(TempData["page"].ToString());
            return RedirectToAction("Images", new { page = page });
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
