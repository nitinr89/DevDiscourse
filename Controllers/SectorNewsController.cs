using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers
{
    public class SectorNewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SectorNewsController(ApplicationDbContext db)
        {
            _db = db;
        }
        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration =60)]
        //public PartialViewResult GetAgroForestoryNews(string reg)
        //{
        //    ViewBag.Sector = "10";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
           
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "10" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetArtCultureNews(string reg)
        //{
        //    ViewBag.Sector = "14";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
         
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "14" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetTechnologyNews(string reg)
        //{
        //    ViewBag.Sector = "6";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "6" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetEconomyNews(string reg)
        //{
        //    ViewBag.Sector = "1";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
         
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "1" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetEducationNews(string reg)
        //{
        //    ViewBag.Sector = "4";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "4" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetEnergyNews(string reg)
        //{
        //    ViewBag.Sector = "7";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
           
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "7" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetPoliticsNews(string reg)
        //{
        //    ViewBag.Sector = "19";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "19" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        //////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetLawGovernanceNews(string reg)
        //{
        //    ViewBag.Sector = "11";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "11" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetHealthNews(string reg)
        //{
        //    ViewBag.Sector = "2";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "2" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetScienceNews(string reg)
        //{
        //    ViewBag.Sector = "15";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "15" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetSocialNews(string reg)
        //{
        //    ViewBag.Sector = "9";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "9" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetSportsNews(string reg)
        //{
        //    ViewBag.Sector = "18";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "18" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetTransportNews(string reg)
        //{
        //    ViewBag.Sector = "3";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
          
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "3" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetUrbanDevelopmentNews(string reg)
        //{
        //    ViewBag.Sector = "17";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
         
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "17" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
        //}

        ////[OutputCache(Duration = 60, VaryByParam = "*")]
        //[OutputCache(Duration = 60)]
        //public PartialViewResult GetWashNews(string reg)
        //{
        //    ViewBag.Sector = "12";
        //    DateTime twoMonth = DateTime.UtcNow.AddDays(-2);
           
        //    var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "12" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
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
        //    }).AsNoTracking().Take(30).ToList();
        //    return PartialView("_getSectorNews", result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
        //    //}
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
