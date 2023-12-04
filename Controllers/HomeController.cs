using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
//using DevDiscourse.Hubs;
//using Devdiscourse.Models;
//using Devdiscourse.Models.BasicModels;
//using Devdiscourse.Models.ViewModel;
//using Html2Amp;
//using Html2Amp.Sanitization;
//using Html2Amp.Sanitization.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using PagedList;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Mvc;

namespace DevDiscourse.Controllers
{
    public class HomeController : Controller, IDisposable
    {
        private ApplicationDbContext _db;
        public HomeController(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        //public string GetMACAddress()
        //{
        //    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        //    string sMacAddress = string.Empty;
        //    foreach (NetworkInterface adapter in nics)
        //    {
        //        if (sMacAddress == string.Empty)// only return MAC Address from first card  
        //        {
        //            IPInterfaceProperties properties = adapter.GetIPProperties();
        //            sMacAddress = adapter.GetPhysicalAddress().ToString();
        //        }
        //    }
        //    return sMacAddress;
        //}
        public string GetMACAddress()
        {
            string sMacAddress = string.Empty;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return sMacAddress;
        }
        //[OutputCache(Duration = 60, VaryByParam = "none")]
        //public ActionResult Disclaimer()
        //{
        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (cookie == null)
        //    {
        //        ViewBag.edition = "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.edition = cookie.Value ?? "Global Edition";
        //    }
        //    return View();
        //}
        //[OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult PrivacyPolicy()
        {
            //HttpCookie cookie = Request.Cookies["Edition"];
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.edition = "Global Edition";
            }
            else
            {
                ViewBag.edition = cookie ?? "Global Edition";
            }
            return View();
        }
        public ActionResult Index(string reg = "")
        {
            ViewBag.edition = "Global Edition";
            //if (Request.Browser.Crawler)
            //{
            //	ViewBag.edition = "Global Edition";
            //}
            //else
            //{
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                string UserCountry = getUserLocation();
                if (string.IsNullOrEmpty(UserCountry))
                {
                    ViewBag.edition = "Global Edition";
                }
                else
                {
                    string? userRegion = _db.Countries.FirstOrDefault(a => a.Title.Contains(UserCountry))?.Title;
                    string cookieName = "Edition";
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.UtcNow.AddDays(1);
                    string cookieValue = userRegion ?? "Global Edition";
                    ViewBag.edition = userRegion ?? "Global Edition";
                    Response.Cookies.Append(cookieName, cookieValue, options);
                }
            }
            else
            {
                string edition = cookie;
                switch (edition)
                {
                    case "Central Africa":
                        return RedirectToAction("CentralAfrica", "Home");
                    case "East Africa":
                        return RedirectToAction("EastAfrica", "Home");
                    case "North America":
                        return RedirectToAction("NorthAmerica", "Home");
                    case "Southern Africa":
                        return RedirectToAction("SouthernAfrica", "Home");
                    case "West Africa":
                        return RedirectToAction("WestAfrica", "Home");
                    case "South Asia":
                        return RedirectToAction("SouthAsia", "Home");
                    case "East and South East Asia":
                        return RedirectToAction("EastAndSouthEastAsia", "Home");
                    case "Pacific":
                        return RedirectToAction("Pacific", "Home");
                    case "Europe and Central Asia":
                        return RedirectToAction("EuropeAndCentralAsia", "Home");
                    case "Latin America and Caribbean":
                        return RedirectToAction("LatinAmericaAndCaribbean", "Home");
                    case "Middle East and North Africa":
                        return RedirectToAction("MiddleEastAndNorthAfrica", "Home");
                }
                ViewBag.edition = edition.Replace("Edition=", "") ?? "Global Edition";
            }
            //}
            return View();
        }
        //public ActionResult Contribute()
        //{
        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (cookie == null)
        //    {
        //        ViewBag.region = "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.region = cookie.Value ?? "Global Edition";
        //    }
        //    return View();
        //}

