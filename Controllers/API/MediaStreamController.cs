using Devdiscourse.Data;
using Devdiscourse.Hubs;
using Devdiscourse.Models;
using Devdiscourse.Models.VideoNewsModels;
using Devdiscourse.Utility;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using NReco.VideoConverter;
using System.Net;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace DevDiscourse.Controllers.API
{
    public class MediaStreamController : Controller, IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        public MediaStreamController(ApplicationDbContext db, IWebHostEnvironment _environment)
        {
            this.db = db;
            this._environment = _environment;
        }

        [Route("api/MediaStream/GetVideoContent/{id}")]
        public IActionResult GetVideoContent(Guid id)
        {
            var search = db.UserNewsFiles.Find(id);
            if (search == null)
            {
                return NotFound(); // Handle the case where the item with the given id is not found
            }
            var filePath = "path/to/your/video.mp4"; // Replace with the actual path to your video file
            var fileInfo = new FileInfo(filePath);
            var streamer = new VideoStream(search.FileCaption, "imagegallery");
            var response = new FileStreamResult(new FileStream(filePath, FileMode.Open), "video/mp4")
            {
                FileDownloadName = search.FileCaption,
                EnableRangeProcessing = true
            };
            var rangeHeader = Request.Headers[HeaderNames.Range];
            if (!StringValues.IsNullOrEmpty(rangeHeader))
            {
                var rangeHeaderString = rangeHeader.ToString();
                var ranges = RangeHeaderValue.Parse(rangeHeaderString);
                var range = ranges.Ranges.FirstOrDefault();
                if (range != null)
                {
                    streamer.Start = range.From ?? 0;
                    streamer.End = range.To ?? fileInfo.Length - 1;

                    response.FileStream.Seek(streamer.Start, SeekOrigin.Begin);
                    response.FileStream.SetLength(streamer.End - streamer.Start + 1);

                    HttpContext.Response.StatusCode = 206;
                    HttpContext.Response.Headers.Add("Content-Range", $"bytes {streamer.Start}-{streamer.End}/{fileInfo.Length}");
                }
            }
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
            return response;
        }
        [Route("api/MediaStream/GetVideoStream/{id}")]
        public IActionResult GetVideoStream(long id)
        {
            var search = db.VideoNews.Find(id);
            if (search == null)
            {
                return NotFound(); // Handle the case where the item with the given id is not found
            }
            var filePath = "path/to/your/video.mp4"; // Replace with the actual path to your video file
            var fileInfo = new FileInfo(filePath);
            var streamer = new VideoStream(search.BlobName, "imagegallery");
            var response = new FileStreamResult(new FileStream(filePath, FileMode.Open), "video/mp4")
            {
                FileDownloadName = search.BlobName,
                EnableRangeProcessing = true
            };
            var rangeHeader = Request.Headers[HeaderNames.Range];
            if (!StringValues.IsNullOrEmpty(rangeHeader))
            {
                var rangeHeaderString = rangeHeader.ToString();
                var ranges = RangeHeaderValue.Parse(rangeHeaderString);
                var range = ranges.Ranges.FirstOrDefault();
                if (range != null)
                {
                    streamer.Start = range.From ?? 0;
                    streamer.End = range.To ?? fileInfo.Length - 1;

                    response.FileStream.Seek(streamer.Start, SeekOrigin.Begin);
                    response.FileStream.SetLength(streamer.End - streamer.Start + 1);

                    HttpContext.Response.StatusCode = 206;
                    HttpContext.Response.Headers.Add("Content-Range", $"bytes {streamer.Start}-{streamer.End}/{fileInfo.Length}");
                }
            }
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
            return response;
        }
        [Route("api/MediaStream/UploadMedia")]
        [HttpPost]
        public async Task<IActionResult> UploadMedia()
        {
            var signalConnectionId = HttpContext.Request.Form["signalConnectionId"];

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                string mimeType = "";
                string fileCaption = "";
                string fileSize = "";
                string fname = "";
                string furl = "";
                string fThumbUrl = "";
                var headline = "";
                var tags = "";
                for (var i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[i];
                    if (file == null || file.Length <= 0) continue;
                    var fileName = RandomName();

                    fname = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);
                    mimeType = GetMimeType(file.FileName);
                    fileSize = file.Length.ToString();
                    fileCaption = fileName + ".mp4";

                    string fileUrl = Path.Combine(_environment.WebRootPath, "images", fileName + fileExtension);
                    using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    string outputfileUrl = Path.Combine(_environment.WebRootPath, "images", "output", fileName + ".mp4");
                    string outputThumbUrl = Path.Combine(_environment.WebRootPath, "images", "output", fileName + ".jpg");
                    string inputExtension = fileExtension.Replace(".", "");
                    var ffMpeg = new FFMpegConverter();
                    ConvertSettings convertSettings = new ConvertSettings()
                    {
                        VideoFrameSize = FrameSize.hd720,
                        VideoFrameRate = 24,
                    };
                    //var context = GlobalHost.ConnectionManager.GetHubContext<FileHub>();
                    //context.Clients.All.SendFileProgress(signalConnectionId, "0");
                    ffMpeg.ConvertProgress += (o, args) =>
                    {
                        var conversionProgress = (String.Format("Progress: {0:HH:mm:ss}/{1:HH:mm:ss}", new DateTime(args.Processed.Ticks), new DateTime(args.TotalDuration.Ticks)));
                        var currentProgress = (int)((args.Processed.TotalSeconds / args.TotalDuration.TotalSeconds) * 100);
                        //context.Clients.All.SendFileProgress(signalConnectionId, conversionProgress);
                    };
                    ffMpeg.ConvertMedia(fileUrl, inputExtension, outputfileUrl, Format.mp4, convertSettings);
                    ffMpeg.GetVideoThumbnail(fileUrl, outputThumbUrl);

                    //file upload to blob
                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        blobContainer = await GetCloudBlobImageContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + ".mp4");
                        await blob.UploadFromStreamAsync(System.IO.File.OpenRead(outputfileUrl));
                        furl = blob.Uri.ToString();
                    }
                    CloudBlockBlob blobThumb;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        blobContainer = await GetCloudBlobImageContainer();
                        blobThumb = blobContainer.GetBlockBlobReference(fileName + ".jpg");
                        await blobThumb.UploadFromStreamAsync(System.IO.File.OpenRead(outputThumbUrl));
                        fThumbUrl = blobThumb.Uri.ToString();
                    }

                    //furl = "/images/output/" + fileName + ".mp4";
                    //IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(fileUrl);
                    //foreach (var directory in directories.Where(a => a.Name == "IPTC"))
                    //{
                    //    foreach (var tag in directory.Tags)
                    //    {
                    //        if (tag.Name == "Headline" || tag.Name == "headline")
                    //        {
                    //            headline = tag.Description;
                    //        }
                    //        if (tag.Name == "Keywords" || tag.Name == "keywords")
                    //        {
                    //            tags = tag.Description;
                    //        }
                    //    }
                    //}
                    if (System.IO.File.Exists(fileUrl))
                    {
                        System.IO.File.Delete(fileUrl);
                    }
                    //if (System.IO.File.Exists(outputfileUrl))
                    //{
                    //    System.IO.File.Delete(outputfileUrl);
                    //}
                    //string imgUrl = blob.Uri.ToString();
                }
                if (headline != "")
                {
                    fname = headline;
                }
                var returnUrl = JsonConvert.SerializeObject(new { FileName = fname, FileUrl = furl, Tags = tags, Caption = fileCaption, MimeType = mimeType, FileSize = fileSize, FileThumbUrl = fThumbUrl });
                return Ok(returnUrl);
            }
            return Ok("Error");
        }
        [Route("api/MediaStream/UploadVideo")]
        [HttpPost]
        public async Task<IActionResult> UploadVideo()
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                string mimeType = "";
                string fileCaption = "";
                string fileSize = "";
                string fname = "";
                string furl = "";
                string fThumbUrl = "";
                for (var i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[i];
                    if (file == null || file.Length <= 0) continue;
                    var fileName = RandomName();
                    fname = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);
                    fileCaption = fileName + ".mp4";

                    string fileUrl = Path.Combine(_environment.WebRootPath, "images", fileName + fileExtension);
                    using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    string outputfileUrl = Path.Combine(_environment.WebRootPath, "images", "output", fileName + ".mp4");
                    string outputThumbUrl = Path.Combine(_environment.WebRootPath, "images", "output", fileName + ".jpg");
                    string inputExtension = fileExtension.Replace(".", "");
                    var ffMpeg = new FFMpegConverter();
                    ConvertSettings convertSettings = new ConvertSettings()
                    {
                        VideoFrameSize = FrameSize.hd720,
                        VideoFrameRate = 24,
                    };
                    //var context = GlobalHost.ConnectionManager.GetHubContext<FileHub>();

                    try
                    {
                        ffMpeg.ConvertMedia(fileUrl, inputExtension, outputfileUrl, Format.mp4, convertSettings);
                        ffMpeg.GetVideoThumbnail(fileUrl, outputThumbUrl);
                    }
                    catch (Exception ex)
                    {
                        // Log or print the exception details
                        Console.WriteLine(ex.ToString());
                        throw; // Rethrow the exception if needed
                    }

                    //file upload to blob
                    CloudBlobContainer blobContainer;
                    CloudBlockBlob blob;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        blobContainer = await GetCloudBlobImageContainer();
                        blob = blobContainer.GetBlockBlobReference(fileName + ".mp4");
                        await blob.UploadFromStreamAsync(System.IO.File.OpenRead(outputfileUrl));
                        furl = blob.Uri.ToString();
                    }
                    CloudBlockBlob blobThumb;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        blobContainer = await GetCloudBlobImageContainer();
                        blobThumb = blobContainer.GetBlockBlobReference(fileName + ".jpg");
                        await blobThumb.UploadFromStreamAsync(System.IO.File.OpenRead(outputThumbUrl));
                        fThumbUrl = blobThumb.Uri.ToString();
                    }

                    mimeType = "video/mp4";
                    await blob.FetchAttributesAsync();
                    fileSize = blob.Properties.Length.ToString();

                    if (System.IO.File.Exists(fileUrl))
                    {
                        System.IO.File.Delete(fileUrl);
                    }
                }
                var returnUrl = JsonConvert.SerializeObject(new { FileName = fname, FileUrl = furl, Caption = fileCaption, MimeType = mimeType, FileSize = fileSize, FileThumbUrl = fThumbUrl });
                return Ok(returnUrl);
            }
            return Ok("Error");
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
