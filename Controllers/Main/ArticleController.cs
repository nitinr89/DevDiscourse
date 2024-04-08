using Devdiscourse.Data;
using Devdiscourse.Hubs;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Html2Amp;
using Html2Amp.Sanitization;
using Html2Amp.Sanitization.Implementation;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ServiceStack.Host;
using ServiceStack.Html;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Devdiscourse.Controllers.Main
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public ArticleController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {

            this.db = db;
            this.userManager = userManager;
        }
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            string sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == string.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        public string GetDeviceInfo()
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var deviceInfo = userAgent;
            return deviceInfo;
        }
        public string GetIPAddress()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IP  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            return myIP;
        }
        public string GetPublicIP()
        {
            string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipAddress == "::1") ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(':');
                if (addresses.Length != 0)
                {
                    ipAddress = addresses[0];
                }
            }
            return ipAddress;
        }
        public ActionResult MobileComments(long id)
        {
            ViewBag.id = id;
            return View();
        }
        public async Task<ActionResult> Index(string? prefix, long id, string reg = "")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string scheme = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            string absoluteUri = HttpContext.Request.GetDisplayUrl();
            if (id == null || id == 0)
            {
                if (int.TryParse(prefix, out int result))
                    id = result;
                else
                    throw new HttpException(404, "Error 404");
            }
            var search = await db.DevNews.Where(dn => dn.NewsId == id && dn.AdminCheck).FirstOrDefaultAsync();
            if (search == null)
            {
                throw new HttpException(404, "Error 404");
            }
            var suffix = scheme.IndexOf("?amp");
            if ((prefix == "agency-wire" || prefix == null) && search.NewsLabels != null && suffix == -1)
            {
                return RedirectToRoutePermanent("ArticleDetailswithprefix", new { prefix = search.NewsLabels, id = search.GenerateSecondSlug() });
            }
            else if (prefix == null && search.NewsLabels == null && suffix == -1)
            {
                return RedirectToRoutePermanent("ArticleDetailswithprefix", new { prefix = "agency-wire", id = search.GenerateSecondSlug() });
            }
            ViewBag.label = search.NewsLabels ?? "";
            var converter = new HtmlToAmpConverter();
            converter.WithSanitizers(
                new HashSet<ISanitizer>
                {
                    new InstagramSanitizer(),
                    new TwitterSanitizer(),
                    new AudioSanitizer(),
                    new HrefJavaScriptSanitizer(),
                    new ImageSanitizer(),
                    new JavaScriptRelatedAttributeSanitizer(),
                    new StyleAttributeSanitizer(),
                    new ScriptElementSanitizer(),
                    new TargetAttributeSanitizer(),
                    new XmlAttributeSanitizer(),
                    new YouTubeVideoSanitizer(),
                    new AmpIFrameSanitizer()
                });
            string ampHtml = converter.ConvertFromHtml(search.Description).AmpHtml;
            ViewBag.ampHtml = ampHtml;
            var geolocation = GetGeoLocation();
            var MACAddress = GetMACAddress();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            bool isCrawler = userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase);
            if (!isCrawler)
            {
                await UpdateViewCount(search.NewsId, search.Title, search.Creator, geolocation, MACAddress);
            }
            string? cookie = Request.Cookies["Edition"];
            if (reg != "")
            {
                ViewBag.region = reg;
            }
            else if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";
            }
            if ((search.Type == "LiveBlog") && suffix == -1)
            {
                var blogUpdate = db.LiveBlogs.AsNoTracking().Where(a => a.ParentId == search.NewsId).OrderByDescending(c => c.CreatedOn).Take(1); ;
                ViewBag.blogUpdate = await blogUpdate.FirstOrDefaultAsync();
                return View("Mobile", search);
            }
            if (search.Type == "LiveBlog")
            {
                var blogUpdate = await db.LiveBlogs.AsNoTracking().Where(a => a.ParentId == search.NewsId).OrderByDescending(c => c.CreatedOn).ToListAsync();
                ViewBag.BlogUpdates = blogUpdate;
                ViewBag.BlogUpdatesCount = blogUpdate.Count();
            }
            ViewBag.publicIP = geolocation.IPv4;
            ViewBag.MACAddress = MACAddress;
            if (scheme.EndsWith("?amp")) return View("Index.amp", search);
            else return View(search);
        }
        public GeoLocationViewModel GetGeoLocation()
        {
            GeoLocationViewModel location = new GeoLocationViewModel();

            string Url = "https://geolocation-db.com/json/1a811210-241d-11eb-b7a9-293dae7a95e1";
            using (WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(Url);
                    var obj = JObject.Parse(json);
                    if (obj["country_name"] != null)
                    {
                        location.country_name = (string)obj["country_name"];
                        location.IPv4 = (string)obj["IPv4"];
                    }
                }
                catch
                {

                }
            }

            return location;
        }
        public async Task<string> UpdateViewCount(long id, string title, string user, GeoLocationViewModel location, string MACAddress)
        {
            string deviceinfo = GetDeviceInfo();
            string ipaddress = GetPublicIP();
            if (ipaddress == "")
            {
                ipaddress = location.IPv4;
            }
            var region = "";
            var country = db.Countries.FirstOrDefault(a => a.Title == location.country_name);
            if (country != null)
            {
                region = country.Title;
            }
            // Find News in DevNews
            var search = await db.DevNews.FirstOrDefaultAsync(a => a.NewsId == id);
            search.ViewCount = search.ViewCount + 1;
            db.Entry(search).State = EntityState.Modified;
            db.Entry(search).Property(n => n.NewsId).IsModified = false;
            await db.SaveChangesAsync();
            return "OK";
        }
        public async Task<ActionResult> Mobile(string prefix, long? id)
        {
            if (id == null || id == 0)
            {
                throw new HttpException(404, "Error 404");
            }
            var search = db.DevNews.FirstOrDefault(a => a.NewsId == id);
            if (search == null)
            {
                throw new HttpException(404, "Error 404");
            }
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";

            }
            var blogUpdate = db.LiveBlogs.Where(a => a.ParentId == search.NewsId).OrderByDescending(c => c.CreatedOn).Take(1); ;
            ViewBag.blogUpdate = await blogUpdate.FirstOrDefaultAsync();
            return View(search);
        }
        public async Task<ActionResult> MobileArticleComments(long? id)
        {
            if (id == null || id == 0)
            {
                throw new HttpException(404, "Error 404");
            }
            var search = await db.DevNews.FirstOrDefaultAsync(a => a.NewsId == id);
            if (search == null)
            {
                throw new HttpException(404, "Error 404");
            }
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.region = "Global Edition";
            }
            else
            {
                ViewBag.region = cookie.Replace("Edition=", "") ?? "Global Edition";

            }
            return View(search);
        }
        public async Task<ActionResult> MobileAmp(long? id, string reg = "Global Edition")
        {
            ViewBag.region = reg;
            var search = await db.DevNews.FirstOrDefaultAsync(a => a.NewsId == id && a.AdminCheck == true);
            if (search != null)
            {
                string description = Regex.Replace(search.Description, "style[^>]*", "");
                description = Regex.Replace(description, "<img", "<amp-img layout='responsive'");
                description = Regex.Replace(description, "<iframe", "<amp-iframe layout='responsive' sandbox=\"allow-scripts allow-same-origin\"");
                description = Regex.Replace(description, "width=\"100%\"", "width=\"300\"");
                description = Regex.Replace(description, "height=\"480\"", "height=\"200\"");
                ViewBag.AmpDescriptoion = description;
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            return View(search);
        }
        public async Task<ActionResult> Amp(long id, string reg)
        {
            ViewBag.macAddress = GetMACAddress();
            ViewBag.region = reg;
            var search = await db.DevNews.FirstOrDefaultAsync(a => a.NewsId == id);
            if (User.Identity.IsAuthenticated)
            {
                string userId = userManager.GetUserId(User);
                await AddAutoDefinedInterest(search.Sector, "AutoDefined", userId);
            }
            return View(search);
        }
        public async Task<string> AddAutoDefinedInterest(string sector, string type, string userId)
        {
            var sectorList = sector.Split(',').ToList();
            foreach (var _sector in sectorList)
            {
                UserInterest interest = new UserInterest
                {
                    UserId = userId,
                    Sector = _sector,
                    InterestType = type
                };
                db.UserInterests.Add(interest);
                await db.SaveChangesAsync();
            }
            return "Success!";
        }
        private string GetFileName(string hrefLink)
        {
            string[] parts = hrefLink.Split('/');
            string fileName = "";

            if (parts.Length > 0)
                fileName = parts[parts.Length - 1];
            else
                fileName = hrefLink;

            return fileName;
        }
        public async Task CreateLog(string url)
        {
            UserLog obj = new UserLog
            {
                LogTitle = "Visitors",
                ActivityUrl = url,
            };
            db.UserLogs.Add(obj);
            await db.SaveChangesAsync();
        }
    }
}