        public string getUserLocation()
        {
            string VisitorIp = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (VisitorIp == "::1") VisitorIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            var jsonString = "";
            var countryName = "";
            try
            {
                string Url = "https://geolocation-db.com/json/0f761a30-fe14-11e9-b59f-e53803842572/" + VisitorIp;
                using (WebClient wc = new WebClient())
                {
                    jsonString = wc.DownloadString(Url);
                }
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonString);
                if (jsonObj.country_name != null)
                {
                    countryName = (string)jsonObj.country_name;
                }
            }
            catch
            {

            }
            return countryName;
        }

        //public async Task<ActionResult> Detail(Guid? id, string reg = "Global Edition", string fl = "")
        //{
        //    ViewBag.region = reg;
        //    ViewBag.fl = fl;
        //    if (id == null || id == new Guid())
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }
        //    var search = await _db.DevNews.FirstOrDefaultAsync(a => a.Id == id);
        //    if (search == null)
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }
        //    string scheme = Request.Url.AbsoluteUri;
        //    var suffix = scheme.IndexOf("?amp");
        //    if (suffix == -1)
        //    {
        //        return RedirectToRoutePermanent("ArticleDetailswithprefix", new { prefix = "agency-wire", id = search.GenerateSecondSlug() });
        //    }
        //    else
        //    {
        //        return RedirectToRoutePermanent("ArticleDetailswithprefix", new { prefix = "agency-wire", id = search.GenerateSecondSlug(), amp = "" });
        //    }
        //}
        //public ActionResult UserProfile()
        //{
        //    string userId = User.Identity.GetUserId();
        //    ViewBag.profile = _db.Users.Find(userId).ProfilePic;
        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (cookie == null)
        //    {
        //        ViewBag.edition = "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.edition = cookie.Value ?? "Global Edition";
        //    }
        //    return View();
        //}
        //[OutputCache(Duration = 60, VaryByParam = "*")]
        //public async Task<ActionResult> Search(string region = "", string sector = "All", string tag = "", string cat = "", string label = "")
        //{
        //    region = region.Replace("+", " ");
        //    ViewBag.sector = sector;
        //    if (sector != "All" && sector != "Videos" && sector != "EditorPic" && sector != "Sponsored")
        //    {
        //        var sectorSearch = await _db.DevSectors.FirstOrDefaultAsync(a => a.Slug == sector);
        //        if (sectorSearch != null)
        //        {
        //            ViewBag.sectorName = sectorSearch.Title;
        //            ViewBag.sector = sectorSearch.Id;
        //            ViewBag.sectorSlug = sectorSearch.Slug;
        //        }
        //    }
        //    else if (sector == "Videos" || sector == "Sponsored")
        //    {
        //        ViewBag.sectorName = sector;
        //    }
        //    else if (sector == "EditorPic")
        //    {
        //        ViewBag.sectorName = "Editor's Pick";
        //    }
        //    ViewBag.region = region;
        //    ViewBag.tag = tag;
        //    ViewBag.cat = cat ?? "";
        //    if (label != "")
        //    {
        //        var searchLabel = await _db.Labels.FirstOrDefaultAsync(a => a.Title == label);
        //        if (searchLabel != null)
        //        {
        //            ViewBag.label = searchLabel.Id;
        //            ViewBag.labelTitle = searchLabel.Title;
        //        }

        //    }
        //    ViewBag.labelSlug = label ?? "";

        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (region != "")
        //    {
        //        ViewBag.region = region;
        //    }
        //    else if (cookie != null)
        //    {
        //        ViewBag.region = cookie.Value ?? "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.region = "Global Edition";
        //    }
        //    return View();
        //}
        //public async Task<ActionResult> AdvancedSearch(string text = "")
        //{
        //    ViewBag.text = text;
        //    ViewBag.sectorList = await _db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo).ToListAsync();
        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (cookie == null)
        //    {
        //        ViewBag.region = "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.region = cookie.Value ?? "Global Edition";
        //    }
        //    return View();
        //}
        //public ActionResult DevBlogs(string type = "")
        //{
        //    ViewBag.type = type;
        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (cookie == null)
        //    {
        //        ViewBag.reg = "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.reg = cookie.Value ?? "Global Edition";
        //    }
        //    return View();
        //}
        //public JsonResult pledge()
        //{
        //    var search = (from m in _db.CampaignPetitions
        //                  where m.PledgeType != null
        //                  select new
        //                  {
        //                      m.FirstName,
        //                      m.LastName,
        //                      Join = m.CreatedOn,
        //                      m.Email,
        //                      m.PledgeType
        //                  }).ToList();
        //    return Json(search, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult DevEvents(string reg = "Global Edition")
        //{
        //    ViewBag.reg = reg;
        //    return View();
        //}
        //public async Task<ActionResult> EventDetails(long id, string fl = "")
        //{
        //    ViewBag.fl = fl;
        //    if (id == 0)
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }
        //    var search = await _db.Events.FirstOrDefaultAsync(a => a.EventId == id);
        //    if (search == null)
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }
        //    if (search != null)
        //    {
        //        string description = Regex.Replace(search.Description, "style[^>]*", "");
        //        description = Regex.Replace(description, "<img", "<amp-img layout=\"responsive\"");
        //        description = Regex.Replace(description, "<iframe", "<amp-iframe layout='responsive' sandbox=\"allow-scripts allow-same-origin\"");
        //        description = Regex.Replace(description, "width=\"100%\"", "width=\"300\"");
        //        ViewBag.AmpDescriptoion = description;
        //    }
        //    else
        //    {
        //        return RedirectToAction("PageNotFound", "Error");
        //    }
        //    return View(search);
        //}
        //public ActionResult OldEventDetails(Guid? id, string fl = "")
        //{
        //    ViewBag.fl = fl;
        //    if (id == null || id == new Guid())
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }
        //    var search = _db.Events.Find(id);
        //    if (search == null)
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }

        //    if (search != null)
        //    {
        //        string description = Regex.Replace(search.Description, "style[^>]*", "");
        //        description = Regex.Replace(description, "<img", "<amp-img layout=\"responsive\"");
        //        description = Regex.Replace(description, "<iframe", "<amp-iframe layout='responsive' sandbox=\"allow-scripts allow-same-origin\"");
        //        description = Regex.Replace(description, "width=\"100%\"", "width=\"300\"");
        //        ViewBag.AmpDescriptoion = description;
        //    }
        //    else
        //    {
        //        return RedirectToAction("PageNotFound", "Error");
        //    }
        //    return View("EventDetails", search);
        //}
        //// Partial Views
        //[OutputCache(Duration = 60, VaryByParam = "*")]

        public PartialViewResult GetInfocus(string reg = "Global Edition")
        {
            var lastThreeHour = DateTime.UtcNow.AddHours(-12);
            var infocus = _db.RegionNewsRankings
              //.AsNoTracking() // Avoid unnecessary tracking
              .Where(a => a.DevNews.AdminCheck == true
                          && a.Region.Title == reg
                          && a.DevNews.CreatedOn > lastThreeHour
                          && !a.DevNews.Title.Contains("News Summary")
                          && !a.DevNews.Title.Contains("Highlights")
                          && a.DevNews.NewsLabels != "Newsalert"
                          && !new[] { "14", "18", "19", "9" }.Contains(a.DevNews.Sector))
              .Select(s => new LatestNewsView
              {
                  Id = s.DevNews.Id,
                  NewId = s.DevNews.NewsId,
                  Title = s.DevNews.Title,
                  ImageUrl = s.DevNews.ImageUrl,
                  CreatedOn = s.DevNews.ModifiedOn,
                  Type = s.DevNews.Type,
                  SubType = s.DevNews.SubType,
                  Country = s.DevNews.Country,
                  Label = s.DevNews.NewsLabels,
                  Ranking = s.Ranking
              })
              .OrderByDescending(a => a.CreatedOn)
              .Take(65)
              .ToList();
            //var infocus = _db.RegionNewsRankings.AsNoTracking().Where(a => a.DevNews.AdminCheck == true && a.Region.Title == reg && a.DevNews.CreatedOn > lastThreeHour && !a.DevNews.Title.Contains("News Summary") && !a.DevNews.Title.Contains("Highlights") && a.DevNews.NewsLabels != "Newsalert" && a.DevNews.Sector != "14" && a.DevNews.Sector != "18" && a.DevNews.Sector != "19" && a.DevNews.Sector != "9").Select(s => new LatestNewsView
            //    {
            //        Id = s.DevNews.Id,
            //        NewId = s.DevNews.NewsId,
            //        Title = s.DevNews.Title,
            //        ImageUrl = s.DevNews.ImageUrl,
            //        CreatedOn = s.DevNews.ModifiedOn,
            //        Type = s.DevNews.Type,
            //        SubType = s.DevNews.SubType,
            //        Country = s.DevNews.Country,
            //        Label = s.DevNews.NewsLabels,
            //        Ranking = s.Ranking
            //    }).OrderByDescending(a => a.CreatedOn).Take(65).ToList();
            return PartialView("_getInfocus", infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());
        }

        //[OutputCache(Duration = 60, VaryByParam = "*")]
        //public PartialViewResult GetAmpInfocus(string reg = "Global Edition")
        //{
        //    var search = (from m in _db.Infocus
        //                  where m.Edition == reg && (m.ItemType == "News" || m.ItemType == "Blog")
        //                  join s in _db.DevNews.Where(s => s.AdminCheck == true) on m.NewsId equals s.NewsId
        //                  orderby m.SrNo
        //                  select new LatestNewsView
        //                  {
        //                      Id = s.Id,
        //                      NewId = s.NewsId,
        //                      Title = s.Title,
        //                      ImageUrl = s.ImageUrl,
        //                      CreatedOn = s.ModifiedOn,
        //                      Type = s.Type,
        //                      SubType = s.SubType,
        //                      Country = s.Country,
        //                      Label = s.NewsLabels,
        //                      SrNo = m.SrNo
        //                  }).AsNoTracking().Take(10);

        //    return PartialView("_getampInfocus", search.ToList());
        //}
        //public PartialViewResult GlobalDevelopment(Guid? id, int skip = 0, int take = 0)
        //{
        //    DateTime threedays = DateTime.Today.AddDays(-3);
        //    if (id == null)
        //    {
        //        var search = _db.DevNews.Where(a => (a.IsGlobal == true || a.Region.Contains("Global Edition")) && a.CreatedOn > threedays && a.AdminCheck == true && a.NewsLabels != null).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take).ToList();
        //        return PartialView("_globalDevelopment", search);

        //    }
        //    else
        //    {
        //        var search = _db.DevNews.Where(a => a.Id != id && (a.IsGlobal == true || a.Region.Contains("Global Edition")) && a.CreatedOn > threedays && a.AdminCheck == true && a.NewsLabels != null).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take).ToList();
        //        return PartialView("_globalDevelopment", search);
        //    }

        //}
        //[OutputCache(Duration = 60, VaryByParam = "*")]

        public PartialViewResult GetNews(string reg = "Global Edition", int skip = 0, int take = 0)
        {
            ViewBag.skipCount = skip;
            DateTime tenDays = DateTime.Today.AddHours(-10);
            //var infocusData = _db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
            if (reg == "Global Edition")
            {
                var result = _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Sector != null && a.NewsLabels != null)
                    //.AsNoTracking()
                    .OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels })
                    //.AsNoTracking()
                    .Skip(skip).Take(take);
                return PartialView("_getNews", result.ToList());
            }
            else
            {
                var result = _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Region.Contains(reg) && a.Sector != null && a.NewsLabels != null)
                    //.AsNoTracking()
                    .OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels })
                    //.AsNoTracking()
                    .Skip(skip).Take(take);
                return PartialView("_getNews", result.ToList());
            }
        }

        //public PartialViewResult GetampNews(string reg = "Global Edition", int skip = 0, int take = 0)
        //{
        //    ViewBag.skipCount = skip;
        //    DateTime threemonths = DateTime.Today.AddDays(-10);
        //    var infocusData = _db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
        //    if (reg == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.CreatedOn > threemonths && a.AdminCheck == true && a.Sector != "0" && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take);
        //        return PartialView("_getampNews", result.ToList());
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.CreatedOn > threemonths && a.AdminCheck == true && a.Region.Contains(reg) && a.Sector != "0" && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take);
        //        return PartialView("_getampNews", result.ToList());
        //    }
        //}
        //public PartialViewResult GetBlogs(string reg, int skip = 0, int take = 0)
        //{
        //    ViewBag.reg = reg;
        //    ViewBag.skipCount = skip;
        //    var infocusData = _db.Infocus.Where(a => a.Edition == reg && a.ItemType == "Blog").Select(a => a.NewsId).ToList();
        //    if (reg == "Global Edition")
        //    {
        //        var resultList = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.Type == "Blog" && a.IsSponsored == false).OrderByDescending(a => a.PublishedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Tags, Country = a.Country, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take).ToList();
        //        return PartialView("_getBlogs", resultList);
        //    }
        //    else
        //    {
        //        var resultList = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.Region.Contains(reg) && a.Type == "Blog" && a.IsSponsored == false).OrderByDescending(a => a.PublishedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Tags, Country = a.Country, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take).ToList();
        //        return PartialView("_getBlogs", resultList);
        //    }
        //}
        //[OutputCache(Duration = 180, VaryByParam = "*")]

        public PartialViewResult GetTrends(Guid? id, string reg = "Global Edition", string filter = "")
        {
            ViewBag.filter = filter;
            DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
            if (reg == "Global Edition")
            {
                var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.Id != id && a.IsSponsored == false).OrderByDescending(a => a.ViewCount).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Take(4)
                    //.AsNoTracking()
                    .ToList();
                return PartialView("_getTrends", resultList);
            }
            else
            {
                var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.Id != id && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.ViewCount).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Take(4)
                    //.AsNoTracking()
                    .ToList();
                return PartialView("_getTrends", resultList);
            }
        }

        //public PartialViewResult GetampTrends(Guid? id, string reg = "Global Edition", string filter = "")
        //{
        //    ViewBag.filter = filter;
        //    DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
        //    DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
        //    if (reg == "Global Edition")
        //    {
        //        var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.Id != id).OrderByDescending(a => a.ViewCount).Take(5).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, SubType = a.SubType, Label = a.NewsLabels }).ToList();
        //        return PartialView("_getampTrends", resultList);
        //    }
        //    else
        //    {
        //        var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.Id != id && a.Region.Contains(reg)).OrderByDescending(a => a.ViewCount).Take(5).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, SubType = a.SubType, Label = a.NewsLabels }).ToList();
        //        return PartialView("_getampTrends", resultList);
        //    }
        //}
        //public PartialViewResult GetRelatedNews(Guid id, string reg, string sector, int skip)
        //{
        //    // Array of sectors
        //    var secList = sector.Split(',').ToList();
        //    DateTime onemonths = DateTime.Today.AddDays(-10);
        //    if (reg == "Global Edition")
        //    {
        //        var search = (from m in _db.DevNews
        //                      from s in secList
        //                      where m.AdminCheck == true && m.CreatedOn > onemonths && m.Id != id && (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //                      orderby m.CreatedOn descending
        //                      select new LatestNewsView
        //                      {
        //                          Id = m.Id,
        //                          Title = m.Title,
        //                          CreatedOn = m.CreatedOn,
        //                          ImageUrl = m.ImageUrl,
        //                          NewId = m.NewsId,
        //                          Type = m.Type,
        //                          SubType = m.SubType,
        //                          Label = m.NewsLabels
        //                      }).Skip(skip).Take(6).ToList();
        //        return PartialView("_getRelatedNews", search);
        //    }
        //    else
        //    {
        //        var search = (from m in _db.DevNews
        //                      from s in secList
        //                      where m.AdminCheck == true && m.CreatedOn > onemonths && m.Region.Contains(reg) && m.Id != id && (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //                      orderby m.CreatedOn descending
        //                      select new LatestNewsView
        //                      {
        //                          Id = m.Id,
        //                          Title = m.Title,
        //                          CreatedOn = m.CreatedOn,
        //                          ImageUrl = m.ImageUrl,
        //                          NewId = m.NewsId,
        //                          Type = m.Type,
        //                          SubType = m.SubType,
        //                          Label = m.NewsLabels

        //                      }).Skip(skip).Take(6).ToList();
        //        return PartialView("_getRelatedNews", search);
        //    }

        //}
        //public PartialViewResult GetRelatedBlogs(Guid id, string reg, string sector, int skip)
        //{
        //    // Array of sectors
        //    var secList = sector.Split(',').ToList();
        //    List<LatestNewsView> resultList = new List<LatestNewsView>();
        //    if (reg == "Global Edition" && sector == "0")
        //    {
        //        var dataList = _db.DevNews.Where(m => m.Sector != "0" && m.AdminCheck == true && m.Type == "Blog" && m.Id != id);
        //        var search = from m in dataList
        //                     select new LatestNewsView
        //                     {
        //                         Id = m.Id,
        //                         Title = m.Title,
        //                         CreatedOn = m.CreatedOn,
        //                         ImageUrl = m.ImageUrl,
        //                         NewId = m.NewsId,
        //                         Type = m.Type,
        //                         SubType = m.SubType,
        //                         Label = m.NewsLabels
        //                     };
        //        resultList = search.Distinct().OrderByDescending(a => a.CreatedOn).Skip(skip).Take(6).ToList();
        //    }
        //    else if (reg == "Global Edition")
        //    {
        //        var dataList = _db.DevNews.Where(m => m.Sector != "0" && m.AdminCheck == true && m.Type == "Blog" && m.Id != id);
        //        var search = from m in dataList
        //                     from s in secList
        //                     where (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //                     select new LatestNewsView
        //                     {
        //                         Id = m.Id,
        //                         Title = m.Title,
        //                         CreatedOn = m.CreatedOn,
        //                         ImageUrl = m.ImageUrl,
        //                         NewId = m.NewsId,
        //                         Type = m.Type,
        //                         SubType = m.SubType,
        //                         Label = m.NewsLabels
        //                     };
        //        resultList = search.Distinct().OrderByDescending(a => a.CreatedOn).Skip(skip).Take(6).ToList();
        //    }
        //    else if (reg != "Global Edition" && sector == "0")
        //    {
        //        var dataList = _db.DevNews.Where(m => m.Sector != "0" && m.AdminCheck == true && m.Region.Contains(reg) && m.Type == "Blog" && m.Id != id);
        //        var search = from m in dataList
        //                     select new LatestNewsView
        //                     {
        //                         Id = m.Id,
        //                         Title = m.Title,
        //                         CreatedOn = m.CreatedOn,
        //                         ImageUrl = m.ImageUrl,
        //                         NewId = m.NewsId,
        //                         Type = m.Type,
        //                         SubType = m.SubType,
        //                         Label = m.NewsLabels
        //                     };
        //        resultList = search.Distinct().OrderByDescending(a => a.CreatedOn).Skip(skip).Take(6).ToList();
        //    }
        //    else
        //    {
        //        var dataList = _db.DevNews.Where(m => m.Sector != "0" && m.AdminCheck == true && m.Region.Contains(reg) && m.Type == "Blog" && m.Id != id);
        //        var search = from m in dataList
        //                     from s in secList
        //                     where (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //                     select new LatestNewsView
        //                     {
        //                         Id = m.Id,
        //                         Title = m.Title,
        //                         CreatedOn = m.CreatedOn,
        //                         ImageUrl = m.ImageUrl,
        //                         NewId = m.NewsId,
        //                         Type = m.Type,
        //                         SubType = m.SubType,
        //                         Label = m.NewsLabels
        //                     };
        //        resultList = search.Distinct().OrderByDescending(a => a.CreatedOn).Skip(skip).Take(6).ToList();
        //    }
        //    return PartialView("_getRelatedNews", resultList);
        //}
        //// Your Interest
        //public PartialViewResult GetYourInterest(string sector, string reg = "Global Edition")
        //{
        //    if (string.IsNullOrEmpty(sector))
        //    {
        //        throw new HttpException(404, "Error 404");
        //    }
        //    List<string> secList = new List<string>();
        //    // In case of Cache Data
        //    secList = sector.Split(',').ToList();
        //    // In case of Login User
        //    //string userId = User.Identity.GetUserId();
        //    //List<string> sectorList = new List<string>();
        //    //if (!String.IsNullOrEmpty(userId))
        //    //{
        //    //    var dataList = _db.UserInterests.Where(a => a.UserId == userId).Select(a => new InterestView { Sector = a.Sector }).AsNoTracking().OrderBy(a => Guid.NewGuid()).Take(2).ToList();
        //    //    if (dataList.Any())
        //    //    {
        //    //        foreach (var item in dataList)
        //    //        {
        //    //            secList.Add(item.Sector);
        //    //        }
        //    //    }
        //    //}
        //    DateTime onemonths = DateTime.Today.AddDays(-15);
        //    // Result Data
        //    if (reg == "Global Edition" || reg == "Global")
        //    {
        //        var search = (from m in _db.DevNews
        //                      where m.AdminCheck == true && m.CreatedOn > onemonths && m.IsSponsored == false && (m.ImageUrl != "/images/defaultImage.jpg" && m.ImageUrl != "/images/newstheme.jpg" && m.ImageUrl != "")
        //                      from s in secList
        //                      where m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s
        //                      select new LatestNewsView
        //                      {
        //                          Id = m.Id,
        //                          Title = m.Title,
        //                          CreatedOn = m.CreatedOn,
        //                          ImageUrl = m.ImageUrl,
        //                          Type = m.Type,
        //                          NewId = m.NewsId,
        //                          SubType = m.SubType,
        //                          Label = m.NewsLabels
        //                      }).AsNoTracking();
        //        return PartialView("_getYourInterest", search.Distinct().OrderByDescending(a => a.CreatedOn).Skip(30).Take(20).ToList());
        //    }
        //    else
        //    {
        //        var search = (from m in _db.DevNews
        //                      where m.AdminCheck == true && m.Region.Contains(reg) && m.CreatedOn > onemonths && m.IsSponsored == false && (m.ImageUrl != "/images/defaultImage.jpg" && m.ImageUrl != "/images/newstheme.jpg" && m.ImageUrl != "")
        //                      from s in secList
        //                      where m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s
        //                      select new LatestNewsView
        //                      {
        //                          Id = m.Id,
        //                          Title = m.Title,
        //                          CreatedOn = m.CreatedOn,
        //                          ImageUrl = m.ImageUrl,
        //                          Type = m.Type,
        //                          NewId = m.NewsId,
        //                          SubType = m.SubType,
        //                          Label = m.NewsLabels
        //                      }).AsNoTracking();
        //        return PartialView("_getYourInterest", search.Distinct().OrderByDescending(a => a.CreatedOn).Skip(30).Take(20).ToList());
        //    }
        //}
        //public PartialViewResult GetSponsoredNews(Guid? id, int take = 3, string reg = "Global Edition")
        //{
        //    var resultList = _db.DevNews.Where(a => a.Id != id && a.AdminCheck == true && a.IsSponsored == true).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Tags, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Take(take).ToList();
        //    return PartialView("_getSponsoredNews", resultList);
        //}
        //public PartialViewResult GetNewsItems(string sector, string region, string country, string tag, string cat, string label, int? page)
        //{
        //    DateTime oneMonth = DateTime.Today.AddDays(-10);
        //    cat = cat ?? "";
        //    int pageSize = 20;
        //    int pageNumber = (page ?? 1);
        //    //if (region == "Global Edition")
        //    //{
        //    //    var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.NewsLabels == label && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
        //    //    {
        //    //        Title = a.Title,
        //    //        NewsId = a.NewsId,
        //    //        ImageUrl = a.ImageUrl,
        //    //        Subtitle = a.SubTitle,
        //    //        Country = a.Country,
        //    //        CreatedOn = a.ModifiedOn,
        //    //        Sector = a.Type,
        //    //        SubType = a.SubType,
        //    //        Label = a.NewsLabels,
        //    //        Ranking = 0
        //    //    }).AsNoTracking().ToPagedList(pageNumber, pageSize);

        //    //    return PartialView("getNewsItems", resultList);
        //    //}
        //    //else
        //    //{

        //    var resultList = _db.RegionNewsRankings.AsNoTracking().Where(a => a.DevNews.AdminCheck == true && a.DevNews.NewsLabels == label && a.Region.Title == region && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
        //    {
        //        Title = a.DevNews.Title,
        //        NewsId = a.DevNews.NewsId,
        //        ImageUrl = a.DevNews.ImageUrl,
        //        Subtitle = a.DevNews.SubTitle,
        //        Country = a.DevNews.Country,
        //        CreatedOn = a.DevNews.ModifiedOn,
        //        Sector = a.DevNews.Type,
        //        SubType = a.DevNews.SubType,
        //        Label = a.DevNews.NewsLabels,
        //        Ranking = a.Ranking
        //    }).ToPagedList(pageNumber, pageSize);

        //    return PartialView("getNewsItems", resultList.OrderByDescending(s => s.CreatedOn.Date).ThenByDescending(o => o.Ranking));
        //    //return PartialView("_getSectorNews", resultList);
        //    //}
        //    //IQueryable<SearchView> search;
        //    //if (region != "Global Edition")
        //    //{
        //    //    search = _db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(region) && a.CreatedOn > oneMonth).Select(a => new SearchView { Id = a.Id, NewsId = a.NewsId, Title = a.Title, ImageUrl = a.ImageUrl, Country = a.Country, CreatedOn = a.CreatedOn, Region = a.Region, Sector = a.Sector, IsGlobal = a.IsGlobal, IsVideo = a.IsVideo, IsSponsored = a.IsSponsored, EditorPick = a.EditorPick, Tags = a.Tags, Type = a.Type, SubType = a.SubType, Category = a.Category, Label = a.NewsLabels, Source = a.Source }).AsNoTracking();
        //    //}
        //    //else
        //    //{
        //    //    search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > oneMonth).Select(a => new SearchView { Id = a.Id, NewsId = a.NewsId, Title = a.Title, ImageUrl = a.ImageUrl, Country = a.Country, CreatedOn = a.CreatedOn, Region = a.Region, Sector = a.Sector, IsGlobal = a.IsGlobal, IsVideo = a.IsVideo, IsSponsored = a.IsSponsored, EditorPick = a.EditorPick, Tags = a.Tags, Type = a.Type, SubType = a.SubType, Category = a.Category, Label = a.NewsLabels, Source = a.Source }).AsNoTracking();
        //    //}
        //    //if (sector == "1")
        //    //{
        //    //    search = search.Where(s => (s.Sector.Contains(",1,") || s.Sector.StartsWith("1,") || s.Sector.EndsWith(",1") || s.Sector == "1") || (s.Sector.Contains(",5,") || s.Sector.StartsWith("5,") || s.Sector.EndsWith(",5") || s.Sector == "5"));
        //    //}
        //    //else if (!string.IsNullOrEmpty(sector) && sector != "0" && sector != "Videos" && sector != "EditorPic" && sector != "Sponsored")
        //    //{
        //    //    search = search.Where(s => s.Sector.Contains("," + sector + ",") || s.Sector.StartsWith(sector + ",") || s.Sector.EndsWith("," + sector) || s.Sector == sector);
        //    //}
        //    //else if (sector == "Videos")
        //    //{
        //    //    search = search.Where(a => a.IsVideo == true);
        //    //}
        //    //else if (sector == "EditorPic")
        //    //{
        //    //    search = search.Where(a => a.EditorPick == true);
        //    //}
        //    //else if (sector == "Sponsored")
        //    //{
        //    //    search = search.Where(a => a.IsSponsored == true);
        //    //}
        //    //if (!string.IsNullOrEmpty(tag))
        //    //{
        //    //    search = search.Where(s => s.Tags.Contains(tag));
        //    //}
        //    //if (!string.IsNullOrEmpty(cat))
        //    //{
        //    //    search = search.Where(s => s.Category.Contains(cat));
        //    //}
        //    //if (!string.IsNullOrEmpty(label))
        //    //{
        //    //    search = search.Where(s => s.Label == label);
        //    //}else if (sector == "0" && string.IsNullOrEmpty(cat))
        //    //{
        //    //    search = search.Where(s => s.Label == null && (s.Source =="PTI" || s.Source == "IANS" || s.Source == "Reuters"));
        //    //}
        //    //if (!string.IsNullOrEmpty(country))
        //    //{
        //    //    search = search.Where(s => s.Country.Contains(country));
        //    //}
        //    //var result = search.OrderByDescending(m => m.CreatedOn);

        //}
        //public JsonResult GetAmpNewsItems(string __amp_source_origin, string sector, string region, string tag, string label, int? moreItemsPageIndex)
        //{
        //    DateTime oneMonth = DateTime.Today.AddDays(-30);
        //    var search = _db.DevNews.AsNoTracking().Where(a => a.AdminCheck == true && a.CreatedOn > oneMonth).Select(a => new { a.Region, a.IsGlobal, a.Sector, a.CreatedOn, a.Title, Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId.ToString(), a.ImageUrl, a.Country, a.Tags, a.NewsLabels, a.NewsId });
        //    if (region != "Global Edition")
        //    {
        //        search = search.Where(s => s.Region.Contains(region));
        //    }
        //    if (!string.IsNullOrEmpty(sector) && sector != "0")
        //    {
        //        search = search.Where(s => s.Sector.Contains("," + sector + ",") || s.Sector.StartsWith(sector + ",") || s.Sector.EndsWith("," + sector) || s.Sector == sector);
        //    }
        //    if (!string.IsNullOrEmpty(tag))
        //    {
        //        search = search.Where(s => s.Tags.Contains("," + tag + ",") || s.Tags.StartsWith(tag + ",") || s.Tags.EndsWith("," + tag) || s.Tags == tag);
        //    }
        //    if (!string.IsNullOrEmpty(label))
        //    {
        //        search = search.Where(s => s.NewsLabels == label);
        //    }
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);

        //    }
        //    var result = search.OrderByDescending(m => m.CreatedOn).Select(b => new
        //    {
        //        b.Title,
        //        b.NewsLabels,
        //        b.NewsId,
        //        b.Country,
        //        defaultImage = b.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //        ImageUrl = b.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? b.ImageUrl : "/remote.axd?" + b.ImageUrl
        //    });
        //    int pageSize = 10;
        //    int pageNumber = (moreItemsPageIndex ?? 1);
        //    var resultData = result.ToPagedList(pageNumber, pageSize);
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetAmpLatestNewsItems(string __amp_source_origin, int? moreItemsPageIndex)
        //{
        //    DateTime threeDays = DateTime.Today.AddDays(-3);
        //    int pageSize = 5;
        //    int pageNumber = (moreItemsPageIndex ?? 1);
        //    var resultData = (_db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > threeDays && a.NewsLabels != null).OrderByDescending(m => m.ModifiedOn).Select(b => new { b.Title, Url = "/article/" + (b.NewsLabels ?? "agency-wire") + "/" + b.NewsId.ToString(), b.ImageUrl, b.Country })).ToPagedList(pageNumber, pageSize);
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
        //    }
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}
        //public PartialViewResult GetAdvancedSearch(string text, string type, string sector, string region, string country, DateTime? beforeDate, DateTime? afterDate, int? page)
        //{
        //    // List<AdvancedSearchView> resultList = new List<AdvancedSearchView>();
        //    var stringSearch = text ?? "";
        //    var typeSearch = type ?? "";
        //    var countrySearch = country ?? "";
        //    var searchRegion = region == "Global Edition" ? "" : region;

        //    var newsSearch = _db.DevNews.Where(a => a.AdminCheck == true && a.Country.Contains(countrySearch) && a.Type.Contains(typeSearch) && a.Title.Contains(stringSearch) && a.Region.Contains(searchRegion)).Select(a => new AdvancedSearchView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Themes = a.Themes, Country = a.Country, Region = a.Region, Tags = a.Tags, Type = a.Type, SubType = a.SubType, IsGlobal = a.IsGlobal, NewsId = a.NewsId, Label = a.NewsLabels });
        //    //resultList = newsSearch.ToList();
        //    if (sector != "0")
        //    {
        //        newsSearch = newsSearch.Where(s => s.Sector.Contains("," + sector + ",") || s.Sector.StartsWith(sector + ",") || s.Sector.EndsWith("," + sector) || s.Sector == sector);
        //    }
        //    if (beforeDate != null)
        //    {
        //        DateTime filterDate = DateTime.Parse(beforeDate.ToString());
        //        newsSearch = newsSearch.Where(s => s.CreatedOn < filterDate);
        //    }
        //    if (afterDate != null)
        //    {
        //        DateTime filterDate2 = DateTime.Parse(afterDate.ToString());
        //        newsSearch = newsSearch.Where(s => s.CreatedOn > filterDate2);
        //    }
        //    int pageSize = 20;
        //    int pageNumber = (page ?? 1);
        //    return PartialView("_getAdvancedSearch", newsSearch.OrderByDescending(m => m.CreatedOn).ToPagedList(pageNumber, pageSize));
        //}
        //public PartialViewResult GetBlogItems(int? page, string type, string region = "Global Edition")
        //{
        //    DateTime LastSixMonth = DateTime.UtcNow.AddMonths(-24);
        //    if (Request.IsAjaxRequest())
        //    {
        //        LastSixMonth = DateTime.UtcNow.AddMonths(-60);
        //    }
        //    var search = _db.DevNews.AsNoTracking().Where(a => a.Type == "Blog" && a.CreatedOn > LastSixMonth && a.AdminCheck == true).Select(a => new AdvancedSearchView { Id = a.Id, NewsId = a.NewsId, Title = a.Title, SubType = a.SubType, ImageUrl = a.ImageUrl, Region = a.Region, IsGlobal = a.IsGlobal, CreatedOn = a.PublishedOn, Label = a.NewsLabels, Country = a.Author }).OrderByDescending(m => m.CreatedOn).ToList();
        //    if (region != "Global Edition")
        //    {
        //        search = search.Where(a => a.Region != null && a.Region.Contains(region)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        search = search.Where(a => string.Equals(a.SubType, type, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }
        //    else
        //    {
        //        search = search.Where(a => !string.Equals(a.SubType, "interview", StringComparison.OrdinalIgnoreCase)).ToList();
        //    }
        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);
        //    return PartialView("_getBlogItems", search.ToPagedList(pageNumber, pageSize));
        //}
        //public JsonResult GetAmpBlogItems(string __amp_source_origin, int? moreItemsPageIndex)
        //{
        //    var search = _db.DevNews.Where(a => a.Type == "Blog" && a.AdminCheck == true).OrderByDescending(m => m.CreatedOn).Select(a => new { a.Region, a.Title, a.IsGlobal, a.ImageUrl, Url = "/article/" + a.NewsLabels + "/" + a.NewsId.ToString() }).ToList();
        //    int pageSize = 10;
        //    int pageNumber = (moreItemsPageIndex ?? 1);
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);

        //    }
        //    var resultData = search.Select(b => new { b.Title, b.Url, b.ImageUrl }).ToPagedList(pageNumber, pageSize);
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}
        //public PartialViewResult GetEvents()
        //{
        //    DateTime startToday = DateTime.Today;
        //    DateTime endToday = DateTime.Today.AddDays(1).AddTicks(-1);
        //    var resultList = _db.Events.Where(a => a.AdminCheck == true && (a.StartDate > startToday || (a.StartDate <= startToday && a.EndDate > startToday || a.StartDate > startToday && a.EndDate <= endToday || a.StartDate < endToday && a.EndDate > endToday))).OrderBy(m => m.StartDate).Select(a => new LatestNewsView { Id = a.Id, NewId = a.EventId, Title = a.Title, CreatedOn = a.StartDate, ImageUrl = a.FileUrl, Country = a.Location }).AsNoTracking().Take(5).ToList();
        //    return PartialView("_getEvents", resultList);
        //}
        //public PartialViewResult GetUpcomingEvents(int skip = 0)
        //{
        //    ViewBag.skipCount = skip;
        //    DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
        //    var resultList = _db.Events.Where(a => a.AdminCheck == true && a.StartDate > todayDate).OrderBy(m => m.StartDate).Skip(skip).Take(6).Select(a => new LatestNewsView { Id = a.Id, NewId = a.EventId, Title = a.Title, CreatedOn = a.StartDate, ImageUrl = a.FileUrl, Country = a.Location }).ToList();
        //    return PartialView("_getUpcomingEvents", resultList);
        //}
        //public JsonResult GetAmpUpcomingEvents(string __amp_source_origin, int? moreItemsPageIndex)
        //{
        //    DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
        //    var search = _db.Events.Where(a => a.AdminCheck == true && a.StartDate > todayDate).OrderBy(m => m.StartDate).Select(a => new { Url = "/Event/" + a.EventId.ToString(), a.Title, EventDate = a.StartDate.Day, EventMonth = a.StartDate.Month, EventYear = a.StartDate.Year, ImageUrl = a.FileUrl, a.Location });
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
        //    }
        //    int pageSize = 10;
        //    int pageNumber = (moreItemsPageIndex ?? 1);
        //    var resultData = search.ToPagedList(pageNumber, pageSize);
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}
        //public PartialViewResult GetPastEvents(int skip = 0)
        //{
        //    DateTime pastDate = DateTime.Today;
        //    ViewBag.skipCount = skip;
        //    var resultList = _db.Events.Where(a => a.AdminCheck == true && a.EndDate < pastDate).OrderByDescending(m => m.EndDate).Skip(skip).Take(4).Select(a => new LatestNewsView { Id = a.Id, NewId = a.EventId, Title = a.Title, CreatedOn = a.StartDate, ImageUrl = a.FileUrl }).ToList();
        //    return PartialView("_getPastEvents", resultList);
        //}
        //public PartialViewResult GetTodayEvents(int skip = 0)
        //{
        //    DateTime startToday = DateTime.Today;
        //    DateTime endToday = DateTime.Today.AddDays(1).AddTicks(-1);
        //    ViewBag.skipCount = skip;
        //    var resultList = _db.Events.Where(a => a.AdminCheck == true && (a.StartDate <= startToday && a.EndDate > startToday || a.StartDate > startToday && a.EndDate <= endToday || a.StartDate < endToday && a.EndDate > endToday)).OrderByDescending(m => m.StartDate).Skip(skip).Take(4).Select(a => new LatestNewsView { Id = a.Id, NewId = a.EventId, Title = a.Title, CreatedOn = a.StartDate, ImageUrl = a.FileUrl, Country = a.Location }).ToList();
        //    return PartialView("_getTodayEvents", resultList);
        //}
        //public PartialViewResult GetAmpTodayEvents()
        //{
        //    DateTime startToday = DateTime.Today;
        //    DateTime endToday = DateTime.Today.AddDays(1).AddTicks(-1);
        //    var resultList = _db.Events.Where(a => a.AdminCheck == true && (a.StartDate <= startToday && a.EndDate > startToday || a.StartDate > startToday && a.EndDate <= endToday || a.StartDate < endToday && a.EndDate > endToday)).OrderByDescending(m => m.StartDate).Take(5).Select(a => new LatestNewsView { Id = a.Id, NewId = a.EventId, Title = a.Title, CreatedOn = a.StartDate, ImageUrl = a.FileUrl, Country = a.Location }).ToList();
        //    return PartialView("_getAmpTodayEvents", resultList);
        //}
        //public PartialViewResult GetRelatedEvents(string sector, Guid? id)
        //{
        //    var secList = sector.Split(',').ToList();
        //    var search = from m in _db.Events
        //                 where m.AdminCheck == true && m.Id != id
        //                 from s in secList
        //                 where m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s
        //                 select new LatestNewsView
        //                 {
        //                     Id = m.Id,
        //                     NewId = m.EventId,
        //                     Title = m.Title,
        //                     CreatedOn = m.ModifiedOn,
        //                     ImageUrl = m.FileUrl
        //                 };
        //    return PartialView("_getRelatedEvents", search.Distinct().OrderByDescending(a => a.CreatedOn).Take(6).ToList());
        //}
        //public JsonResult GetAmpRelatedEvents(string __amp_source_origin, string sector, Guid? id)
        //{
        //    var secList = sector.Split(',').ToList();
        //    var search = from m in _db.Events
        //                 where m.AdminCheck == true && m.Id != id
        //                 from s in secList
        //                 where m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s
        //                 select new
        //                 {
        //                     m.Title,
        //                     Url = "/Event/" + m.EventId.ToString(),
        //                     ImageUrl = m.FileUrl,
        //                     m.CreatedOn
        //                 };
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
        //    }
        //    var resultData = search.Distinct().OrderByDescending(m => m.CreatedOn).ToPagedList(1, 3);
        //    return Json(new { items = resultData, hasMorePages = resultData.Any() }, JsonRequestBehavior.AllowGet);
        //}
        //public PartialViewResult GetEditorsPick(string reg = "Global Edition")
        //{
        //    ViewBag.reg = reg;
        //    if (reg != "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(reg) && a.EditorPick == true && a.IsSponsored == false).OrderByDescending(m => m.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Take(4).ToList();
        //        return PartialView("_getEditorsPick", result);
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.EditorPick == true && a.IsSponsored == false).OrderByDescending(m => m.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Take(4).ToList();
        //        return PartialView("_getEditorsPick", result);
        //    }
        //}
        public PartialViewResult GetVideoNews(long id = 0, string reg = "Global Edition", string filter = "")
        {
            ViewBag.filter = filter;
            if (reg == "Global Edition")
            {
                var result = _db.VideoNews
                //.Where(a => a.AdminCheck == true)
                .Select(a => new VideoViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    CreatedOn = a.CreatedOn,
                    FileThumbUrl = a.VideoThumbUrl,
                    Duration = a.Duration
                }).OrderByDescending(m => m.CreatedOn).Take(5);
                return PartialView("_getVideoNews", result);
            }
            else
            {
                var result = _db.VideoNews
                //.Where(a => a.AdminCheck == true && a.VideoNewsRegions.Any(r => r.Edition.Title == reg))
                .Select(a => new VideoViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    CreatedOn = a.CreatedOn,
                    FileThumbUrl = a.VideoThumbUrl,
                    Duration = a.Duration
                }).OrderByDescending(m => m.CreatedOn).Take(5);
                return PartialView("_getVideoNews", result);
            }
        }
        ////public PartialViewResult GetComments(long id, string type, int skip)
        ////{
        ////    ViewBag.loginUser = User.Identity.GetUserId();
        ////    var search = _db.Comments.Where(a => a.CommentItemId == id && a.CommentItemType == type && a.ParentId == new Guid()).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(5).ToList();
        ////    return PartialView("_getComments", search);
        ////}
        ////public PartialViewResult ReplyOnComments(Guid parentId)
        ////{
        ////    var search = from m in _db.Comments
        ////                 where m.ParentId == parentId
        ////                 select m;
        ////    return PartialView("_getReplyComments", search.OrderByDescending(a => a.CreatedOn).ToList());
        ////}
        //// For Ticker
        //public PartialViewResult GetLatestNews(int skip = 0, string reg = "Global Edition")
        //{
        //    ViewBag.reg = reg;
        //    if (reg != "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(6).ToList();
        //        return PartialView("_getLatestNews", result);
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.IsSponsored == false && a.Sector != "0").OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(6).ToList();
        //        return PartialView("_getLatestNews", result);
        //    }
        //}
        //[OutputCache(Duration = 60, VaryByParam = "reg")]
        //public PartialViewResult GetTags(string reg = "Global Edition")
        //{
        //    List<string> result = new List<string>();
        //    var search = _db.DevNews.Where(a => a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(a => a.Tags).AsNoTracking().Take(10).ToList();
        //    foreach (var item in search)
        //    {
        //        if (!String.IsNullOrEmpty(item))
        //        {
        //            var tagarr = item.Split(',').ToList();
        //            foreach (var tag in tagarr)
        //            {
        //                result.Add(tag);
        //            }
        //        }
        //    }
        //    ViewBag.tags = result.Take(10);
        //    return PartialView("_getTags");
        //}
        //public PartialViewResult GetProfile()
        //{
        //    string userId = User.Identity.GetUserId();
        //    var user = _db.Users.Where(a => a.Id == userId).Select(a => new UserView { FirstName = a.FirstName, LastName = a.LastName, AboutMe = a.AboutMe, Address = a.Address, Contact = a.PhoneNumber, Country = a.Country, Email = a.Email, ProfilePic = a.ProfilePic });
        //    ViewBag.sector = _db.DevSectors.OrderBy(a => a.SrNo).ToList();
        //    return PartialView("_getProfile", user);
        //}
        //public PartialViewResult GetInterest()
        //{
        //    string userId = User.Identity.GetUserId();
        //    var search = _db.UserInterests.Where(a => a.UserId == userId && a.InterestType != "AutoDefined").ToList();
        //    return PartialView("_getInterest", search);
        //}
        //[OutputCache(Duration = 10, VaryByParam = "filter")]
        //public PartialViewResult GetRegion(string filter = "")
        //{
        //    ViewBag.filter = filter;
        //    var search = _db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").ToList().OrderBy(a => a.SrNo);
        //    return PartialView("_getRegion", search);
        //}
        //public PartialViewResult GetUserPic()
        //{
        //    string userId = User.Identity.GetUserId();
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        ViewBag.profilePic = _db.Users.Find(userId).ProfilePic;
        //    }
        //    return PartialView("_getUserPic");
        //}
        //[OutputCache(Duration = 10, VaryByParam = "reg; filter")]
        //public PartialViewResult GetSectorMenu(string filter = "", string reg = "Global Edition")
        //{
        //    ViewBag.reg = reg;
        //    ViewBag.filter = filter;
        //    var search = _db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.Title).ToList();
        //    return PartialView("_getSectorMenu", search);
        //}
        //[OutputCache(Duration = 10, VaryByParam = "sector; reg; filter")]
        //public PartialViewResult GetSector(string sector, string reg = "", string filter = "")
        //{
        //    ViewBag.reg = reg;
        //    ViewBag.filter = filter;
        //    if (!string.IsNullOrEmpty(sector))
        //    {
        //        var idList = sector.Split(',').ToList().Select(int.Parse).ToList();
        //        var search = from m in _db.DevSectors
        //                     where m.Id != 8 && m.Id != 16
        //                     join s in idList on m.Id equals s
        //                     select new ItemView
        //                     {
        //                         Id = m.Id,
        //                         Title = m.Title,
        //                     };
        //        if (filter == "Single")
        //        {
        //            search = search.Take(1);
        //        }
        //        return PartialView("_getSector", search.OrderBy(a => a.Title));
        //    }
        //    return PartialView("_getSector");
        //}
        //public PartialViewResult GetThemes(string theme, string filter = "")
        //{
        //    ViewBag.filter = filter;
        //    var idList = theme.Split(',').ToList().Select(int.Parse).ToList();
        //    var search = from m in _db.DevThemes
        //                 join s in idList on m.Id equals s
        //                 select new ItemView
        //                 {
        //                     Id = m.Id,
        //                     Title = m.Title,
        //                 };
        //    return PartialView("_getThemes", search);
        //}
        //public PartialViewResult GetSavedImages(int skip = 0, string sector = "", string title = "")
        //{
        //    ViewBag.skipCount = skip;
        //    var resultList = _db.ImageGallery.ToList().Select(m => new SavedImagesView { Title = m.Title, Sector = m.Sector, ImageUrl = m.ImageUrl, CreatedOn = m.CreatedOn, FileMimeType = m.FileMimeType, FileSize = m.FileSize, Caption = m.Caption, ImageCopyright = m.ImageCopyright, Tags = m.Tags });
        //    if (sector != "")
        //    {
        //        resultList = resultList.Where(m => m.Sector.StartsWith(sector + ",") || m.Sector.Contains("," + sector + ",") || m.Sector.EndsWith("," + sector) || m.Sector == sector).ToList();
        //    }
        //    if (title != "")
        //    {
        //        resultList = resultList.Where(a => a.Title.ToUpper().Contains(title.ToUpper())).ToList();
        //    }
        //    return PartialView("_getSavedImages", resultList.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(20));
        //}
        //public PartialViewResult GetOldSavedImages(int skip = 0, string sector = "", string title = "")
        //{
        //    ViewBag.skipCount = skip;
        //    var resultList = _db.UserFiles.ToList().Select(m => new SavedImagesView { Title = m.Title, Sector = m.FileFor, ImageUrl = m.FileUrl, CreatedOn = m.CreatedOn, FileMimeType = m.FileMimeType, FileSize = m.FileSize, Caption = "", ImageCopyright = "" });
        //    if (sector != "")
        //    {
        //        resultList = resultList.Where(m => m.Sector.StartsWith(sector + ",") || m.Sector.Contains("," + sector + ",") || m.Sector.EndsWith("," + sector) || m.Sector == sector).ToList();
        //    }
        //    if (title != "")
        //    {
        //        resultList = resultList.Where(a => a.Title.ToUpper().Contains(title.ToUpper())).ToList();
        //    }
        //    return PartialView("_getSavedImages", resultList.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(20));
        //}
        //// Json Methods
        //public JsonResult SubscribeNews(string email)
        //{
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return Json("Something went wrong!", JsonRequestBehavior.AllowGet);
        //    }
        //    Subscription obj = new Subscription
        //    {
        //        Title = "Subscriber",
        //        Email = email,
        //        Type = 1
        //    };
        //    _db.Subscriptions.Add(obj);
        //    _db.SaveChanges();
        //    return Json("Successful!", JsonRequestBehavior.AllowGet);
        //}
        //public async void NotifyToParent(string user, string commentText, string type)
        //{
        //    var search = await _db.ActivityLogs.Where(a => a.Activity == type).ToListAsync();
        //    if (search.Any())
        //    {
        //        EmailController email = new EmailController();
        //        foreach (var item in search)
        //        {
        //            string parentEmail = item.ApplicationUsers.Email;
        //            string activityUrl = item.ActivityUrl;
        //            string itemTitle = item.LogDescription;
        //            string EmailTime = DateTime.Today.ToString("dd.MM.yyyy");

        //            string emailData = string.Format("<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
        //                        "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
        //                        "<tr><td  align=\"right\" colspan=\"2\">Sent date: " + EmailTime + "</td></tr>" +
        //                        "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Dear, <br><br> " + user + " commented on " + itemTitle + " at " + EmailTime + "</td></tr>" +
        //                        "<tr><td colspan=\"2\"><table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; width: 100%; color: #555555; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\"><tbody>" +
        //                        commentText +
        //                        "</tbody></table>" +
        //                        "<a href=" + activityUrl + " target=\"_blank\" title=\"More Details\">More Details</a>" +
        //                        "</table>" +
        //                        "</div>");

        //            email.SendMail(parentEmail, emailData, "Devdiscourse Alert!");
        //        }
        //    }
        //}
        //// itemId : News or Blog Id
        //public async Task CreateLog(string title, long itemId, string logFor, string username, string email)
        //{
        //    string itemTitle = "";
        //    string activityUrl = "";
        //    if (logFor == "News")
        //    {
        //        activityUrl = "/article/" + itemId;
        //        itemTitle = (await _db.DevNews.Where(a => a.NewsId == itemId).FirstOrDefaultAsync()).Title;
        //    }
        //    else
        //    {
        //        activityUrl = "/Event/" + itemId;
        //        itemTitle = (await _db.Events.Where(a => a.EventId == itemId).FirstOrDefaultAsync()).Title;
        //    }
        //    CommentLog logs = new CommentLog
        //    {
        //        LogTitle = title,
        //        LogDescription = itemTitle,
        //        ItemId = itemId.ToString(),
        //        Username = username,
        //        Email = email,
        //        ItemType = logFor,
        //        ActivityUrl = activityUrl,
        //        ActivityDate = DateTime.Now,
        //        IsRead = false
        //    };
        //    _db.CommentLogs.Add(logs);
        //    await _db.SaveChangesAsync();
        //}
        public JsonResult GetCountry(string region)
        {
            var regList = region?.Split(',').ToList();
            if (regList == null || regList.Contains("Global Edition"))
            {
                var search = from m in _db.Countries
                             select new
                             {
                                 m.Title,
                             };
                return Json(search.OrderBy(a => a.Title).ToList());
            }
            else
            {
                //var search = from m in _db.Countries
                //             join s in regList on m.Regions.Title equals s
                //             select new
                //             {
                //                 m.Title,
                //             };
                //return Json(search.OrderBy(a => a.Title).ToList());
                var query = from m in _db.Countries
                            select new
                            {
                                m.Title,
                                RegionTitle = m.Regions.Title // Include the region title for comparison
                            };

                var fetchedData = query.ToList(); // Fetch the data from the database

                var search = fetchedData
                    .Where(m => regList.Contains(m.RegionTitle)) // Filter based on regList
                    .Select(m => new { m.Title })
                    .OrderBy(a => a.Title)
                    .ToList();

                return Json(search);
            }
        }
        //public JsonResult UpdateProfilePic()
        //{
        //    if (Request.Files.Count > 0)
        //    {
        //        string returnUrl = "";
        //        for (int i = 0; i < Request.Files.Count; i++)
        //        {
        //            var file = Request.Files[i];
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var fileName = RandomName();
        //                var fileExtension = Path.GetExtension(file.FileName);
        //                var fileKey = Request.Files.Keys[i];
        //                var path = Path.Combine(Server.MapPath("~/AdminFiles/UserProfile/"), fileName + fileExtension);
        //                file.SaveAs(path);
        //                returnUrl = "/AdminFiles/UserProfile/" + fileName + fileExtension;
        //            }
        //        }
        //        return Json(returnUrl, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json("Error", JsonRequestBehavior.AllowGet);
        //}
        //public string RandomName()
        //{
        //    DateTime time = DateTime.UtcNow.ToLocalTime();
        //    return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        //}
        //public JsonResult UpdateProfile(string fname, string lname, string aboutme, string address, string contact, string country, string email, string profile)
        //{
        //    var user = _db.Users.Find(User.Identity.GetUserId());
        //    user.FirstName = fname;
        //    user.LastName = lname;
        //    user.AboutMe = aboutme;
        //    user.Address = address;
        //    user.PhoneNumber = contact;
        //    user.Country = country;
        //    user.Email = email;
        //    user.ProfilePic = profile;
        //    _db.Entry(user).State = EntityState.Modified;
        //    _db.SaveChanges();
        //    return Json("Success!", JsonRequestBehavior.AllowGet);
        //}
        //public async Task<JsonResult> AddAutoDefinedInterest(string sector, string type, string userId)
        //{
        //    var sectorList = sector.Split(',').ToList();
        //    foreach (var _sector in sectorList)
        //    {
        //        UserInterest interest = new UserInterest
        //        {
        //            UserId = userId,
        //            Sector = _sector,
        //            InterestType = type
        //        };
        //        _db.UserInterests.Add(interest);
        //        await _db.SaveChangesAsync();
        //    }
        //    return Json("Success!", JsonRequestBehavior.AllowGet);
        //}
        //[Authorize]
        //public JsonResult AddInterest(string sector, string type)
        //{
        //    var sectorArr = sector.Split(',');
        //    string userId = User.Identity.GetUserId();
        //    for (int i = 0; i < sectorArr.Length; i++)
        //    {
        //        string _sector = sectorArr[i];
        //        // Create Interest
        //        UserInterest interest = new UserInterest
        //        {
        //            UserId = userId,
        //            Sector = _sector,
        //            InterestType = type
        //        };
        //        _db.UserInterests.Add(interest);
        //        _db.SaveChanges();
        //    }
        //    return Json("Success!", JsonRequestBehavior.AllowGet);
        //}
        //public async Task<JsonResult> ViewsCount(Guid id)
        //{
        //    var search = await _db.DevNews.FindAsync(id);
        //    search.ViewCount = search.ViewCount + 1;
        //    _db.Entry(search).State = EntityState.Modified;
        //    await _db.SaveChangesAsync();
        //    return Json(search.ViewCount, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult SendNotification()
        //{
        //    //EmailController email = new EmailController();
        //    ////Get Today News
        //    //DateTime startDateTime = DateTime.Today; //Today at 00:00:00
        //    //DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
        //    //var EmailTime = startDateTime.ToString("dd.MM.yyyy");
        //    //var subscriber = new List<string>();
        //    //var newsData = await (from p in _db.DevNews
        //    //                      join po in _db.DevSectors
        //    //                      on p.Sector equals po.Id.ToString()
        //    //                      where p.AdminCheck == true && p.IsGlobal == true && p.ModifiedOn >= startDateTime && p.ModifiedOn <= endDateTime && p.IsSponsored == false
        //    //                      orderby p.ViewCount descending
        //    //                      select new EmailViewModel
        //    //                      {
        //    //                          NewsId = p.NewsId,
        //    //                          Title = p.Title,
        //    //                          Description = p.Description,
        //    //                          ImageUrl = p.ImageUrl,
        //    //                          Sector = p.Sector
        //    //                      })
        //    //                    .GroupBy(x => x.Sector)
        //    //                    .SelectMany(x => x.Take(1))
        //    //                    .AsNoTracking()
        //    //                    .ToListAsync();

        //    //var subscriberUser = await _db.Subscriptions.Select(a => a.Email).ToListAsync();
        //    ////Email to Registered User
        //    //var RegisteredUsers = await _db.Users.Where(a => a.EmailConfirmed).Select(a => a.Email).ToListAsync();
        //    //if (RegisteredUsers.Count() > 0)
        //    //{
        //    //    subscriber = subscriberUser.Union(RegisteredUsers).Distinct().ToList();
        //    //}
        //    //else
        //    //{
        //    //    subscriber = subscriberUser.Distinct().ToList();
        //    //}
        //    //List<string> listofNews = new List<string>();
        //    //if (newsData.Any())
        //    //{
        //    //    foreach (var item in newsData)
        //    //    {
        //    //        listofNews.Add(string.Format("<tr>" +
        //    //               "<td width=\"100\" rowspan=\"2\" style=\"padding:10px;\">" +
        //    //               "<a href=\"\" target=\"_blank\" title=\"This external link will open in a new window\">" +
        //    //               "<img alt=\"News\" src=\"https://www.devdiscourse.com/" + item.ImageUrl + "?w=100\"></a></td>" +
        //    //               "<td style=\"padding:10px;\">" +
        //    //               "<a href=\"https://www.devdiscourse.com/article/" + item.Slug() + "\" style=\"color:##0090d4;text-decoration:none;\" target =\"_blank\" title=\"This external link will open in a new window\">" + item.Title + "</a>" +
        //    //               "</td></tr><tr><td style=\"padding:10px;\">" + item.GetFirstParagraph() +
        //    //               "</td></tr>"));
        //    //    }
        //    //    string combindedString = string.Join("", listofNews.ToArray());
        //    //    string emailData = string.Format("<img src=\"https://www.devdiscourse.com/AdminFiles/Logo/Mail_Banner.jpg\" style=\"width:100%;max-width:800px;\"/>" +
        //    //        "<div style=\"padding: 0px 15px 0px;max-width: 770px;\">" +
        //    //        "<table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px;  width:100%; color: #555555; border: 0 none transparent; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\" >" +
        //    //        "<tr><td  align=\"right\" colspan=\"2\">Sent date: " + EmailTime + "</td></tr>" +
        //    //        "<tr><td style=\"padding: 30px 0 10px 0;\" colspan=\"2\">Dear colleague, <br><br>   Top News posted today on <a href=\"https://www.devdiscourse.com/\" target =\"_blank\" title=\"This external link will open in a new window\">Devdiscourse</a> are listed below. To view all the News, <a href=\"https://www.devdiscourse.com/Home/Search/All\" target=\"_blank\" title=\"This external link will open in a new window\">click here. </a></td></tr>" +
        //    //        "<tr><td colspan=\"2\"><table style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; width: 100%; color: #555555; border-collapse: collapse; border-spacing: 0; empty-cells: hide; table-layout: auto;\"><tbody>" +
        //    //        combindedString +
        //    //        "</tbody></table></td></tr>" +
        //    //        "<tr><td style=\"padding:20px 0;\" colspan=\"2\">Thank you, <br> The Devdiscourse team</td></tr>" +
        //    //        "<tr style=\"background-color:#e1e1e1;font-size:12px;\"><td style=\"padding: 30px 0 30px 30px; width: 85%; vertical-align: bottom;\">" +
        //    //        "<div>For customized alerts, <a href=\"https://www.devdiscourse.com/Account/Register\" target =\"_blank\" title=\"This external link will open in a new window\">Register</a> with us.</div>" +
        //    //        "<div>If you have any questions or concerns, <a href=\"https://www.devdiscourse.com/AboutUs#meet\" target =\"_blank\" title=\"This external link will open in a new window\">contact us</a> for assistance.</div>" +
        //    //        "<br><div style=\"color: #696969;\"> © Copyright 2018 <a href=\"http://www.visionri.com/\" style =\"color:#222;text-decoration:unset;\"> VisionRI</a></div></td>" +
        //    //        "<td style=\"padding: 0 30px 30px 0; width: 15%; vertical-align: bottom;\" ><div style= \"width: 100%; text-align: center;\" > Follow us:</div>" +
        //    //        "<br><div style =\"width: 100%; text-align: center;\">" +
        //    //        "<a href=\"https://www.facebook.com/devdiscourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"https://www.devdiscourse.com/AdminFiles/Logo/facebook.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
        //    //        "<a href=\"https://twitter.com/dev_discourse\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"https://www.devdiscourse.com/AdminFiles/Logo/twitter.png\" alt=\"\"></a>&nbsp;&nbsp;&nbsp;" +
        //    //        "<a href=\"https://www.linkedin.com/showcase/dev_discourse/\" target=\"_blank\" title=\"This external link will open in a new window\"><img src=\"https://www.devdiscourse.com/AdminFiles/Logo/linkedin.png\" alt=\"\"></a>" +
        //    //        "</div></td></tr>" +
        //    //        "</table>" +
        //    //        "</div>");

        //    //    var EmailDataTask = Task.Factory.StartNew(() =>
        //    //    {

        //    //        var EmailTask = Task.Factory.StartNew(() =>
        //    //        {
        //    //            if (subscriber.Any())
        //    //            {
        //    //                Parallel.ForEach(subscriber, recieverEmail =>
        //    //                {
        //    //                    var message = new MailMessage();
        //    //                    message.To.Add(new MailAddress(recieverEmail)); // replace with valid value 
        //    //                    message.From = new MailAddress("noreply@devdiscourse.com"); // replace with valid value
        //    //                    message.Subject = "Devdiscourse News";
        //    //                    message.Body = string.Format(emailData, "DevDiscourse");
        //    //                    message.IsBodyHtml = true;
        //    //                    SmtpClient client = new SmtpClient("in.mailjet.com", 587)
        //    //                    {
        //    //                        Credentials = new NetworkCredential("f81887b0a57aa0603312a98443d32f40", "84d180273651fd6cfe3b95b5b4d653b7"),
        //    //                        EnableSsl = true
        //    //                    };
        //    //                    client.Send(message);
        //    //                });
        //    //            }
        //    //        });

        //    //    });
        //    //    EmailDataTask.Wait();
        //    //}
        //    return Json("Successfull", JsonRequestBehavior.AllowGet);
        //}
        //public string TruncateDescription(string htmlString)
        //{
        //    string htmlTagPattern = "<.*?>";
        //    var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //    htmlString = regexCss.Replace(htmlString, string.Empty);
        //    htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
        //    htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
        //    htmlString = htmlString.Replace("&nbsp;", string.Empty);
        //    htmlString = htmlString.Replace("&ldquo;", "\"");
        //    htmlString = htmlString.Replace("&rdquo;", "\"");
        //    htmlString = htmlString.Replace("&lsquo;", "'");
        //    htmlString = htmlString.Replace("&rsquo;", "'");

        //    if (htmlString.Length <= 100)
        //    {
        //        return htmlString;
        //    }
        //    else
        //    {
        //        return htmlString.Substring(0, 100) + "...";
        //    }

        //}
        //public async Task<JsonResult> SendOldNotification()
        //{
        //    EmailController email = new EmailController();
        //    //Get Today News
        //    DateTime startDateTime = DateTime.Today; //Today at 00:00:00
        //    DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
        //    var newsData = await _db.DevNews.Where(a => a.ModifiedOn >= startDateTime && a.ModifiedOn <= endDateTime && a.AdminCheck).Select(n => new { n.Id, n.Title, n.Description, n.Sector }).ToListAsync();

        //    var subscriber = await _db.Subscriptions.ToListAsync();
        //    //var newsData = await _db.SubscribeNews.ToListAsync();
        //    List<string> listofNews = new List<string>();
        //    string newsList = "";
        //    if (newsData.Any())
        //    {
        //        foreach (var item in newsData.Take(1))
        //        {
        //            var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });

        //            newsList = string.Format("<div style='padding:15px;background-color:#ededed'><p style='font-size:24px;'>" + item.Title + "</p><p style='font-size:16px;'>" + item.Description + "</p><a href ='www.devdiscourse.com" + newsUrl + "' style='font-size:16px'>Read More</a></div>");
        //        }
        //        if (newsData.Skip(1).Any())
        //        {
        //            foreach (var item in newsData.Skip(1).Take(9))
        //            {
        //                var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });
        //                listofNews.Add(string.Format("<li style='font-size:16px;line-height:1.9;margin-left:unset;'><a href='www.devdiscourse.com" + newsUrl + "' style='color:#222'>" + item.Title + "</a></li>"));
        //            }
        //        }
        //        if (subscriber.Any())
        //        {
        //            foreach (var item in subscriber)
        //            {
        //                if (!String.IsNullOrEmpty(item.Email) && newsData.Count() > 0)
        //                {
        //                    string combindedString = string.Join("", listofNews.ToArray());
        //                    string emailData = string.Format("<div style='padding-left:15%;width:60%;text-align:justify;padding-top:30px;padding-bottom:20px;'><div style='margin-left:32%'><a href='http://devdiscourse.com/'><img src='www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></a></div>" +
        //                         "<div>" + newsList + "</div><p style='text-align:center;font-size:20px;color:red;'>More form Devdiscourse :</p><ol>" + combindedString + "</ol><div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
        //                         "© Copyright 2017 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                         "<span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                        "<span><a href='http://devdiscourse.com/Home/TermsConditions' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");
        //                    // email.SendMail(item.Email, emailData, "Devdiscourse News");
        //                }
        //            }
        //        }
        //        //foreach (var item in newsData)
        //        //{
        //        //    _db.SubscribeNews.Remove(item);
        //        //    await _db.SaveChangesAsync();
        //        //}
        //    }
        //    //Email to Registered User
        //    var RegisteredUsers = await _db.Users.Where(a => a.EmailConfirmed).ToListAsync();
        //    foreach (var user in RegisteredUsers)
        //    {
        //        listofNews.Clear();
        //        if (user.UserInterests.Count(s => s.InterestType != "AutoDefined") != 0)
        //        {
        //            var newNewsData = from m in newsData
        //                              from s in user.UserInterests
        //                              where s.InterestType != "AutoDefined"
        //                              where (m.Sector.StartsWith(s.Sector + ",") || m.Sector.Contains("," + s.Sector + ",") || m.Sector.EndsWith("," + s.Sector) || m.Sector == s.Sector)
        //                              select new
        //                              {
        //                                  m.Id,
        //                                  m.Title,
        //                                  m.Description
        //                              };
        //            var newNewsList = newNewsData.Distinct().Take(10).ToList();
        //            foreach (var item in newNewsList.Take(1))
        //            {
        //                var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });

        //                newsList = string.Format("<div style='padding:15px;background-color:#ededed'><p style='font-size:24px;'>" + item.Title + "</p><p style='font-size:16px;'>" + item.Description + "</p><a href ='www.devdiscourse.com" + newsUrl + "' style='font-size:16px'>Read More</a></div>");
        //            }
        //            if (newNewsList.Skip(1).Any())
        //            {
        //                foreach (var item in newNewsList.Skip(1).Take(9))
        //                {
        //                    var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });
        //                    listofNews.Add(string.Format("<li style='font-size:16px;line-height:1.9;margin-left:unset;'><a href='www.devdiscourse.com" + newsUrl + "' style='color:#222'>" + item.Title + "</a></li>"));
        //                }
        //            }
        //            if (!String.IsNullOrEmpty(user.Email) && newNewsList.Count() > 0)
        //            {
        //                string combindedString = string.Join("", listofNews.ToArray());
        //                string emailData = string.Format("<div style='padding-left:15%;width:60%;text-align:justify;padding-top:30px;padding-bottom:20px;'><div style='margin-left:32%'><a href='http://devdiscourse.com/'><img src='www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></a></div>" +
        //                        "<div>" + newsList + "</div><p style='text-align:center;font-size:20px;color:red;'>More form Devdiscourse :</p><ol>" + combindedString + "</ol><div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
        //                        "© Copyright 2017 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                        "<span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                    "<span><a href='http://devdiscourse.com/Home/TermsConditions' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");
        //                email.SendMail(user.Email, emailData, "Devdiscourse News");
        //            }
        //        }
        //        else
        //        {
        //            var newNewsList = newsData.Distinct().Take(10).ToList();
        //            foreach (var item in newNewsList.Take(1))
        //            {
        //                var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });
        //                var Description = item.Description.Length > 400 ? item.Description.Substring(0, 400) + "..." : item.Description;
        //                newsList = string.Format("<div style='padding:15px;background-color:#ededed'><p style='font-size:24px;'>" + item.Title + "</p><p style='font-size:18px;line-height:1.2;'>" + Description + "</p><a href ='www.devdiscourse.com" + newsUrl + "' style='font-size:16px;display:inline;'>Read More</a></div>");
        //            }
        //            if (newNewsList.Skip(1).Any())
        //            {
        //                foreach (var item in newNewsList.Skip(1).Take(9))
        //                {
        //                    var newsUrl = Url.Action("Detail", "Home", new { id = item.Id });
        //                    listofNews.Add(string.Format("<li style='font-size:16px;line-height:1.9;margin-left:unset;'><a href='www.devdiscourse.com" + newsUrl + "' style='color:#222'>" + item.Title + "</a></li>"));
        //                }
        //            }
        //            if (!String.IsNullOrEmpty(user.Email) && newNewsList.Count() > 0)
        //            {
        //                string combindedString = string.Join("", listofNews.ToArray());
        //                string emailData = string.Format("<div style='padding-left:15%;width:60%;text-align:justify;padding-top:30px;padding-bottom:20px;'><div style='margin-left:32%'><a href='http://devdiscourse.com/'><img src='www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></a></div>" +
        //                        "<div>" + newsList + "</div><p style='text-align:center;font-size:20px;color:red;'>More form Devdiscourse :</p><ol>" + combindedString + "</ol><div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
        //                        "© Copyright 2017 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                        "<span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                    "<span><a href='http://devdiscourse.com/Home/TermsConditions' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");
        //                email.SendMail(user.Email, emailData, "Devdiscourse News");
        //            }
        //        }
        //    }
        //    return Json("Successfull", JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult SubmitFeedback(string name, string email, string message)
        //{
        //    Feedback obj = new Feedback
        //    {
        //        Name = name,
        //        Email = email,
        //        Message = message
        //    };
        //    _db.Feedbacks.Add(obj);
        //    _db.SaveChanges();
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        //public void DeleteInterest()
        //{
        //    var search = _db.UserInterests.ToList();
        //    foreach (var item in search)
        //    {
        //        _db.UserInterests.Remove(item);
        //        _db.SaveChanges();
        //    }
        //}
        //public string UpdateInfo(string id, string code)
        //{
        //    var search = _db.SDGSamurais.Where(a => a.Creator == id && a.ReferenceCode == code).FirstOrDefault();
        //    if (search != null)
        //    {
        //        return "Already used code";
        //    }
        //    else
        //    {
        //        search.ReferenceCode = code;
        //        _db.Entry(search).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }
        //    return "Success";
        //}
        //public void ViewCount(long id, int count)
        //{
        //    var search = _db.DevNews.Where(a => a.NewsId == id).FirstOrDefault();
        //    search.ViewCount = count;
        //    _db.Entry(search).State = EntityState.Modified;
        //    _db.SaveChanges();
        //}
        //public async Task<string> UpdateInfocusData()
        //{
        //    var infocus = await (from m in _db.Infocus
        //                         join s in _db.DevNews on m.NewsId equals s.NewsId
        //                         select s).ToListAsync();
        //    foreach (var item in infocus)
        //    {
        //        item.IsInfocus = true;
        //        _db.Entry(item).State = EntityState.Modified;
        //    }
        //    await _db.SaveChangesAsync();
        //    return "Ok";
        //}
        //public PartialViewResult NewsBySector(string reg, string sector, string url, string title)
        //{
        //    ViewBag.url = url;
        //    ViewBag.reg = reg;
        //    ViewBag.title = title;
        //    var infocusData = _db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
        //    if (reg == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && (a.Sector.StartsWith(sector + ",") || a.Sector.Contains("," + sector + ",") || a.Sector.EndsWith("," + sector) || a.Sector == sector) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Subtitle = a.SubTitle,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Sector = a.Type,
        //            SubType = a.SubType,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(10);
        //        return PartialView("_getNewsBySector", result.ToList());
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && (a.Sector.StartsWith(sector + ",") || a.Sector.Contains("," + sector + ",") || a.Sector.EndsWith("," + sector) || a.Sector == sector) && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Subtitle = a.SubTitle,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Sector = a.Type,
        //            SubType = a.SubType,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(10);
        //        return PartialView("_getNewsBySector", result.ToList());
        //    }
        //}
        //[OutputCache(Duration = 60, VaryByParam = "*")]
        //public PartialViewResult GetSectorNews(string reg, string sector)
        //{
        //    ViewBag.Sector = sector;
        //    DateTime twoMonth = DateTime.Today.AddDays(-30);
        //    if (reg == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => a.CreatedOn > twoMonth && a.AdminCheck == true && (a.Sector.StartsWith(sector + ",") || a.Sector.Contains("," + sector + ",") || a.Sector.EndsWith("," + sector) || a.Sector == sector) && a.IsSponsored == false).OrderByDescending(a => a.PublishedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Subtitle = a.SubTitle,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Sector = a.Type,
        //            SubType = a.SubType,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(6);
        //        return PartialView("_getSectorNews", result.ToList());
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => a.CreatedOn > twoMonth && a.AdminCheck == true && (a.Sector.StartsWith(sector + ",") || a.Sector.Contains("," + sector + ",") || a.Sector.EndsWith("," + sector) || a.Sector == sector) && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.PublishedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Subtitle = a.SubTitle,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Sector = a.Type,
        //            SubType = a.SubType,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(6);
        //        return PartialView("_getSectorNews", result.ToList());
        //    }
        //}
        //[OutputCache(Duration = 60, VaryByParam = "*")]
        //public PartialViewResult GetInterview(string reg)
        //{
        //    DateTime thirtyDays = DateTime.Today.AddDays(-30);
        //    if (reg == "Global Edition")
        //    {
        //        var result = (from m in _db.Infocus
        //                      where m.Edition == "Universal Edition" && m.ItemType == "Interview"
        //                      join a in _db.DevNews on m.NewsId equals a.NewsId
        //                      where a.AdminCheck == true
        //                      select new NewsViewModel
        //                      {
        //                          Title = a.Title,
        //                          ImageUrl = a.ImageUrl,
        //                          NewsId = a.NewsId,
        //                          Label = a.NewsLabels,
        //                          Country = a.Country,
        //                          SrNo = m.SrNo
        //                      }).OrderBy(a => a.SrNo).AsNoTracking().Take(4);
        //        return PartialView("_getInterview", result.ToList());
        //    }
        //    else
        //    {
        //        var result = (from m in _db.Infocus
        //                      where m.Edition == reg && m.ItemType == "Interview"
        //                      join a in _db.DevNews on m.NewsId equals a.NewsId
        //                      where a.AdminCheck == true
        //                      select new NewsViewModel
        //                      {
        //                          Title = a.Title,
        //                          ImageUrl = a.ImageUrl,
        //                          NewsId = a.NewsId,
        //                          Label = a.NewsLabels,
        //                          Country = a.Country,
        //                          SrNo = m.SrNo
        //                      }).OrderBy(a => a.SrNo).AsNoTracking().Take(4);
        //        return PartialView("_getInterview", result.ToList());
        //    }
        //}
        //[OutputCache(Duration = 120, VaryByParam = "*")]

        public PartialViewResult GetOpinion(string reg)
        {
            reg = reg == "Global Edition" ? "" : reg;
            DateTime thirtyDays = DateTime.Today.AddDays(-15);
            if (reg == "")
            {
                var search = (from a in _db.DevNews
                              where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.SubType != "Interview"
                              orderby a.PublishedOn descending
                              select new NewsViewModel
                              {
                                  Title = a.Title,
                                  CreatedOn = a.PublishedOn,
                                  ImageUrl = a.ImageUrl,
                                  SubType = a.Themes,
                                  Subtitle = a.Description,
                                  Country = a.Author,
                                  NewsId = a.NewsId,
                                  Label = a.NewsLabels
                              }).Take(10);
                //.AsNoTracking();
                return PartialView("_getOpinion", search.ToList());
            }
            else
            {
                var search = (from a in _db.DevNews
                              where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.Region.Contains(reg) && a.SubType != "Interview"
                              orderby a.PublishedOn descending
                              select new NewsViewModel
                              {
                                  Title = a.Title,
                                  CreatedOn = a.PublishedOn,
                                  ImageUrl = a.ImageUrl,
                                  SubType = a.Themes,
                                  Subtitle = a.Description,
                                  Country = a.Author,
                                  NewsId = a.NewsId,
                                  Label = a.NewsLabels
                              }).Take(10);
                //.AsNoTracking();
                return PartialView("_getOpinion", search.ToList());
            }
        }

        //public PartialViewResult GetLabelNews(string reg, string label, string url, string title, int take)
        //{
        //    ViewBag.url = url;
        //    ViewBag.title = title;
        //    var infocusData = _db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
        //    if (reg == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.NewsLabels == label && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Sector = a.Type,
        //            SubType = a.SubType,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(take);
        //        return PartialView("_getLabelNews", result.ToList());
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.NewsLabels == label && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Sector = a.Type,
        //            SubType = a.SubType,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(take);
        //        return PartialView("_getLabelNews", result.ToList());
        //    }
        //}
        //public async Task<JsonResult> GetLabelNewsForAmp(string region, string label)
        //{
        //    var infocusData = _db.Infocus.Where(a => a.Edition == region).Select(a => a.NewsId);
        //    if (region == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && (a.NewsLabels.StartsWith(label + ",") || a.NewsLabels.Contains("," + label + ",") || a.NewsLabels.EndsWith("," + label) || a.NewsLabels == label) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new
        //        {
        //            a.Title,
        //            defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //            ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl,
        //            Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId,
        //            a.Country
        //        }).AsNoTracking().Take(10);
        //        return Json(new { items = await result.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && (a.NewsLabels.StartsWith(label + ",") || a.NewsLabels.Contains("," + label + ",") || a.NewsLabels.EndsWith("," + label) || a.NewsLabels == label) && a.Region.Contains(region) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new
        //        {
        //            a.Title,
        //            defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //            ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl,
        //            Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId,
        //            a.Country
        //        }).AsNoTracking().Take(10);
        //        return Json(new { items = await result.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public async Task<JsonResult> GetsectorNewsforAmp(string edition, string sectorId, string __amp_source_origin)
        //{
        //    // var infocusData = _db.Infocus.Where(a => a.Edition == edition).Select(a => a.NewsId);
        //    if (!string.IsNullOrEmpty(__amp_source_origin))
        //    {
        //        HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);

        //    }
        //    if (edition == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && (a.Sector.StartsWith(sectorId + ",") || a.Sector.Contains("," + sectorId + ",") || a.Sector.EndsWith("," + sectorId) || a.Sector == sectorId) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new
        //        {
        //            a.Title,
        //            defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //            ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl,
        //            Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId,
        //            a.Country
        //        }).AsNoTracking().Take(10);
        //        return Json(new { items = await result.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && (a.Sector.StartsWith(sectorId + ",") || a.Sector.Contains("," + sectorId + ",") || a.Sector.EndsWith("," + sectorId) || a.Sector == sectorId) && a.Region.Contains(edition) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new
        //        {
        //            a.Title,
        //            defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //            ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl,
        //            Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId,
        //            a.Country
        //        }).AsNoTracking().Take(10);
        //        return Json(new { items = await result.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public async Task<JsonResult> GetVideoAmpNews(string region = "Global Edition")
        //{
        //    if (region != "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(region) && a.IsVideo == true && a.EditorPick == false && a.IsSponsored == false).OrderByDescending(m => m.CreatedOn).Select(a => new
        //        {
        //            a.Title,
        //            defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //            ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl,
        //            Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId,
        //            a.Country
        //        }).AsNoTracking().Take(5);
        //        return Json(new { items = await result.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.IsVideo == true && a.EditorPick == false && a.IsSponsored == false).OrderByDescending(m => m.CreatedOn).Select(a => new
        //        {
        //            a.Title,
        //            defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //            ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl,
        //            Url = "/article/" + (a.NewsLabels ?? "agency-wire") + "/" + a.NewsId,
        //            a.Country
        //        }).AsNoTracking().Take(5);
        //        return Json(new { items = await result.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public async Task<ActionResult> VideoAmp(long? id)
        //{
        //    var search = await _db.VideoNews.FirstOrDefaultAsync(a => a.Id == id);
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

        //public PartialViewResult LatestVideosAmp()
        //{
        //    DateTime todayDate = DateTime.Today.AddDays(-30);
        //    var resultList = _db.VideoNews.Where(a => a.AdminCheck == true).Select(a => new VideoViewModel { Id = a.Id, Title = a.Title, FileThumbUrl = a.VideoThumbUrl, CreatedOn = a.CreatedOn, Duration = a.Duration }).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
        //    return PartialView("_latestVideosAmp", resultList);
        //}
        //public PartialViewResult NewsBySectorTitle(string reg, string sector, string sectorName, string slug)
        //{
        //    ViewBag.reg = reg;
        //    ViewBag.sectorName = sectorName;
        //    ViewBag.slug = slug;
        //    var infocusData = _db.Infocus.Where(a => a.Edition == reg && a.ItemType == "News").Select(a => a.NewsId);
        //    if (reg == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.Sector == sector && a.Type == "News" && a.IsSponsored == false).OrderByDescending(a => a.ModifiedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Subtitle = a.SubTitle,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(2);
        //        return PartialView("_getNewsBySectorTitle", result.ToList());
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.Sector == sector && a.Region.Contains(reg) && a.Type == "News" && a.IsSponsored == false).OrderByDescending(a => a.ModifiedOn).Select(a => new NewsViewModel
        //        {
        //            Title = a.Title,
        //            NewsId = a.NewsId,
        //            ImageUrl = a.ImageUrl,
        //            Subtitle = a.SubTitle,
        //            Country = a.Country,
        //            CreatedOn = a.ModifiedOn,
        //            Label = a.NewsLabels
        //        }).AsNoTracking().Take(2);
        //        return PartialView("_getNewsBySectorTitle", result.ToList());
        //    }
        //}
        //public PartialViewResult AmpNewsBySector(string reg, string sector, int skip = 0, int take = 0)
        //{
        //    ViewBag.skipCount = skip;
        //    ViewBag.reg = reg;
        //    var infocusData = _db.Infocus.Where(a => a.Edition == reg && a.ItemType == "News").Select(a => a.NewsId);
        //    if (reg == "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.Sector == sector && a.Type == "News" && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take);
        //        return PartialView("_getampNewsBySector", result.ToList());
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.AdminCheck == true && a.Sector == sector && a.Region.Contains(reg) && a.Type == "News" && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take);
        //        return PartialView("_getampNewsBySector", result.ToList());
        //    }
        //}
        //public async Task<JsonResult> AmpRelatedNews(long id, string reg, string sector)
        //{
        //    DateTime OneMonth = DateTime.Today.AddDays(-10);
        //    // Array of sectors
        //    var secList = sector.Split(',').ToList();
        //    if (reg == "Global Edition")
        //    {
        //        //from m in _db.DevNews
        //        // from s in secList
        //        // where m.AdminCheck == true && m.CreatedOn > onemonths && m.IsGlobal == true && m.Id != id && (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //        // var dataList = _db.DevNews.Where(m => m.AdminCheck == true && m.CreatedOn>OneMonth && m.NewsLabels!= null && m.IsGlobal== true && m.NewsId != id);
        //        var search = (from m in _db.DevNews
        //                      from s in secList
        //                      where m.AdminCheck == true && m.CreatedOn > OneMonth && m.NewsId != id && (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //                      orderby m.CreatedOn descending
        //                      select new
        //                      {
        //                          title = m.Title,
        //                          image = m.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? m.ImageUrl : "/remote.axd?" + m.ImageUrl,
        //                          url = "/article/" + (m.NewsLabels ?? "agency-wire") + "/" + m.NewsId,
        //                          defaultImage = m.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false
        //                      }).Take(5);
        //        return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        // var dataList = _db.DevNews.Where(m => m.AdminCheck == true && m.CreatedOn > OneMonth && m.Region.Contains(reg) && m.NewsId != id);
        //        var search = (from m in _db.DevNews
        //                      from s in secList
        //                      where m.AdminCheck == true && m.CreatedOn > OneMonth && m.Region.Contains(reg) && m.NewsId != id && (m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s)
        //                      orderby m.CreatedOn descending
        //                      select new
        //                      {
        //                          title = m.Title,
        //                          image = m.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? m.ImageUrl : "/remote.axd?" + m.ImageUrl,
        //                          url = "/article/" + (m.NewsLabels ?? "agency-wire") + "/" + m.NewsId,
        //                          defaultImage = m.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false
        //                      }).Take(5);
        //        return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public async Task<JsonResult> AmpOtherEditionNews(long id)
        //{
        //    DateTime OneMonth = DateTime.Today.AddDays(-5);
        //    var search = (from m in _db.DevNews
        //                  where m.AdminCheck == true && m.CreatedOn > OneMonth && (m.IsGlobal == false || !m.Region.Contains("Global")) && m.NewsId != id && m.NewsLabels != null
        //                  orderby m.CreatedOn descending
        //                  select new
        //                  {
        //                      title = m.Title,
        //                      image = m.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? m.ImageUrl : "/remote.axd?" + m.ImageUrl,
        //                      url = "/article/" + (m.NewsLabels ?? "agency-wire") + "/" + m.NewsId,
        //                      defaultImage = m.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false
        //                  }).Take(5);
        //    return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //}
        //public async Task<JsonResult> AndroidAmpRelatedNews(long id, string reg, string sector)
        //{
        //    // Array of sectors
        //    var secList = sector.Split(',').ToList();
        //    if (reg == "Global Edition")
        //    {
        //        var dataList = _db.DevNews.Where(m => m.AdminCheck == true && m.Type == "News" && m.NewsId != id);
        //        var search = (from m in dataList
        //                      from s in secList
        //                      where m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s
        //                      orderby m.CreatedOn descending
        //                      select new
        //                      {
        //                          title = m.Title,
        //                          image = m.ImageUrl,
        //                          url = "/MobileArticle/" + m.NewsId + "?amp"
        //                      }).Distinct().Take(5);
        //        return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var dataList = _db.DevNews.Where(m => m.AdminCheck == true && m.Region.Contains(reg) && m.Type == "News" && m.NewsId != id);
        //        var search = (from m in dataList
        //                      from s in secList
        //                      where m.Sector.StartsWith(s + ",") || m.Sector.Contains("," + s + ",") || m.Sector.EndsWith("," + s) || m.Sector == s
        //                      orderby m.CreatedOn descending
        //                      select new
        //                      {
        //                          title = m.Title,
        //                          image = m.ImageUrl,
        //                          url = "/MobileArticle/" + m.NewsId + "?amp"
        //                      }).Distinct().Take(5);
        //        return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public async Task<JsonResult> AmpLatestNews(string reg)
        //{
        //    if (reg != "Global Edition")
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.Type == "News" && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, NewId = a.NewsId, Country = a.Country, ImageUrl = a.ImageUrl, Label = a.NewsLabels }).Take(6);
        //        var search = (from m in result
        //                      orderby m.CreatedOn descending
        //                      select new
        //                      {
        //                          country = m.Country,
        //                          datetime = m.CreatedOn,
        //                          title = m.Title,
        //                          image = m.ImageUrl,
        //                          url = "/article/" + (m.Label ?? "agency-wire") + "/" + m.NewId + "?amp"
        //                      }).Distinct().Take(6);
        //        return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var result = _db.DevNews.Where(a => a.AdminCheck == true && a.Type == "News" && a.IsSponsored == false).OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, NewId = a.NewsId, Country = a.Country, ImageUrl = a.ImageUrl, Label = a.NewsLabels }).AsNoTracking().Take(6);
        //        var search = (from m in result
        //                      orderby m.CreatedOn descending
        //                      select new
        //                      {
        //                          country = m.Country,
        //                          datetime = m.CreatedOn,
        //                          title = m.Title,
        //                          image = m.ImageUrl,
        //                          url = "/article/" + (m.Label ?? "agency-wire") + "/" + m.NewId + "?amp"
        //                      }).Distinct().Take(6);
        //        return Json(new { items = await search.ToListAsync() }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public string GenerateSlug(long NewId, string Title)
        //{
        //    string phrase = string.Format("{0}-{1}", NewId, Title);
        //    string str = RemoveAccent(phrase).ToLower();
        //    // invalid chars           
        //    str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        //    // convert multiple spaces into one space   
        //    str = Regex.Replace(str, @"\s+", " ").Trim();
        //    // cut and trim 
        //    str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim();
        //    str = Regex.Replace(str, @"\s", "-"); // hyphens   
        //    return str;
        //}
        //private string RemoveAccent(string text)
        //{
        //    byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
        //    return System.Text.Encoding.ASCII.GetString(bytes);
        //}

        //public async Task<JsonResult> newsGroup()
        //{
        //    DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
        //    DateTime weekend = todayDate.AddDays(-1).AddTicks(1);
        //    var news = await (from p in _db.DevNews
        //                      where p.AdminCheck == true && p.ModifiedOn < todayDate && p.ModifiedOn > weekend && p.IsSponsored == false
        //                      join po in _db.DevSectors
        //                          on p.Sector equals po.Id.ToString()
        //                      orderby p.ViewCount descending
        //                      select new
        //                      {
        //                          p.Title,
        //                          p.NewsId,
        //                          p.ImageUrl,
        //                          p.Sector,
        //                          SectorTitle = po.Title,
        //                          p.ViewCount
        //                      })
        //                        .GroupBy(x => x.Sector)
        //                        .SelectMany(x => x.Take(2))
        //                        .AsNoTracking()
        //                        .ToListAsync();

        //    return Json(news, JsonRequestBehavior.AllowGet);

        //}
        //public async Task<PartialViewResult> NewsGroupSection(string reg)
        //{
        //    //var devsector = _db.DevSectors.Where(s=>s.Id!= 8).ToList();
        //    //var infocusData = _db.Infocus.Where(a => a.Edition == reg && a.ItemType == "News").Select(a => a.NewsId).ToList();
        //    if (reg == "Global Edition")
        //    {
        //        var sectors = await (from s in _db.DevSectors
        //                             where s.Id != 8
        //                             select s).ToListAsync();
        //        var news = from d in _db.DevNews
        //                   where d.AdminCheck == true && d.IsSponsored == false && d.Type == "News"
        //                   select d;
        //        List<NewsViewModel> resultList = new List<NewsViewModel>();
        //        foreach (var item in sectors)
        //        {
        //            var data1 = await (from p in news
        //                               where p.Sector == item.Id.ToString()
        //                               select new NewsViewModel
        //                               {
        //                                   Title = p.Title,
        //                                   NewsId = p.NewsId,
        //                                   ImageUrl = p.ImageUrl,
        //                                   Subtitle = p.SubTitle,
        //                                   Country = p.Country,
        //                                   CreatedOn = p.ModifiedOn,
        //                                   Sector = item.Title,
        //                                   Slug = item.Slug,
        //                                   Label = p.NewsLabels
        //                               }).OrderByDescending(x => x.CreatedOn).Take(2).ToListAsync();
        //            resultList.AddRange(data1);
        //        }
        //        return PartialView("_newsGroupSection", resultList);
        //    }
        //    else
        //    {
        //        var sectors = await (from s in _db.DevSectors
        //                             where s.Id != 8
        //                             select s).ToListAsync();
        //        var news = from d in _db.DevNews
        //                   where d.Region.Contains(reg) && d.AdminCheck == true && d.IsSponsored == false && d.Type == "News"
        //                   select d;
        //        List<NewsViewModel> resultList = new List<NewsViewModel>();
        //        foreach (var item in sectors)
        //        {
        //            var data1 = await (from p in news
        //                               where p.Sector == item.Id.ToString()
        //                               select new NewsViewModel
        //                               {
        //                                   Title = p.Title,
        //                                   NewsId = p.NewsId,
        //                                   ImageUrl = p.ImageUrl,
        //                                   Subtitle = p.SubTitle,
        //                                   Country = p.Country,
        //                                   CreatedOn = p.ModifiedOn,
        //                                   Sector = item.Title,
        //                                   Slug = item.Slug,
        //                                   Label = p.NewsLabels
        //                               }).OrderByDescending(x => x.CreatedOn).Take(2).ToListAsync();
        //            resultList.AddRange(data1);
        //        }
        //        return PartialView("_newsGroupSection", resultList);
        //    }
        //}
        //public async Task<JsonResult> UserLog(string url)
        //{

        //    HttpContext.Response.AddHeader("Cache-Control", "no-cache");
        //    UserLog obj = new UserLog
        //    {
        //        LogTitle = "UserLog",
        //        ActivityUrl = url
        //    };
        //    _db.UserLogs.Add(obj);
        //    await _db.SaveChangesAsync();
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult AuthorArticles(string fl)
        //{
        //    HttpCookie cookie = Request.Cookies["Edition"];
        //    if (cookie == null)
        //    {
        //        ViewBag.region = "Global Edition";
        //    }
        //    else
        //    {
        //        ViewBag.region = cookie.Value ?? "Global Edition";
        //    }
        //    ViewBag.id = fl;
        //    return View();
        //}
        //public PartialViewResult GetAuthorArticles(string id, string region, int? page)
        //{
        //    var result = _db.DevNews.Where(a => a.AdminCheck == true && a.Author == id).OrderByDescending(m => m.CreatedOn).AsNoTracking();
        //    int pageSize = 20;
        //    int pageNumber = (page ?? 1);
        //    return PartialView("_getAuthorArticles", result.ToPagedList(pageNumber, pageSize));
        //}
        //public PartialViewResult GetAuthorList()
        //{
        //    List<AssignedRoleView> result = new List<AssignedRoleView>();
        //    var roles = _db.Roles.Where(a => a.Name == "Admin" || a.Name == "Upfront").ToList();
        //    foreach (var item in roles)
        //    {
        //        var users = _db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(item.Id)).ToList();
        //        foreach (var m in users)
        //        {
        //            result.Add(UserList(m.Id, m.FirstName + " " + m.LastName));
        //        }
        //    }
        //    ViewBag.data = result;
        //    return PartialView("_getAuthor");
        //}
        //public AssignedRoleView UserList(string id, string user)
        //{
        //    AssignedRoleView obj = new AssignedRoleView
        //    {
        //        Id = id,
        //        User = user,
        //    };
        //    return obj;
        //}
        //public ActionResult CentralAfrica()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "Central Africa";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "Central Africa";
        //    return View("HomeNews");
        //}
        //public ActionResult EastAfrica()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "East Africa";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "East Africa";
        //    return View("HomeNews");
        //}
        //public ActionResult NorthAmerica()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "North America";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "North America";
        //    return View("HomeNews");
        //}
        //public ActionResult WestAfrica()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "West Africa";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "West Africa";
        //    return View("HomeNews");
        //}
        //public ActionResult SouthAsia()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "South Asia";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "South Asia";
        //    return View("HomeNews");
        //}
        //public ActionResult EastAndSouthEastAsia()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "East and South East Asia";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "East and South East Asia";
        //    return View("HomeNews");
        //}
        //public ActionResult Pacific()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "Pacific";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "Pacific";
        //    return View("HomeNews");
        //}
        //public ActionResult EuropeAndCentralAsia()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "Europe and Central Asia";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "Europe and Central Asia";
        //    return View("HomeNews");
        //}
        //public ActionResult LatinAmericaAndCaribbean()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "Latin America and Caribbean";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "Latin America and Caribbean";
        //    return View("HomeNews");
        //}
        //public ActionResult MiddleEastAndNorthAfrica()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "Middle East and North Africa";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "Middle East and North Africa";
        //    return View("HomeNews");
        //}
        //public ActionResult SouthernAfrica()
        //{
        //    HttpCookie cookie = new HttpCookie("Edition");
        //    cookie.Value = "Southern Africa";
        //    cookie.Expires = DateTime.UtcNow.AddDays(1);
        //    Response.Cookies.Add(cookie);
        //    ViewBag.edition = "Southern Africa";
        //    return View("HomeNews");
        //}
        //public PartialViewResult GetPreviousNews(long id, string label, string reg = "Global Edition")
        //{
        //    var search = _db.DevNews.FirstOrDefault(a => a.NewsId == id);
        //    DateTime tenDays = search.CreatedOn.AddDays(-3);
        //    reg = reg == "Global Edition" ? "" : reg;
        //    var news = _db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == label).Select(s => new SearchView { Id = s.Id, Title = s.Title, Label = s.NewsLabels, NewsId = s.NewsId, CreatedOn = s.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(1).FirstOrDefault();
        //    return PartialView("GetPreviousNews", news);
        //}
        //public PartialViewResult GetPreviousAppNews(long id, string label, string reg = "Global Edition", int skip = 0)
        //{
        //    var search = _db.DevNews.FirstOrDefault(a => a.NewsId == id);
        //    DateTime tenDays = search.CreatedOn.AddDays(-10);
        //    DevNews news;
        //    if (reg == "Global Edition")
        //    {
        //        if (label != "agency-wire")
        //        {
        //            news = _db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.NewsLabels == label && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(1).FirstOrDefault();
        //        }
        //        else
        //        {
        //            news = _db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.NewsLabels == null && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(1).FirstOrDefault();
        //        }
        //    }
        //    else
        //    {
        //        if (label != "agency-wire")
        //        {
        //            news = _db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == label).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(1).FirstOrDefault();
        //        }
        //        else
        //        {
        //            news = _db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == null).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(1).FirstOrDefault();
        //        }
        //    }
        //    return PartialView("GetPreviousAppNews", news);
        //}

        //public PartialViewResult getTrendNews()
        //{
        //    DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
        //    DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
        //    var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.IsSponsored == false).OrderByDescending(o => o.ViewCount).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(5);
        //    var returnData = search.AsEnumerable().Select(a => new
        //    {
        //        a.Title,
        //        a.Label,
        //        a.Country,
        //        Url = "/mobilearticle/" + a.GenerateSecondSlug().ToString(),
        //        defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
        //        ImageUrl = a.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") == -1 ? a.ImageUrl : "/remote.axd?" + a.ImageUrl
        //    });
        //    return PartialView("GetTrendNews", returnData);
        //}

        //public PartialViewResult getAmpTrendNews()
        //{
        //    DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
        //    DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
        //    var search = _db.DevNews.Where(a => a.AdminCheck == true && a.IsSponsored == false).OrderByDescending(o => o.ViewCount).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(5);
        //    return PartialView("getAmpTrendNews", search);
        //}
        //public string isEmailConfirmed()
        //{
        //    var search = _db.Users.Where(a => a.EmailConfirmed == false);
        //    if (search != null)
        //    {
        //        foreach (var item in search)
        //        {
        //            item.EmailConfirmed = true;
        //            _db.Entry(item).State = EntityState.Modified;
        //        }
        //        _db.SaveChanges();
        //    }
        //    return "OK";
        //}
        //public ActionResult AmpStories()
        //{
        //    var halfDay = DateTime.UtcNow.AddHours(-12);
        //    var infocus = _db.RegionNewsRankings.Where(a => a.DevNews.AdminCheck == true && a.Region.Title == "Global Edition" && a.DevNews.CreatedOn > halfDay && !a.DevNews.Title.Contains("News Summary") && a.DevNews.NewsLabels != "Newsalert" && a.DevNews.Sector != "14" && a.DevNews.Sector != "18" && a.DevNews.Sector != "19" && a.DevNews.Sector != "9").Select(s => new LatestNewsView
        //    {
        //        Id = s.DevNews.Id,
        //        NewId = s.DevNews.NewsId,
        //        Title = s.DevNews.Title,
        //        ImageUrl = s.DevNews.ImageUrl,
        //        CreatedOn = s.DevNews.ModifiedOn,
        //        Type = s.DevNews.Type,
        //        SubType = s.DevNews.SubType,
        //        Country = s.DevNews.Country,
        //        Label = s.DevNews.NewsLabels,
        //        Ranking = s.Ranking
        //    }).OrderByDescending(a => a.CreatedOn).AsNoTracking().Take(65).ToList();
        //    return View(infocus.OrderByDescending(o => o.Ranking).Take(10).ToList());
        //    //var search = (from m in _db.Infocus
        //    //              where m.Edition == "Universal Edition" && m.ItemType=="News"
        //    //              join s in _db.DevNews on m.NewsId equals s.NewsId
        //    //              where s.AdminCheck == true
        //    //              orderby m.SrNo
        //    //              select new LatestNewsView
        //    //              {
        //    //                  Id = s.Id,
        //    //                  NewId = s.NewsId,
        //    //                  Title = s.Title,
        //    //                  ImageUrl = s.ImageUrl,
        //    //                  CreatedOn = s.ModifiedOn,
        //    //                  Type = s.Type,
        //    //                  SubType = s.SubType,
        //    //                  Country = s.Country,
        //    //                  Label = s.NewsLabels,
        //    //                  SrNo = m.SrNo
        //    //              }).AsNoTracking().Take(10);
        //    //return View(search.ToList());
        //}
        ////public JsonResult ReadExcel()
        ////{
        ////    string fileName = Server.MapPath("/AdminFiles/Jan_Visitor_updated.xlsx");
        ////    var workbook = new XLWorkbook(fileName);
        ////    var worksheet = workbook.Worksheet(1);
        ////    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row
        ////    var rowCount = rows.Count();
        ////    List<excelclass> excelclassList = new List<excelclass>();
        ////    foreach (var row in rows)
        ////    {
        ////        var rowNumber = row.RowNumber();
        ////        var newsId = long.Parse((row.Cell(3).Value ?? "0").ToString());
        ////        var pageview = long.Parse((row.Cell(2).Value ?? "0").ToString());
        ////        excelclassList.Add(new excelclass
        ////        {
        ////            //url  = row.Cell(0).Value.ToString(),
        ////            id  = (row.Cell(3).Value??"0").ToString(),
        ////            pageview  = pageview,
        ////            sector  = _db.DevNews.FirstOrDefault(s=>s.NewsId == newsId).Sector??""
        ////        });
        ////    }
        ////    //var excelclassListten = excelclassList.Take(100).AsQueryable();
        ////    //var search = (from m in _db.DevNews.AsEnumerable()
        ////    //             join e in excelclassListten on m.NewsId.ToString() equals e.id
        ////    //             select new 
        ////    //             {
        ////    //                 //url = e.url,
        ////    //                 id = e.id,
        ////    //                 pageview = e.pageview,
        ////    //                 sector = m.Sector
        ////    //             }).ToList();
        ////    return Json(excelclassList.GroupBy(s=>s.sector).Select(a=> new { sector = a.Key ,pageviews = a.Sum(b=>b.pageview)}), JsonRequestBehavior.AllowGet);
        ////}
        //public JsonResult sectors()
        //{
        //    var data = _db.DevSectors.Select(s => new { s.Id, s.Title });
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult getSectorImages(string id)
        //{
        //    var domainUrl = "https://www.devdiscourse.com";
        //    var blobdomain = "https://devdiscourse.blob.core.windows.net";
        //    var images = new List<string> {
        //            blobdomain + "/imagegallery/29_02_2020_19_19_41_2821142.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_17_44_8184075.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_17_04_3595712.jpg",
        //            blobdomain + "/imagegallery/29_02_2020_19_19_03_7965895.jpg",
        //            blobdomain + "/imagegallery/29_02_2020_19_18_39_4765366.jpg",
        //            blobdomain + "/imagegallery/29_02_2020_19_18_04_5779125.jpg",
        //            blobdomain + "/imagegallery/29_02_2020_19_16_04_3385426.jpg",
        //            blobdomain + "/imagegallery/29_02_2020_19_14_39_2122985.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_15_40_08_7202075.png",

        //            blobdomain + "/imagegallery/27_06_2019_18_23_57_7812851.png",
        //            blobdomain + "/imagegallery/03_03_2020_19_01_27_1763153.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_19_01_00_2807161.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_19_00_38_2827682.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_19_00_22_9749544.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_19_02_17_130752.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_18_59_53_6525806.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_19_01_57_1514112.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_18_59_53_6525806.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_18_59_34_5882013.jpg",
        //            blobdomain + "/imagegallery/03_03_2020_18_59_07_8553443.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_18_23_19_2663987.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_18_23_10_1033124.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_18_22_44_8556925.png",
        //            blobdomain + "/imagegallery/27_06_2019_18_22_28_337697.png",
        //            blobdomain + "/imagegallery/27_06_2019_18_22_18_3476595.jpg",

        //            blobdomain + "/imagegallery/27_06_2019_16_08_29_5606353.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_16_08_33_8108588.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_20_36_1176942.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_20_07_9483095.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_19_56_6666387.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_19_38_651933.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_19_01_7721992.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_16_07_33_2739065.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_16_07_26_6465234.png",
        //            blobdomain + "/imagegallery/27_06_2019_16_07_19_543745.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_16_07_12_1951489.png",
        //            blobdomain + "/imagegallery/27_06_2019_16_07_03_0698587.jpg",

        //            blobdomain + "/imagegallery/27_06_2019_18_34_04_2312796.png", blobdomain + "/imagegallery/27_06_2019_18_33_56_2660097.png", blobdomain + "/imagegallery/27_06_2019_18_33_51_6762345.png", blobdomain + "/imagegallery/27_06_2019_18_33_47_4854533.png", blobdomain + "/imagegallery/27_06_2019_18_33_35_2442501.png", blobdomain + "/imagegallery/29_02_2020_17_02_16_4468374.jpg", blobdomain + "/imagegallery/29_02_2020_17_01_41_8935244.jpg",blobdomain + "/imagegallery/29_02_2020_17_01_17_7363597.jpg", blobdomain + "/imagegallery/29_02_2020_17_00_46_5151186.jpg",blobdomain + "/imagegallery/29_02_2020_17_00_20_7172535.jpg",blobdomain + "/imagegallery/29_02_2020_16_59_34_3794932.jpg", blobdomain + "/imagegallery/29_02_2020_16_59_02_2201424.jpg",

        //            blobdomain + "/imagegallery/27_06_2019_18_18_25_2697677.jpg", blobdomain + "/imagegallery/27_06_2019_18_18_10_5703828.png", blobdomain + "/imagegallery/27_06_2019_18_01_02_8802375.jpg", blobdomain + "/imagegallery/27_06_2019_18_00_51_6514463.png", blobdomain + "/imagegallery/27_06_2019_18_00_09_3968771.jpg",

        //            blobdomain + "/devnews/08_02_2019_18_28_14_3352403.jpg", blobdomain + "/imagegallery/16_01_2019_13_00_03_4175236.jpg", blobdomain + "/imagegallery/16_01_2019_12_56_33_3001799.jpg",

        //            blobdomain + "/imagegallery/27_06_2019_15_04_53_4794057.jpg", blobdomain + "/imagegallery/27_06_2019_15_04_39_4075153.png", blobdomain + "/imagegallery/27_06_2019_15_04_07_0802794.jpg", blobdomain + "/imagegallery/27_06_2019_15_03_33_2835321.png", blobdomain + "/imagegallery/27_06_2019_14_59_16_3719142.jpg", blobdomain + "/imagegallery/27_06_2019_14_54_35_7405713.png", blobdomain + "/imagegallery/27_06_2019_14_53_21_8982776.jpg",

        //            blobdomain + "/imagegallery/27_05_2019_11_52_07_9391333.jpg",
        //        blobdomain + "/imagegallery/27_05_2019_11_51_19_0419949.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_08_19_05616.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_05_15_9474824.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_07_58_0139831.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_04_52_2445844.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_07_32_0301512.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_04_32_3060217.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_07_11_3625933.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_04_05_4766438.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_06_33_8443168.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_06_09_904154.jpg",
        //        blobdomain + "/imagegallery/03_03_2020_19_05_37_3344557.jpg",
        //        blobdomain + "/imagegallery/27_05_2019_11_49_14_1279651.jpg",
        //        blobdomain + "/imagegallery/01_04_2019_17_12_02_6631371.jpg",

        //        blobdomain + "/imagegallery/27_06_2019_18_51_01_3595413.png", blobdomain + "/imagegallery/27_06_2019_18_50_56_3254.png", blobdomain + "/imagegallery/27_06_2019_18_50_27_8479601.png",

        //        blobdomain + "/imagegallery/01_04_2019_19_26_59_8821975.jpg", blobdomain + "/imagegallery/01_04_2019_19_29_06_9517358.jpg", blobdomain + "/imagegallery/01_04_2019_19_28_56_3344151.jpg",

        //        blobdomain + "/imagegallery/27_06_2019_18_29_44_9544197.png",
        //            blobdomain + "/imagegallery/27_06_2019_18_29_38_8242443.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_25_51_351326.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_25_34_3607277.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_25_10_8590644.jpg",
        //            blobdomain + "/imagegallery/27_06_2019_18_29_32_8711902.png",
        //            blobdomain + "/imagegallery/27_06_2019_18_29_28_0264767.png",
        //            blobdomain + "/imagegallery/27_06_2019_18_29_23_945969.png",
        //            blobdomain + "/imagegallery/27_06_2019_18_29_17_3321446.png",

        //            blobdomain + "/imagegallery/27_06_2019_18_41_29_2135076.png",blobdomain+ "/imagegallery/12_11_2020_11_20_52_1499489.jpg", blobdomain + "/imagegallery/27_06_2019_18_46_53_9831308.png",blobdomain+ "/imagegallery/12_11_2020_11_29_14_7835671.jpg",

        //            "/AdminFiles/UserFiles/22_11_2018_14_00_28_6024474.jpg", blobdomain + "/imagegallery/16_01_2019_13_05_02_5451348.jpg", blobdomain + "/imagegallery/16_01_2019_13_01_16_4088895.jpg",

        //            blobdomain + "/imagegallery/29_02_2020_16_58_29_8149663.jpg",
        //                        blobdomain + "/imagegallery/11_03_2020_13_23_01_9613234.jpg",
        //                        blobdomain + "/imagegallery/11_03_2020_13_22_31_2034832.jpg",
        //                        blobdomain + "/imagegallery/29_02_2020_16_57_55_697089.jpg",
        //                        blobdomain + "/imagegallery/29_02_2020_16_57_14_7140931.jpg",
        //                        blobdomain + "/imagegallery/29_02_2020_16_56_48_7796591.jpg",
        //                        blobdomain + "/imagegallery/29_02_2020_16_55_29_8451536.jpg",
        //                        blobdomain + "/imagegallery/29_02_2020_16_53_54_7540817.jpg",
        //                        blobdomain + "/imagegallery/27_05_2019_11_50_40_2945094.jpg",
        //                        blobdomain + "/imagegallery/27_05_2019_12_11_15_3417518.jpg",
        //                        blobdomain + "/imagegallery/27_05_2019_11_50_05_8739542.jpg",
        //                        blobdomain + "/imagegallery/27_05_2019_12_10_51_4304562.jpg",
        //                        blobdomain + "/imagegallery/27_05_2019_11_49_54_2610105.jpg",
        //                        blobdomain + "/imagegallery/27_05_2019_12_10_31_1940386.jpg",

        //                        blobdomain + "/imagegallery/25_06_2019_19_11_46_1465506.jpg", blobdomain + "/imagegallery/25_06_2019_19_11_42_0025718.jpg", blobdomain + "/imagegallery/25_06_2019_19_11_37_8118616.jpg", blobdomain + "/imagegallery/25_06_2019_19_11_22_6945673.jpg", blobdomain + "/imagegallery/25_06_2019_19_11_50_2011104.jpg",

        //                        blobdomain + "/imagegallery/25_06_2019_19_11_55_1876187.jpg",
        //            blobdomain + "/imagegallery/25_06_2019_19_11_59_5183121.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_37_24_6979208.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_37_19_6120453.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_37_11_6036339.jpg",
        //            blobdomain + "/imagegallery/11_03_2020_13_36_53_1759644.jpg",
        //            blobdomain + "/imagegallery/25_06_2019_19_12_02_9568411.jpg",
        //            blobdomain + "/imagegallery/25_06_2019_19_12_06_3802569.jpg",
        //            blobdomain + "/imagegallery/25_06_2019_19_12_10_0069135.jpg",
        //            "/images/defaultImage.jpg"
        //        };
        //    var news = _db.DevNews.Where(s => s.Sector == id && !s.ImageUrl.Contains("AdminFiles") && !s.ImageUrl.Contains("/images/sector/") && !s.ImageUrl.Contains("UN_News_cover") && !s.ImageUrl.Contains("newstheme") && !images.Contains(s.ImageUrl)).OrderByDescending(s => s.CreatedOn).Select(a => new { a.Title, a.ImageUrl }).Take(1000);
        //    //news = news.Where(i => !images.Contains(i.ImageUrl));
        //    news = news.Select(o => new { o.Title, ImageUrl = o.ImageUrl.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? domainUrl + "/remote.axd?" + o.ImageUrl + "?width=224" : domainUrl + "" + o.ImageUrl + "?width=224" });
        //    return Json(news.ToList(), JsonRequestBehavior.AllowGet);
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                _db.Dispose();
                _db = null;
            }
            base.Dispose(disposing);
        }
    }
}