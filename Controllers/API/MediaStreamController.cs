using Devdiscourse.Data;
using Devdiscourse.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Net;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using System.Web.Http;
using NReco.VideoConverter;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Format = NReco.VideoConverter.Format;
using Microsoft.AspNet.SignalR;
using FrameSize = NReco.VideoConverter.FrameSize;
using Devdiscourse.Hubs;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using RangeHeaderValue = Microsoft.Net.Http.Headers.RangeHeaderValue;
using ContentRangeHeaderValue = System.Net.Http.Headers.ContentRangeHeaderValue;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;
using AngleSharp.Network.Default;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace Devdiscourse.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaStreamController : ControllerBase
    {
        public ApplicationDbContext db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHubContext<FileHub> _hubContext;
        private readonly IContentTypeProvider _contentType;

        public MediaStreamController(ApplicationDbContext _db, IWebHostEnvironment hostEnvironment,
            IHubContext<FileHub> hubContext, IContentTypeProvider contentType)
        {
            db = _db;
            _hostEnvironment = hostEnvironment;
            _hubContext = hubContext;
            _contentType = contentType;
        }
        //[Route("api/MediaStream/GetVideoContent/{id}")]
        //public HttpResponseMessage GetVideoContent(Guid id)
        //{
        //    var search = db.UserNewsFiles.Find(id);
        //    var streamer = new VideoStream(search.FileCaption, "imagegallery");
        //    //var response = Response.CreateResponse();
        //    var response = new HttpResponseMessage();
        //    response.Headers.AcceptRanges.Add("bytes");
        //    response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)streamer.WriteToStream, new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4"));
        //    RangeHeaderValue rangeHeader = Request.Headers.Range;
        //    //RangeHeaderValue rangeHeader = Request.Headers.Range;
        //    if (rangeHeader != null)
        //    {
        //        long totalLength = streamer.Length;
        //        var range = rangeHeader.Ranges.First();
        //        streamer.Start = range.From ?? 0;
        //        streamer.End = range.To ?? totalLength - 1;
        //        response.Content.Headers.ContentLength = streamer.End - streamer.Start + 1;
        //        response.Content.Headers.ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(streamer.Start, streamer.End, totalLength);
        //        response.StatusCode = HttpStatusCode.PartialContent;
        //    }
        //    else
        //    {
        //        response.StatusCode = HttpStatusCode.OK;
        //    }
        //    return response;
        //}
        [HttpGet]
        [Route("api/MediaStream/GetVideoStream/{id}")]
        public HttpResponseMessage GetVideoStream(long id)
        {
            var search = db.VideoNews.Find(id);
            var streamer = new VideoStream(search.BlobName, "imagegallery");
            HttpResponseMessage response =new HttpResponseMessage();
            response.Headers.AcceptRanges.Add("bytes");
            response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)streamer.WriteToStream, new MediaTypeHeaderValue("video/mp4"));
            //RangeHeaderValue rangeHeader = Request.Headers.Range;
            RangeHeaderValue rangeHeader = HttpContext.Request.GetTypedHeaders().Range;
            if (rangeHeader != null)
            {
                long totalLength = streamer.Length;
                var range = rangeHeader.Ranges.First();
                streamer.Start = range.From ?? 0;
                streamer.End = range.To ?? totalLength - 1;
                response.Content.Headers.ContentLength = streamer.End - streamer.Start + 1;
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(streamer.Start, streamer.End, totalLength);
                response.StatusCode = HttpStatusCode.PartialContent;
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
            }
            return response;
        }

        [Route("api/MediaStream/UploadMedia")]
        [HttpPost]
        public IHttpActionResult UploadMedia()
        {
            //var httpRequest = HttpContext.Current.Request;
            var httpRequest = HttpContext.Request;
            var signalConnectionId = httpRequest.Form["signalConnectionId"];
            var Files = HttpContext.Request.Form.Files;
            //if (httpRequest.Files.Count > 0)
            if (Files.Count > 0)
            {
                string mimeType = "";
                string fileCaption = "";
                string fileSize = "";
                string fname = "";
                string furl = "";
                string fThumbUrl = "";
                var headline = "";
                var tags = "";
                // for (var i = 0; i < httpRequest.Files.Count; i++)
                for (var i = 0; i < Files.Count; i++)
                {
                    //  var file = httpRequest.Files[i];
                    var file = Files[i];
                    //if (file == null || file.ContentLength <= 0) continue;
                    if (file == null || file.Length <= 0) continue;
                    var fileName = RandomName();

                    fname = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);
                    // mimeType = MimeMapping.GetMimeMapping(file.FileName);
                    mimeType = _contentType.TryGetContentType(file.FileName, out mimeType).ToString();
                    //fileSize = file.ContentLength.ToString();
                    fileSize = file.Length.ToString();
                    fileCaption = fileName + ".mp4";

                    //string fileUrl = Path.Combine(HostingEnvironment.MapPath("~/images"), fileName + fileExtension);
                    var uniqueFileName = $"{fileName}{fileExtension}";//--------------------------------------------
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "~/images");//                  |
                    string fileUrl = Path.Combine(uploadsFolder, uniqueFileName);//                                |
                    //----------------------------------------------------------------------------------------------
                    // file.SaveAs(fileUrl);<------------------doubt
                    using (Stream fileStream = new FileStream(fileUrl, FileMode.Create)) { file.CopyToAsync(fileStream); }
                    //string outputfileUrl = Path.Combine(HostingEnvironment.MapPath("~/images/output"), fileName + ".mp4");
                    var uniqueFileUrl = $"{fileName}{".mp4"}";//-------------------------------------------------------------
                    string uploadOutptfileUrl = Path.Combine(_hostEnvironment.WebRootPath, "~/images/output");//            |
                    string outputfileUrl = Path.Combine(uploadOutptfileUrl, uniqueFileUrl);//                               |
                    //-------------------------------------------------------------------------------------------------------

                    //string outputThumbUrl = Path.Combine(HostingEnvironment.MapPath("~/images/output"), fileName + ".jpg");
                    var uniqueFileUrl2 = $"{fileName}{".jpg"}";//-----------------------------------------------------------
                    string uploadoutputThumbUrl = Path.Combine(_hostEnvironment.WebRootPath, "~/images/output");//          |
                    string outputThumbUrl = Path.Combine(uploadoutputThumbUrl, uniqueFileUrl2);//                           |
                    //------------------------------------------------------------------------------------------------------
                    string inputExtension = fileExtension.Replace(".", "");
                    var ffMpeg = new FFMpegConverter();
                    ConvertSettings convertSettings = new ConvertSettings()
                    {
                        VideoFrameSize = FrameSize.hd720,
                        VideoFrameRate = 24,
                    };
                    // var context = GlobalHost.ConnectionManager.GetHubContext<FileHub>();
                    var context = _hubContext;
                    //context.Clients.All.SendFileProgress(signalConnectionId, "0");
                    context.Clients.All.SendProgresss(signalConnectionId, "0");
                    ffMpeg.ConvertProgress += (o, args) =>
                    {
                        var conversionProgress = (String.Format("Progress: {0:HH:mm:ss}/{1:HH:mm:ss}", new DateTime(args.Processed.Ticks), new DateTime(args.TotalDuration.Ticks)));
                        var currentProgress = (int)((args.Processed.TotalSeconds / args.TotalDuration.TotalSeconds) * 100);
                        // context.Clients.All.SendFileProgress(signalConnectionId, conversionProgress);
                        context.Clients.All.SendProgresss(signalConnectionId, conversionProgress);
                    };
                    ffMpeg.ConvertMedia(fileUrl, inputExtension, outputfileUrl, Format.mp4, convertSettings);
                    ffMpeg.GetVideoThumbnail(fileUrl, outputThumbUrl);




                    //file upload to blob
                    CloudBlobContainer blobContainer = GetCloudBlobImageContainer();
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName + ".mp4");
                    blob.UploadFromStreamAsync(System.IO.File.OpenRead(outputfileUrl));
                    CloudBlockBlob blobThumb = blobContainer.GetBlockBlobReference(fileName + ".jpg");
                    blobThumb.UploadFromStreamAsync(System.IO.File.OpenRead(outputThumbUrl));
                    furl = blob.Uri.ToString();
                    fThumbUrl = blobThumb.Uri.ToString();
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
                return (IHttpActionResult)Ok(returnUrl);
            }
            return (IHttpActionResult)Ok("Error");
        }
        [Route("api/MediaStream/UploadVideo")]
        [HttpPost]
        public IHttpActionResult UploadVideo()
        {
            //var httpRequest = HttpContext.Current.Request;
            var httpRequest = HttpContext.Request;
            var files = HttpContext.Request.Form.Files;
            // if (httpRequest.Files.Count > 0)
            if (files.Count > 0)
            {
                string mimeType = "";
                string fileCaption = "";
                string fileSize = "";
                string fname = "";
                string furl = "";
                string fThumbUrl = "";
                // for (var i = 0; i < httpRequest.Files.Count; i++)
                for (var i = 0; i < files.Count; i++)
                {
                    // var file = httpRequest.Files[i];
                    var file = files[i];
                    // if (file == null || file.ContentLength <= 0) continue;
                    if (file == null || file.Length <= 0) continue;
                    var fileName = RandomName();
                    fname = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);

                    fileCaption = fileName + ".mp4";
                    // string fileUrl = Path.Combine(HostingEnvironment.MapPath("~/images"), fileName + fileExtension);
                    var uniqueFileName = $"{fileName}{fileExtension}";
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "~/images");
                    string fileUrl = Path.Combine(uploadsFolder, uniqueFileName);
                    //file.SaveAs(fileUrl);//doubt here need to think<--------------------------
                    using (Stream fileStream = new FileStream(fileUrl, FileMode.Create)) { file.CopyToAsync(fileStream); }

                    //string outputfileUrl = Path.Combine(HostingEnvironment.MapPath("~/images/output"), fileName + ".mp4");
                    var uniqueFileUrl = $"{fileName}{".mp4"}";//-------------------------------------------------------------
                    string uploadOutptfileUrl = Path.Combine(_hostEnvironment.WebRootPath, "~/images/output");//            |
                    string outputfileUrl = Path.Combine(uploadOutptfileUrl, uniqueFileUrl);//                               |
                    //-------------------------------------------------------------------------------------------------------
                    //string outputThumbUrl = Path.Combine(HostingEnvironment.MapPath("~/images/output"), fileName + ".jpg");
                    var uniqueFileUrl2 = $"{fileName}{".jpg"}";//-----------------------------------------------------------
                    string uploadoutputThumbUrl = Path.Combine(_hostEnvironment.WebRootPath, "~/images/output");//          |
                    string outputThumbUrl = Path.Combine(uploadoutputThumbUrl, uniqueFileUrl2);//                           |
                    //------------------------------------------------------------------------------------------------------

                    string inputExtension = fileExtension.Replace(".", "");
                    var ffMpeg = new FFMpegConverter();
                    ConvertSettings convertSettings = new ConvertSettings()
                    {
                        VideoFrameSize = FrameSize.hd720,
                        VideoFrameRate = 24,
                    };
                    // var context = GlobalHost.ConnectionManager.GetHubContext<FileHub>();
                    // var context = GlobalHost.ConnectionManager.GetHubContext<FileHub>();
                    var context = _hubContext;
                    ffMpeg.ConvertMedia(fileUrl, inputExtension, outputfileUrl, Format.mp4, convertSettings);
                    ffMpeg.GetVideoThumbnail(fileUrl, outputThumbUrl);
                    //file upload to blob
                    CloudBlobContainer blobContainer = GetCloudBlobImageContainer();
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName + ".mp4");
                    blob.UploadFromStreamAsync(System.IO.File.OpenRead(outputfileUrl));
                    CloudBlockBlob blobThumb = blobContainer.GetBlockBlobReference(fileName + ".jpg");
                    blobThumb.UploadFromStreamAsync(System.IO.File.OpenRead(outputThumbUrl));
                    furl = blob.Uri.ToString();
                    fThumbUrl = blobThumb.Uri.ToString();
                    mimeType = "video/mp4";
                    blob.FetchAttributesAsync();
                    fileSize = blob.Properties.Length.ToString();
                    if (System.IO.File.Exists(fileUrl))
                    {
                        System.IO.File.Delete(fileUrl);
                    }
                }
                var returnUrl = JsonConvert.SerializeObject(new { FileName = fname, FileUrl = furl, Caption = fileCaption, MimeType = mimeType, FileSize = fileSize, FileThumbUrl = fThumbUrl });
                return (IHttpActionResult)Ok(returnUrl);
            }
            return (IHttpActionResult)Ok("Error");
        }
        private CloudBlobContainer GetCloudBlobImageContainer()
        {
            var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .Build();
            string connectionString = config.GetConnectionString("devdiscourse_AzureStorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("imagegallery");
            var created = Convert.ToBoolean(container.CreateIfNotExistsAsync());
            //f (container.CreateIfNotExists())
            if (created)
            {
                container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return container;
        }

        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }

    }
}
