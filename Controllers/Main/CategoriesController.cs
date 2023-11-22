using X.PagedList;
using Devdiscourse.Models.BasicModels;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devdiscourse.Data;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure;

namespace DevDiscourse.Controllers.Main
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        public CategoriesController(ApplicationDbContext db, IWebHostEnvironment _environment)
        {
            this.db = db;
            this._environment = _environment;
        }


        // GET: Categories
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index(int? page = 1)
        {
            var category = db.Categories.ToList();
            return View(category.OrderByDescending(a => a.Id).ToPagedList((page ?? 1), 20));
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Category? category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //POST: Categories/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,SrNo,Title,Slug,IsActive,PageTitle,PageImage,BannerImage,PageDescription,Keywords")] Category category, IFormFile? PageImage, IFormFile? BannerImage)
        {
            if (ModelState.IsValid)
            {
                if (PageImage != null && PageImage.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(PageImage.FileName);
                    var actName = Path.GetFileNameWithoutExtension(PageImage.FileName);
                    string mimeType = GetMimeType(PageImage.FileName);
                    string fileSize = PageImage.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await PageImage.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        category.PageImage = blob.Uri.ToString();
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = category.PageImage,
                            ImageCopyright = "",
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = "",
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGallery.Add(fileobj);
                        db.SaveChanges();
                    }
                }
                if (BannerImage != null && BannerImage.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(BannerImage.FileName);
                    var actName = Path.GetFileNameWithoutExtension(BannerImage.FileName);
                    string mimeType = GetMimeType(BannerImage.FileName);
                    string fileSize = BannerImage.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await BannerImage.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        category.BannerImage = blob.Uri.ToString();
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = category.BannerImage,
                            ImageCopyright = "",
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = "",
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGallery.Add(fileobj);
                        db.SaveChanges();
                    }
                }

                //if (!String.IsNullOrEmpty(ChooseImage) && !String.IsNullOrEmpty(bannerimage))
                //{
                //    category.PageImage = ChooseImage;
                //    category.BannerImage = bannerimage;
                //    // Find Image in Old Image Gallery
                //    var findimage = db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
                //    var findImage = db.UserFiles.FirstOrDefault(a => a.FileUrl == bannerimage);
                //    if (findimage != null)
                //    {
                //        // Saved Image in New Image Gallery

                //        ImageGallery fileobj = new ImageGallery()
                //        {
                //            Title = findimage.Title,
                //            ImageUrl = findimage.FileUrl,
                //            ImageCopyright = "",
                //            Caption = "",
                //            FileMimeType = findimage.FileMimeType,
                //            FileSize = findimage.FileSize,
                //            Sector = findimage.FileFor,
                //            Tags = "",
                //            UseCount = 1,
                //        };
                //        db.ImageGallery.Add(fileobj);
                //        db.SaveChanges();
                //        // Remove from old gallery
                //        db.UserFiles.Remove(findimage);
                //        db.SaveChanges();
                //    }
                //    if (findImage != null)
                //    {
                //        // Saved Image in New Image Gallery

                //        ImageGallery fileobj = new ImageGallery()
                //        {
                //            Title = findImage.Title,
                //            ImageUrl = findImage.FileUrl,
                //            ImageCopyright = "",
                //            Caption = "",
                //            FileMimeType = findImage.FileMimeType,
                //            FileSize = findImage.FileSize,
                //            Sector = findImage.FileFor,
                //            Tags = "",
                //            UseCount = 1,
                //        };
                //        db.ImageGallery.Add(fileobj);
                //        db.SaveChanges();
                //        // Remove from old gallery
                //        db.UserFiles.Remove(findImage);
                //        db.SaveChanges();
                //    }
                //}
                //else 
                if (String.IsNullOrEmpty(category.PageImage) && String.IsNullOrEmpty(category.BannerImage))
                {
                    category.PageImage = "/images/sector/all_sectors.jpg";
                    category.BannerImage = "/images/sector/all_sectors.jpg";
                }

                category.Slug = ReturnSlug(category.Title);
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Category? category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,SrNo,Title,Slug,IsActive,PageTitle,PageImage,BannerImage,PageDescription,Keywords")] Category category, IFormFile? PageImageUrlUpdate, IFormFile? BannerImageUrlUpdate)
        {
            if (ModelState.IsValid)
            {
                if (PageImageUrlUpdate != null && PageImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(PageImageUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(PageImageUrlUpdate.FileName);
                    string mimeType = GetMimeType(PageImageUrlUpdate.FileName);
                    string fileSize = PageImageUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await PageImageUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        category.PageImage = blob.Uri.ToString();
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = category.PageImage,
                            ImageCopyright = "",
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = "",
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGallery.Add(fileobj);
                        db.SaveChanges();
                    }
                }
                if (BannerImageUrlUpdate != null && BannerImageUrlUpdate.Length > 0)
                {
                    var fileName = RandomName(); // method to generate a random name.
                    var fileExtension = Path.GetExtension(BannerImageUrlUpdate.FileName);
                    var actName = Path.GetFileNameWithoutExtension(BannerImageUrlUpdate.FileName);
                    string mimeType = GetMimeType(BannerImageUrlUpdate.FileName);
                    string fileSize = BannerImageUrlUpdate.Length.ToString();

                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await BannerImageUrlUpdate.CopyToAsync(ms);
                        ms.Position = 0;

                        blobContainer = await GetCloudBlobContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                        await blob.UploadFromStreamAsync(ms);

                        category.BannerImage = blob.Uri.ToString();
                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = actName,
                            ImageUrl = category.BannerImage,
                            ImageCopyright = "",
                            Caption = "",
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = "",
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGallery.Add(fileobj);
                        db.SaveChanges();
                    }
                }

                //if (!String.IsNullOrEmpty(ChooseImage) && !String.IsNullOrEmpty(bannerimage))
                //{
                //    category.PageImage = ChooseImage;
                //    category.BannerImage = bannerimage;
                //    // Find Image in Old Image Gallery
                //    var findimage = db.UserFiles.FirstOrDefault(a => a.FileUrl == ChooseImage);
                //    var findImage = db.UserFiles.FirstOrDefault(a => a.FileUrl == bannerimage);
                //    if (findimage != null)
                //    {
                //        // Saved Image in New Image Gallery

                //        ImageGallery fileobj = new ImageGallery()
                //        {
                //            Title = findimage.Title,
                //            ImageUrl = findimage.FileUrl,
                //            ImageCopyright = "",
                //            Caption = "",
                //            FileMimeType = findimage.FileMimeType,
                //            FileSize = findimage.FileSize,
                //            Sector = findimage.FileFor,
                //            Tags = "",
                //            UseCount = 1,
                //        };
                //        db.ImageGallery.Add(fileobj);
                //        db.SaveChanges();
                //        // Remove from old gallery
                //        db.UserFiles.Remove(findimage);
                //        db.SaveChanges();
                //    }
                //    if (findImage != null)
                //    {
                //        // Saved Image in New Image Gallery

                //        ImageGallery fileobj = new ImageGallery()
                //        {
                //            Title = findImage.Title,
                //            ImageUrl = findImage.FileUrl,
                //            ImageCopyright = "",
                //            Caption = "",
                //            FileMimeType = findImage.FileMimeType,
                //            FileSize = findImage.FileSize,
                //            Sector = findImage.FileFor,
                //            Tags = "",
                //            UseCount = 1,
                //        };
                //        db.ImageGallery.Add(fileobj);
                //        db.SaveChanges();
                //        // Remove from old gallery
                //        db.UserFiles.Remove(findImage);
                //        db.SaveChanges();
                //    }
                //}
                //else

                if (String.IsNullOrEmpty(category.PageImage) && String.IsNullOrEmpty(category.BannerImage))
                {
                    category.PageImage = "/images/sector/all_sectors.jpg";
                    category.BannerImage = "/images/sector/all_sectors.jpg";
                }

                category.Slug = ReturnSlug(category.Title);
                db.Categories.Update(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Category? category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category? category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult GetCategory()
        {
            var search = db.Categories.ToList().OrderBy(a => a.SrNo);
            return PartialView("_getCategory", search);
        }
        public PartialViewResult ShowCategory(string id)
        {
            var idList = id.Split(',').ToList();
            var search = db.Categories.Where(a => idList.Contains(a.Id.ToString())).ToList();
            return PartialView("_showCategory", search);
        }
        public string ReturnSlug(string title)
        {
            string str = RemoveAccent(title).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }
        private string RemoveAccent(string text)
        {
            //byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
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
