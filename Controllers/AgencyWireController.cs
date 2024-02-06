using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Devdiscourse.Controllers
{
    public class AgencyWireController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AgencyWireController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            DateTime oneMonth = DateTime.Today.AddDays(-10);
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > oneMonth && a.Type == "News" && (a.Source == "PTI" || a.Source == "Reuters" || a.Source == "IANS")).Select(a => new PublisherView { ModifiedOn = a.CreatedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn);
            return View(search.ToPagedList((page ?? 1), 40));
        }
        public ActionResult ANIStories(int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            DateTime dayBefore = DateTime.Today.AddDays(-30);
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > dayBefore && a.Type == "News" && a.Source == "ANI").Select(a => new PublisherView { ModifiedOn = a.CreatedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).AsNoTracking();
            return View(search.ToPagedList((page ?? 1), 20));
        }
        [OutputCache(Duration = 60)]
        public ActionResult PRNewswire(int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            DateTime dayBefore = DateTime.Today.AddDays(-30);
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > dayBefore && a.Type == "News" && a.Source == "PR Newswire").Select(a => new PublisherView { ModifiedOn = a.CreatedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).AsNoTracking();
            return View(search.ToPagedList((page ?? 1), 20));
        }
        [OutputCache(Duration = 60)]
        public ActionResult NewsSource(string source, int? page)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            ViewBag.source = source;
            DateTime dayBefore = DateTime.Today.AddDays(-30);
            var search = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > dayBefore && a.Type == "News" && a.Source == source).Select(a => new PublisherView { ModifiedOn = a.CreatedOn, Title = a.Title, Id = a.NewsId, ImageUrl = a.ImageUrl, Country = a.Country, Label = a.NewsLabels }).OrderByDescending(a => a.ModifiedOn).AsNoTracking();
            return View(search.ToPagedList((page ?? 1), 20));
        }
        [OutputCache(Duration = 30)]
        public ActionResult NewsAnalysis(string type)
        {
            string? cookie = Request.Cookies["Edition"];
            switch (cookie)
            {
                case null:
                    ViewBag.region = "Global Edition";
                    break;
                default:
                    ViewBag.region = cookie ?? "Global Edition";
                    break;
            }
            ViewBag.type = type;
            return View();
        }
        //public PartialViewResult GetNewsAnalysisItems(string region, string type, int? page)
        //{
        //    int pageSize = 25;
        //    int pageNumber = (page ?? 1);
        //    ViewBag.page = pageNumber;
        //    DateTime oneMonth = pageNumber == 1 ? DateTime.Today.AddDays(-2) : DateTime.Today.AddDays(-15);
        //    if (type == "Other" || region == "Global Edition")
        //    {
        //        //var search =  _db.RegionNewsRankings.Where(a=> a.DevNews.AdminCheck == true && a.DevNews.CreatedOn> oneMonth).Select(a=> new SearchView
        //        //{
        //        //    Id = a.DevNews.Id,
        //        //    NewsId = a.DevNews.NewsId,
        //        //    Title = a.DevNews.Title,
        //        //    ImageUrl = a.DevNews.ImageUrl,
        //        //    Country = a.DevNews.Country,
        //        //    CreatedOn = a.DevNews.ModifiedOn,
        //        //    Region = a.DevNews.Region,
        //        //    Sector = a.DevNews.Sector,
        //        //    IsGlobal = a.DevNews.IsGlobal,
        //        //    IsVideo = a.DevNews.IsVideo,
        //        //    IsSponsored = a.DevNews.IsSponsored,
        //        //    EditorPick = a.DevNews.EditorPick,
        //        //    Tags = a.DevNews.Tags,
        //        //    Type = a.DevNews.Type,
        //        //    SubType = a.DevNews.SubType,
        //        //    Category = a.DevNews.Category,
        //        //    Label = a.DevNews.NewsLabels,
        //        //    Ranking =  a.Ranking
        //        //}).GroupBy(a=>a.Title).Select(s=>s.FirstOrDefault()).AsNoTracking().OrderByDescending(o => o.CreatedOn).ThenByDescending(s => s.Ranking).ToPagedList(pageNumber, pageSize);
        //        var search = (from a in _db.DevNews
        //                      //where a.AdminCheck && a.CreatedOn > oneMonth
        //                      select new NewsAnalysisViewModel
        //                      {
        //                          NewsId = a.NewsId,
        //                          Title = a.Title,
        //                          ImageUrl = a.ImageUrl,
        //                          Country = a.Country,
        //                          CreatedOn = a.ModifiedOn,
        //                          Type = a.Type,
        //                          SubType = a.SubType,
        //                          Label = a.NewsLabels
        //                      })
        //                      //.OrderByDescending(o => o.CreatedOn).AsNoTracking()
        //                      .ToPagedList(pageNumber, pageSize);
        //        return PartialView("_getNewsAnalysisItems", search);
        //    }
        //    else
        //    {
        //        //var search = (from a in _db.DevNews
        //        //              where a.AdminCheck == true && a.CreatedOn > oneMonth && a.Region.Contains(region)
        //        //               select new SearchView
        //        //              {
        //        //                  Id = a.Id,
        //        //                  NewsId = a.NewsId,
        //        //                  Title = a.Title,
        //        //                  ImageUrl = a.ImageUrl,
        //        //                  Country = a.Country,
        //        //                  CreatedOn = a.ModifiedOn,
        //        //                  Region = a.Region,
        //        //                  Sector = a.Sector,
        //        //                  IsGlobal = a.IsGlobal,
        //        //                  IsVideo = a.IsVideo,
        //        //                  IsSponsored = a.IsSponsored,
        //        //                  EditorPick = a.EditorPick,
        //        //                  Tags = a.Tags,
        //        //                  Type = a.Type,
        //        //                  SubType = a.SubType,
        //        //                  Category = a.Category,
        //        //                  Label = a.NewsLabels
        //        //              }).OrderByDescending(o => o.CreatedOn).AsNoTracking().ToPagedList(pageNumber, pageSize);
        //        NewsAnalysisvar search = _db.RegionNewsRankings
        //            //.Where(a => a.DevNews.AdminCheck == true && a.Region.Title == region && a.DevNews.CreatedOn > oneMonth)
        //            .Select(a => new NewsAnalysisViewModel
        //        {
        //            NewsId = a.DevNews.NewsId,
        //            Title = a.DevNews.Title,
        //            ImageUrl = a.DevNews.ImageUrl,
        //            Country = a.DevNews.Country,
        //            CreatedOn = a.DevNews.ModifiedOn,
        //            Type = a.DevNews.Type,
        //            SubType = a.DevNews.SubType,
        //            Label = a.DevNews.NewsLabels,
        //            Ranking = a.Ranking
        //        })
        //            //.AsNoTracking().OrderByDescending(o => o.CreatedOn)
        //            .ToPagedList(pageNumber, pageSize);
        //        return PartialView("_getNewsAnalysisItems"/*, search.OrderByDescending(o => o.CreatedOn.Date).ThenByDescending(s => s.Ranking).AsEnumerable()*/);
        //    }
        //}

        public PartialViewResult GetNews(int skip = 0, int take = 0)
        {
            ViewBag.skipCount = skip;
            DateTime fiveDay = DateTime.Today.AddDays(-5);
            var result = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > fiveDay && (a.Source == "PTI" || a.Source == "Reuters" || a.Source == "IANS") && a.NewsLabels == null).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Skip(skip).Take(take);
            return PartialView("_getNews", result.ToList());
        }
        public PartialViewResult GetLatestNews(string reg = "Global Edition")
        {
            DateTime threemonths = DateTime.Today.AddDays(-5);
            if (reg == "Global Edition")
            {
                var result = _db.DevNews.Where(a => (a.IsGlobal == true || a.Region.Contains(reg)) && a.CreatedOn > threemonths && a.AdminCheck == true && a.Sector != null && a.NewsLabels != null).OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Take(6);
                return PartialView("_getLatestNews", result.ToList());
            }
            else
            {
                var result = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > threemonths && a.Region.Contains(reg) && a.Sector != null && a.NewsLabels != null).OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Take(6);
                return PartialView("_getLatestNews", result.ToList());
            }
        }

        public PartialViewResult GetTagNews(long id, string tag, string sector)
        {

            var tagList = tag.Split(',').Reverse().Skip(3).Take(3).ToList();
            DateTime threemonths = DateTime.Today.AddDays(-15);
            if (!string.IsNullOrEmpty(sector))
            {
                var result = (from a in _db.DevNews
                              from s in tagList
                              where a.CreatedOn > threemonths && a.NewsId != id && a.AdminCheck == true
                              && (a.Title.Contains(s)) && a.Sector == sector
                              orderby a.ModifiedOn descending
                              select new LatestNewsView { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels }).Distinct().AsNoTracking().Take(5).ToList();
                return PartialView("_getTagNews", result);
            }
            else
            {
                var result = (from a in _db.DevNews
                              from s in tagList
                              where a.CreatedOn > threemonths && a.NewsId != id && a.AdminCheck == true
                              && (a.Title.Contains(s))
                              orderby a.ModifiedOn descending
                              select new LatestNewsView { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels }).Distinct().AsNoTracking().Take(5).ToList();
                return PartialView("_getTagNews", result);
            }


        }
        public PartialViewResult OtherEdition(long id, string reg)
        {
            DateTime onemonths = DateTime.Today.AddDays(-5);
            if (reg == "Global Edition")
            {
                var result = _db.DevNews.Where(a => (a.IsGlobal == false || !a.Region.Contains("Global Edition")) && a.CreatedOn > onemonths && a.AdminCheck == true && a.Sector != null && a.NewsLabels != null).OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Take(6);
                return PartialView("_getOtherEditionNews", result.AsNoTracking().ToList());
            }
            else
            {
                var result = _db.DevNews.Where(a => (a.IsGlobal == true || a.Region.Contains(reg)) && a.CreatedOn > onemonths && (!a.Region.Contains(reg)) && a.Sector != null && a.NewsLabels != null).OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Take(6);
                return PartialView("_getOtherEditionNews", result.AsNoTracking().ToList());
            }
        }
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
