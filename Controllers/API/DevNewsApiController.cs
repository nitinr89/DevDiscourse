using System.Net;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Devdiscourse.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.OutputCaching;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace DevDiscourse.Controllers.API
{
    public class DevNewsApiController : Controller, IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> context;
        public DevNewsApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> context)
        {
            this.db = db;
            this.userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            this.context = context;
        }

        //GET: api/InfocusApi
        [Route("api/InfocusApi/{Edition}")]
        public IQueryable<NewsView> GetInfocusNews(string Edition)
        {
            if (Edition == "Global Edition")
            {
                var search = (from m in db.Infocus
                              where m.SrNo < 3
                              join s in db.DevNews on m.NewsId equals s.NewsId
                              where s.AdminCheck == true
                              orderby m.SrNo
                              select new NewsView
                              {
                                  Id = s.NewsId,
                                  Title = s.Title,
                                  ImageUrl = s.ImageUrl,
                                  CreatedOn = s.ModifiedOn,
                                  Sector = s.Sector,
                                  Country = s.Country,
                              }).Take(6);
                return search;
            }
            else
            {
                var search = (from m in db.Infocus
                              where m.Edition == Edition
                              join s in db.DevNews on m.NewsId equals s.NewsId
                              where s.AdminCheck == true
                              orderby m.SrNo
                              select new NewsView
                              {
                                  Id = s.NewsId,
                                  Title = s.Title,
                                  ImageUrl = s.ImageUrl,
                                  CreatedOn = s.ModifiedOn,
                                  Sector = s.Sector,
                                  Country = s.Country,
                              }).Take(6);
                return search;
            }

        }

        // GET: api/DevNewsApi
        [Route("api/DevNews/GetDevNews/{Page}/{Edition}")]
        public List<NewsView> GetDevNews(int Page, string Edition)
        {
            int skip = 10 * (Page - 1);
            var regArr = Edition.Split(',').ToList();
            var infocusSkip = (Page % 6) - 1;
            var FirstEdition = regArr[0];
            DateTime LastSevenDays = DateTime.Today.AddDays(-7);
            var searchinfocus = new List<NewsView>();
            if (Page < 6)
            {
                searchinfocus = (from m in db.Infocus
                                 where regArr.Any(edi => m.Edition.Contains(edi)) && m.ItemType == "News"
                                 join s in db.DevNews on m.NewsId equals s.NewsId
                                 where s.AdminCheck == true
                                 orderby m.SrNo
                                 select new NewsView
                                 {
                                     Id = s.NewsId,
                                     Title = s.Title,
                                     ImageUrl = s.ImageUrl,
                                     CreatedOn = s.ModifiedOn,
                                     Sector = s.Sector,
                                     Country = s.Country,
                                     NewsType = s.Type,
                                     IsInfocus = true
                                 }).Skip(infocusSkip).Take(1).ToList();
            }
            if (regArr.IndexOf("Global Edition") != -1 && regArr.Count() == 1)
            {
                var search = (from a in db.DevNews
                              where a.AdminCheck == true && a.CreatedOn > LastSevenDays
                              orderby a.ModifiedOn descending
                              select new NewsView
                              {
                                  Id = a.NewsId,
                                  Title = a.Title,
                                  ImageUrl = a.ImageUrl,
                                  CreatedOn = a.ModifiedOn,
                                  Sector = a.Sector,
                                  Country = a.Country,
                                  NewsType = a.Type,
                                  IsInfocus = false,
                              }).Skip(skip).Take(10).ToList();
                searchinfocus.AddRange(search);
                return searchinfocus;
            }
            else if (regArr.IndexOf("Global Edition") == -1)
            {
                var search = (from a in db.DevNews
                              where a.AdminCheck == true && regArr.Any(edi => a.Region.Contains(edi)) && a.CreatedOn > LastSevenDays
                              orderby a.ModifiedOn descending
                              select new NewsView
                              {
                                  Id = a.NewsId,
                                  Title = a.Title,
                                  ImageUrl = a.ImageUrl,
                                  CreatedOn = a.ModifiedOn,
                                  Sector = a.Sector,
                                  Country = a.Country,
                                  NewsType = a.Type,
                                  IsInfocus = false,
                              }).Skip(skip).Take(10).ToList();
                searchinfocus.AddRange(search);
                return searchinfocus;
            }
            else
            {
                regArr = Edition.Split(',').ToList();
                regArr = regArr.Where(val => val != "Global Edition").ToList();
                var search = (from a in db.DevNews
                              where a.AdminCheck == true && a.CreatedOn > LastSevenDays
                              orderby a.ModifiedOn descending
                              select new NewsView
                              {
                                  Id = a.NewsId,
                                  Title = a.Title,
                                  ImageUrl = a.ImageUrl,
                                  CreatedOn = a.ModifiedOn,
                                  Sector = a.Sector,
                                  Country = a.Country,
                                  NewsType = a.Type,
                                  IsInfocus = false,
                              }).Skip(skip).Take(10).ToList();
                searchinfocus.AddRange(search);
                return searchinfocus;
            }

        }

        [Route("api/DevNews/GetSectorDevNews/{Edition}/{sector}/{page}")]
        public IActionResult GetSectorDevNews(string Edition, string sector, int? page)
        {
            DateTime twoMonth = DateTime.UtcNow.AddDays(-3);
            var pageCount = page ?? 1;
            var skipCount = (pageCount - 1) * 10;
            var result = db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == sector && a.Region.Title == Edition && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new
            {
                Id = a.DevNews.NewsId,
                Title = a.DevNews.Title,
                ImageUrl = a.DevNews.ImageUrl,
                Country = a.DevNews.Country,
                CreatedOn = a.DevNews.ModifiedOn,
                Sector = a.DevNews.Sector,
                NewsType = a.DevNews.Type,
                Ranking = a.Ranking
            }).OrderByDescending(m => m.Ranking).Skip(skipCount).Take(10).ToList();
            return Json(result);
        }

        [Route("api/DevNews/GetSectorArticle/{reg}/{sector}")]
        public IQueryable<NewsViewModel> GetSectorArticle(string reg, string sector)
        {
            DateTime twoMonth = DateTime.Today.AddDays(-30);
            var infocusData = db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
            if (reg == "Global Edition")
            {
                if (sector == "1")
                {
                    var result = db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.CreatedOn > twoMonth && a.AdminCheck == true && a.Source == "Devdiscourse News Desk" && (a.Sector.StartsWith("1,") || a.Sector.Contains(",1,") || a.Sector.EndsWith(",1") || a.Sector == "1") || (a.Sector.StartsWith("5,") || a.Sector.Contains(",5,") || a.Sector.EndsWith(",5") || a.Sector == "5") && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
                    {
                        Title = a.Title,
                        NewsId = a.NewsId,
                        ImageUrl = a.ImageUrl,
                        Subtitle = a.SubTitle,
                        Country = a.Country,
                        CreatedOn = a.ModifiedOn,
                        Sector = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
                    }).Take(10);
                    return result;
                }
                else
                {
                    var result = db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.CreatedOn > twoMonth && a.AdminCheck == true && (a.Sector.StartsWith(sector + ",") || a.Sector.Contains("," + sector + ",") || a.Sector.EndsWith("," + sector) || a.Sector == sector) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
                    {
                        Title = a.Title,
                        NewsId = a.NewsId,
                        ImageUrl = a.ImageUrl,
                        Subtitle = a.SubTitle,
                        Country = a.Country,
                        CreatedOn = a.ModifiedOn,
                        Sector = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
                    }).Take(10);
                    return result;
                }
            }
            else
            {
                if (sector == "1")
                {
                    var result = db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.CreatedOn > twoMonth && a.AdminCheck == true && a.Source == "Devdiscourse News Desk" && (a.Sector.StartsWith("1,") || a.Sector.Contains(",1,") || a.Sector.EndsWith(",1") || a.Sector == "1") || (a.Sector.StartsWith("5,") || a.Sector.Contains(",5,") || a.Sector.EndsWith(",5") || a.Sector == "5") && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
                    {
                        Title = a.Title,
                        NewsId = a.NewsId,
                        ImageUrl = a.ImageUrl,
                        Subtitle = a.SubTitle,
                        Country = a.Country,
                        CreatedOn = a.ModifiedOn,
                        Sector = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
                    }).Take(10);
                    return result;
                }
                else
                {
                    var result = db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.CreatedOn > twoMonth && a.AdminCheck == true && (a.Sector.StartsWith(sector + ",") || a.Sector.Contains("," + sector + ",") || a.Sector.EndsWith("," + sector) || a.Sector == sector) && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
                    {
                        Title = a.Title,
                        NewsId = a.NewsId,
                        ImageUrl = a.ImageUrl,
                        Subtitle = a.SubTitle,
                        Country = a.Country,
                        CreatedOn = a.ModifiedOn,
                        Sector = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
                    }).Take(10);
                    return result;
                }
            }
        }
        [Route("api/DevNews/GetOtherNews/{type}")]
        public IQueryable<NewsViewModel> GetOtherNews(string type)
        {
            DateTime threeDays = DateTime.Today.AddDays(-3);
            if (type == "Global")
            {
                var result = db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > threeDays).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
                {
                    Title = a.Title,
                    NewsId = a.NewsId,
                    ImageUrl = a.ImageUrl,
                    Subtitle = a.SubTitle,
                    Country = a.Country,
                    CreatedOn = a.ModifiedOn,
                    Sector = a.Type,
                    SubType = a.SubType,
                    Label = a.NewsLabels
                }).Take(16);
                return result;
            }
            else
            {
                var result = db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > threeDays && (a.Source == "PTI" || a.Source == "Reuters" || a.Source == "IANS")).OrderByDescending(a => a.CreatedOn).Select(a => new NewsViewModel
                {
                    Title = a.Title,
                    NewsId = a.NewsId,
                    ImageUrl = a.ImageUrl,
                    Subtitle = a.SubTitle,
                    Country = a.Country,
                    CreatedOn = a.ModifiedOn,
                    Sector = a.Type,
                    SubType = a.SubType,
                    Label = a.NewsLabels
                }).Take(15);
                return result;
            }
        }

        [OutputCache(Duration = 120)]
        [Route("api/DevNews/GetVideoNews/{reg}")]
        public IQueryable<LatestNewsView> GetVideoNews(string reg = "Global Edition")
        {
            DateTime OneMonth = DateTime.Today.AddDays(-180);
            if (reg != "Global Edition")
            {
                var result = db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(reg) && a.IsVideo == true).OrderByDescending(m => m.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels }).Take(5);
                return result;
            }
            else
            {
                var result = db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > OneMonth && a.IsVideo == true).OrderByDescending(m => m.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels }).Take(5);
                return result;
            }
        }

        [Route("api/DevNews/GetRelatedDevNews/{sector}/{region}/{newsId}")]
        public IQueryable<NewsView> GetRelatedDevNews(string sector, string region, long newsId)
        {
            var secArr = sector.Split(',').ToList();
            var regArr = region.Split(',').ToList();
            if (region.Contains("Global Edition"))
            {
                var search = from m in db.DevNews
                             where secArr.Contains(m.Sector) && m.NewsId != newsId && m.AdminCheck == true && m.Type == "News"
                             orderby m.CreatedOn descending
                             select new NewsView
                             {
                                 Id = m.NewsId,
                                 Title = m.Title,
                                 ImageUrl = m.ImageUrl,
                                 CreatedOn = m.ModifiedOn,
                                 Sector = m.Sector,
                                 Country = m.Country,
                                 NewsType = m.Type
                             };
                return search.OrderByDescending(a => a.CreatedOn).Take(3);
            }
            else
            {
                var search = from m in db.DevNews
                             where secArr.Contains(m.Sector) && regArr.Contains(m.Region) && m.NewsId != newsId && m.AdminCheck == true && m.Type == "News"
                             orderby m.CreatedOn descending
                             select new NewsView
                             {
                                 Id = m.NewsId,
                                 Title = m.Title,
                                 ImageUrl = m.ImageUrl,
                                 CreatedOn = m.ModifiedOn,
                                 Sector = m.Sector,
                                 Country = m.Country,
                                 NewsType = m.Type
                             };
                return search.OrderByDescending(a => a.CreatedOn).Take(3);
            }
        }

        [Route("api/DevNews/GetRecommendedNews/{page}/{id}/{sectors}")]
        public IQueryable<NewsView> GetRecommendedNews(int page, string id, string sectors)
        {
            int skip = 10 * (page - 1);
            var secArr = sectors.Split(',').ToList();
            var userinterest = db.UserInterests.Where(a => a.UserId == id).Select(a => a.Sector);
            DateTime LastSevenDays = DateTime.Today.AddDays(-7);
            secArr.AddRange(userinterest);
            var search = from m in db.DevNews
                         where m.AdminCheck == true && m.Sector != "0" && secArr.Contains(m.Sector)
                         orderby m.CreatedOn > LastSevenDays descending
                         select new NewsView
                         {
                             Id = m.NewsId,
                             Title = m.Title,
                             ImageUrl = m.ImageUrl,
                             CreatedOn = m.ModifiedOn,
                             Sector = m.Sector,
                             Country = m.Country,
                         };
            return search.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(10);
        }

        [Route("api/News/GetRecommendedNews/{page}/{id}/{sectors}")]
        public IActionResult RecommendedNews(int page, string id, string sectors)
        {
            int skip = 10 * (page - 1);
            var secArr = sectors.Split(',').ToList();
            var userinterest = db.UserInterests.Where(a => a.UserId == id).Select(a => a.Sector);
            secArr.AddRange(userinterest);
            var search = from m in db.DevNews
                         where m.AdminCheck == true && m.Sector != "0" && secArr.Contains(m.Sector)
                         orderby m.CreatedOn descending
                         select new
                         {
                             id = m.NewsId,
                             title = m.Title,
                             image = m.ImageUrl,
                             url = "",
                             total_view = m.ViewCount,
                             total_comment = 0,
                             date = m.CreatedOn,
                             type = m.Type,
                             featured = m.EditorPick,
                             sector = m.Sector,
                             country = m.Country,
                         };
            return Ok(new { status = "success", count = 10, pages = page, news = search.OrderByDescending(a => a.date).Skip(skip).Take(10) });
        }

        [Route("api/DevNews/GetOneNews/{id}")]
        public IQueryable<NewsView> GetOneNews(long id)
        {
            var search = from m in db.DevNews
                         where m.NewsId == id
                         select new NewsView
                         {
                             Id = m.NewsId,
                             Title = m.Title,
                             ImageUrl = m.ImageUrl,
                             CreatedOn = m.ModifiedOn,
                             Sector = m.Sector,
                             Country = m.Country
                         };
            return search;
        }

        [Route("api/DevNews/GetTrendNews/{Page}/{reg}")]
        public IQueryable<NewsView> GetTrendNews(int Page, string reg)
        {
            var regArr = reg.Split(',').ToList();
            int skip = 10 * (Page - 1);
            if (regArr.Count <= 1 && regArr[0] == "Global Edition")
            {
                var search = from m in db.DevNews
                             where m.AdminCheck == true && m.IsSponsored == false
                             orderby m.ViewCount descending
                             select new NewsView
                             {
                                 Id = m.NewsId,
                                 Title = m.Title,
                                 ImageUrl = m.ImageUrl,
                                 CreatedOn = m.ModifiedOn,
                                 Sector = m.Sector,
                                 Country = m.Country
                             };
                return search.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(10);
            }
            else
            {
                var search = from m in db.DevNews
                             where regArr.Contains(m.Region) && m.AdminCheck == true && m.IsSponsored == false
                             orderby m.ViewCount descending
                             select new NewsView
                             {
                                 Id = m.NewsId,
                                 Title = m.Title,
                                 ImageUrl = m.ImageUrl,
                                 CreatedOn = m.ModifiedOn,
                                 Sector = m.Sector,
                                 Country = m.Country
                             };
                return search.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(10);
            }
        }
        [Route("api/DevNews/GetRelatedDevBlog/{sector}/{blogId}")]
        public IQueryable<NewsView> GetRelatedDevBlog(string sector, long blogId)
        {
            var secArr = sector.Split(',').ToList();
            if (secArr.Count == 13)
            {
                var search = from m in db.DevNews
                             where secArr.Contains(m.Sector) && m.NewsId != blogId && m.AdminCheck == true && m.Type == "Blog"
                             orderby m.CreatedOn descending
                             select new NewsView
                             {
                                 Id = m.NewsId,
                                 Title = m.Title,
                                 ImageUrl = m.ImageUrl,
                                 CreatedOn = m.ModifiedOn,
                                 Sector = m.Sector,
                                 Country = m.Country
                             };
                return search.OrderByDescending(a => a.CreatedOn).Take(3);
            }
            else
            {
                var search = from m in db.DevNews
                             where secArr.Contains(m.Sector) && m.NewsId != blogId && m.AdminCheck == true && m.Type == "Blog"
                             orderby m.CreatedOn descending
                             select new NewsView
                             {
                                 Id = m.NewsId,
                                 Title = m.Title,
                                 ImageUrl = m.ImageUrl,
                                 CreatedOn = m.ModifiedOn,
                                 Sector = m.Sector,
                                 Country = m.Country
                             };
                return search.OrderByDescending(a => a.CreatedOn).Take(3);
            }
        }

        [Route("api/DevNews/GetSDGsNews/{Page}")]
        public IQueryable<NewsView> GetSDGsNews(int Page)
        {
            int skip = 10 * (Page - 1);
            var search = from m in db.DevNews
                         where m.AdminCheck == true && (m.Category.StartsWith("13,") || m.Category.EndsWith(",13") || m.Category.Contains(",13,") || m.Category.Equals("13"))
                         orderby m.ViewCount descending
                         select new NewsView
                         {
                             Id = m.NewsId,
                             Title = m.Title,
                             ImageUrl = m.ImageUrl,
                             CreatedOn = m.ModifiedOn,
                             Sector = m.Sector,
                             Country = m.Country
                         };
            return search.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(10);
        }
        [Route("api/EventsView/GetDevEvents/{Page}/{category}")]
        public IQueryable<EventsView> GetDevEvents(int Page, int category)
        {
            int skip = 10 * (Page - 1);
            DateTime todayDate = DateTime.UtcNow.AddDays(1);
            if (category == 1)
            {
                var search = db.Events.Where(a => a.AdminCheck == true && a.EndDate > todayDate).Select(a => new EventsView { Id = a.Id, Title = a.Title, Description = a.Description, ImageUrl = a.FileUrl, StartDate = a.StartDate, EndDate = a.EndDate, CreatedOn = a.ModifiedOn }).OrderBy(a => a.StartDate).Skip(skip).Take(10);
                return search;
            }
            else if (category == 2)
            {
                var search = db.Events.Where(a => a.AdminCheck == true && a.EndDate < todayDate).Select(a => new EventsView { Id = a.Id, Title = a.Title, Description = a.Description, ImageUrl = a.FileUrl, StartDate = a.StartDate, EndDate = a.EndDate, CreatedOn = a.ModifiedOn }).OrderBy(a => a.StartDate).Skip(skip).Take(10);
                return search;
            }
            else
            {
                var search = db.Events.Where(a => a.AdminCheck == true).Select(a => new EventsView { Id = a.Id, Title = a.Title, Description = a.Description, ImageUrl = a.FileUrl, StartDate = a.StartDate, EndDate = a.EndDate, CreatedOn = a.ModifiedOn }).OrderBy(a => a.StartDate).Skip(skip).Take(10);
                return search;
            }
        }
        [Route("api/ResearchView/GetDevResearch/{Page}")]
        public IQueryable<ResearchView> GetDevResearch(int Page)
        {
            int skip = 10 * (Page - 1);
            var search = db.DevResearches.Where(a => a.AdminCheck == true).Select(a => new ResearchView { Id = a.Id, Title = a.Title, Description = a.Description, ImageUrl = a.FileUrl, CreatedOn = a.ModifiedOn, Tags = a.Tags }).OrderByDescending(a => a.CreatedOn).Skip(skip).Take(10);
            return search;
        }
        [Route("api/DevSector/GetSector/")]
        public IActionResult GetSector()
        {
            var search = db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.Title).Select(s => new { s.Id, s.Slug, s.SrNo, s.Title });
            return Ok(search);
        }
        [Route("api/news/topics")]
        public IActionResult GetTopics()
        {
            var search = db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.Title).Select(s => new { id = s.Id, slug = s.Slug, name = s.Title, icon = "", color = "#ff6a00", featured = 1, priority = s.SrNo, });
            return Ok(new { topic = search });
        }
        [Route("api/Region/GetEdition/")]
        public IQueryable<string> GetEdition()
        {
            var search = db.Regions.Where(a => a.SrNo != 0).OrderBy(a => a.SrNo).Select(a => a.Title);
            return search;
        }
        [Route("api/TeamView/GetTeam/{Page}")]
        public IQueryable<TeamView> GetTeam(int Page)
        {
            int skip = 10 * (Page - 1);
            var search = db.Teams.Where(a => a.Active == true).Select(a => new TeamView { TeamId = a.Id, TeamMember = a.TeamMember, CreatedOn = a.CreatedOn, Profile = a.ProfilePic, Desgination = a.Designation, SrNo = a.SrNo }).OrderBy(a => a.SrNo).Skip(skip).Take(20);
            return search;
        }
        [Route("api/Region/GetRegion/{country}")]
        public string GetRegion(string country)
        {
            var region = db.Countries.Where(a => a.Title.ToUpper() == country.ToUpper()).FirstOrDefault();
            if (region != null)
            {
                return region.Regions.Title;
            }
            return "Global Edition";
        }
        [Route("api/UpdateOriginalNews")]
        [HttpPost]
        public string PTINews(UpdateNewsViewModel obj)
        {
            string description = obj.Description;

            var newsObj = db.DevNews.FirstOrDefault(a => a.NewsId == obj.Id);
            if (newsObj != null)
            {
                newsObj.Description = obj.Description;
                newsObj.ModifiedOn = DateTime.Now;
                db.Entry(newsObj).State = EntityState.Modified;
                db.SaveChanges();
            }
            return obj.Id.ToString();
        }
        [Route("api/GenImg")]
        [HttpPost]
        public async Task<string> GenImg(string title, string sector, string prompt)
        {
            try
            {
                string endpoint = "https://api.openai.com/v1/images/generations";
                var requestData = new
                {
                    model = "dall-e-2",
                    prompt = prompt,
                    n = 1,
                    size = "1024x1024"
                };

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "api-key");
                var jsonPayload = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                var response1 = await client.PostAsync(endpoint, content);
                response1.EnsureSuccessStatusCode();
                var responseBody = await response1.Content.ReadAsStringAsync();
                var jsonresponse = JsonObject.Parse(responseBody);
                string imageUrl = jsonresponse["data"][0]["url"].ToString();

                if (string.IsNullOrWhiteSpace(imageUrl)) return $"NotOk200 - imageUrl is null or empty.";
                else
                {
                    HttpClient httpClient = new();
                    HttpResponseMessage response = await httpClient.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            var fileName = RandomName();
                            var fileExtension = ".png";
                            string mimeType;
                            string fileSize;
                            try
                            {

                                mimeType = response.Content.Headers.ContentType.MediaType;
                                fileSize = response.Content.Headers.ContentLength.ToString();
                            }
                            catch (Exception ex)
                            {
                                mimeType = "";
                                fileSize = "";
                            }

                            CloudBlobContainer blobContainer;
                            CloudBlockBlob blob;
                            using (MemoryStream ms = new())
                            {
                                await contentStream.CopyToAsync(ms);
                                ms.Position = 0;

                                blobContainer = await GetCloudBlobimagegalleryContainer();
                                blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                                await blob.UploadFromStreamAsync(ms);
                                string finalImageUrl = blob.Uri.ToString();

                                ImageGallery fileobj = new()
                                {
                                    Title = title,
                                    ImageUrl = finalImageUrl,
                                    ImageCopyright = "",
                                    Caption = title,
                                    FileMimeType = mimeType,
                                    FileSize = fileSize,
                                    Sector = sector,
                                    Tags = "",
                                    UseCount = 1,
                                };
                                db.ImageGalleries.Add(fileobj);
                                db.SaveChanges();

                                return finalImageUrl;
                            }
                        }
                    }
                    else return $"NotOk200 - can't download image from imageUrl.";
                }
            }
            catch (Exception ex)
            {
                return $"NotOk200 {ex.Message}";
            }
        }
        [Route("api/PTINews")]
        [HttpPost]
        public async Task<string> PTINews(SourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            var AutoAssign = GetAutoAssignStatus();
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            string sector = obj.Sector;
            if (obj.Category.ToUpper() == "NATIONAL")
            {
                region = "South Asia";
                country = "India";
            }
            else if (obj.Edition == null || obj.Edition == "")
            {
                region = "Global Edition";
            }
            else
            {
                region = obj.Edition;
                country = obj.Country;
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = description,
                Sector = sector,
                Region = region,
                Country = country,
                NewsLabels = obj.NewsLabel,
                Source = "PTI",
                OriginalSource = "PTI",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "Press Trust of India",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            var edition_ml = ML_Edition(description);
            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (edition_ml.Any())
            {
                foreach (var item in edition_ml)
                {
                    var regObj = new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    };
                    newsRankingList.Add(regObj);
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.Edition);
            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }
        public List<NewsRankingViewModel> ML_Edition(string content)
        {
            List<NewsRankingViewModel> newsRankingList = new List<NewsRankingViewModel>();
            try
            {
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    reqparm.Add("content", content);
                    byte[] responsebytes = client.UploadValues("https://devdiscourseml.azurewebsites.net/prediction", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    newsRankingList = JsonConvert.DeserializeObject<List<NewsRankingViewModel>>(responsebody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return newsRankingList;
        }
        [Route("api/PTIAutoNews")]
        [HttpPost]
        public async Task<string> PTIAutoNews(MLSourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            var AutoAssign = GetAutoAssignStatus();
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            string sector = obj.Sector;
            if (obj.Category.ToUpper() == "NATIONAL")
            {
                region = "South Asia";
                country = "India";
            }
            else if (obj.Edition == null || obj.Edition == "")
            {
                region = "Global Edition";
            }
            else
            {
                region = obj.Edition;
                country = obj.Country;
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = description,
                Sector = sector,
                Region = region,
                Country = country,
                NewsLabels = obj.NewsLabel,
                Source = "PTI",
                OriginalSource = "PTI",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "Press Trust of India",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (obj.NewsRanking != null)
            {
                foreach (var item in obj.NewsRanking)
                {
                    var regObj = new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    };
                    newsRankingList.Add(regObj);
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.Edition);
            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }

        [Route("api/PRNews")]
        [HttpPost]
        public async Task<string> PRNews(SourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            var AutoAssign = GetAutoAssignStatus();
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            string sector = obj.Sector;
            if (obj.Category.ToUpper() == "NATIONAL")
            {
                region = "South Asia";
                country = "India";
            }
            else if (obj.Edition == null || obj.Edition == "")
            {
                region = "Global Edition";
            }
            else
            {
                region = obj.Edition;
                country = obj.Country;
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = "",
                Description = description,
                Sector = sector,
                Region = region,
                Country = country,
                NewsLabels = obj.NewsLabel,
                Source = "PR Newswire",
                OriginalSource = "PR Newswire",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            //var newsId = newsObj.NewsId;
            //var userId = GetShiftUser(obj.Edition);
            //if (userId != "No User" && AutoAssign == true)
            //{
            //    AssignedNews(userId, newsId);
            //}
            return newsObj.NewsId.ToString();
        }
        [Route("api/ANINews")]
        [HttpPost]
        public async Task<string> ANINews(SourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            var AutoAssign = GetAutoAssignStatus();
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            string sector = obj.Sector;
            if (obj.Category.ToUpper() == "NATIONAL")
            {
                region = "South Asia";
                country = "India";
            }
            else if (obj.Edition == null || obj.Edition == "")
            {
                region = "Global Edition";
            }
            else
            {
                region = obj.Edition;
                country = obj.Country;
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = description,
                Sector = sector,
                Region = region,
                Country = country,
                NewsLabels = obj.NewsLabel,
                Source = "ANI",
                OriginalSource = "ANI",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                ImageCaption = obj.ImageCaption,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "ANI",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            var edition_ml = ML_Edition(description);
            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (edition_ml.Any())
            {
                foreach (var item in edition_ml)
                {
                    var regObj = new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    };
                    newsRankingList.Add(regObj);
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }

            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.Edition);
            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }
        [Route("api/ANIAutoNews")]
        [HttpPost]
        public async Task<string> ANIAutoNews(MLSourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string country = "";
            var AutoAssign = GetAutoAssignStatus();
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            string sector = obj.Sector;
            if (obj.Category.ToUpper() == "NATIONAL")
            {
                region = "South Asia";
                country = "India";
            }
            else if (obj.Edition == null || obj.Edition == "")
            {
                region = "Global Edition";
            }
            else
            {
                region = obj.Edition;
                country = obj.Country;
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = description,
                Sector = sector,
                Region = region,
                Country = country,
                NewsLabels = obj.NewsLabel,
                Source = "ANI",
                OriginalSource = "ANI",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                ImageCaption = obj.ImageCaption,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "ANI",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();
            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (obj.NewsRanking != null)
            {
                foreach (var item in obj.NewsRanking)
                {
                    newsRankingList.Add(new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    });
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.Edition);
            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }
        [Route("api/UNNews")]
        [HttpPost]
        public string UNNews(SourceNewsView obj)
        {
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);


            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = obj.Description,
                Sector = obj.Sector,
                Region = obj.Edition,
                Country = obj.Country,
                NewsLabels = obj.NewsLabel,
                Source = "UN News",
                OriginalSource = "UN News",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                ImageCaption = obj.ImageCaption,
                FileMimeType = "image/jpg",
                FileSize = "2,41,664",
                Category = obj.Category,
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = false,
                Author = "UN News",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            return newsObj.NewsId.ToString();
        }

        [Route("api/UNAutoNews")]
        [HttpPost]
        public string UNAutoNews(MLSourceNewsView obj)
        {
            string isadminCheck = obj.AdminCheck;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            bool admCheck = Boolean.Parse(isadminCheck);


            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = obj.Description,
                Sector = obj.Sector,
                Region = obj.Edition,
                Country = obj.Country,
                NewsLabels = obj.NewsLabel,
                Source = "UN News",
                OriginalSource = "UN News",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                ImageCopyright = obj.ImageCopyright,
                ImageCaption = obj.ImageCaption,
                FileMimeType = "image/jpg",
                FileSize = "2,41,664",
                Category = obj.Category,
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = false,
                Author = "UN News",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (obj.NewsRanking != null)
            {
                foreach (var item in obj.NewsRanking)
                {
                    var regObj = new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    };
                    newsRankingList.Add(regObj);
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }

            return newsObj.NewsId.ToString();
        }

        [Route("api/UpdateNews/{NewsId}/{Keywords}")]
        public string UpdateNews(long NewsId, string Keywords)
        {
            var news = db.DevNews.FirstOrDefault(a => a.NewsId == NewsId);
            news.Tags = Keywords;
            news.AdminCheck = true;
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            return "Success";
        }
        [Route("api/UpdateKeywords")]
        [HttpPost]
        public string UpdateKeywords(UpdateObj obj)
        {
            long NewsId = obj.NewsId;
            string Keywords = obj.Keywords;
            var news = db.DevNews.FirstOrDefault(a => a.NewsId == NewsId);
            var keyArr = news.Tags.Split(',').Reverse().Take(3);
            string twoKeywords = string.Join(",", keyArr.Reverse());
            string toatalKeywordsString = Keywords + "," + twoKeywords;
            var finalKeywordArray = toatalKeywordsString.Split(',').Distinct();
            news.Tags = string.Join(",", finalKeywordArray);
            //news.AdminCheck = false;
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            return "Success";
        }
        [Route("api/AdminCheck/{NewsId}")]
        public string AdminCheck(long NewsId)
        {
            var news = db.DevNews.FirstOrDefault(a => a.NewsId == NewsId);
            news.AdminCheck = true;
            db.Entry(news).State = EntityState.Modified;
            db.SaveChanges();
            return "Success";
        }
        [Route("api/ReutersNews")]
        [HttpPost]
        public async Task<string> ReutersNews(SourceNewsView obj)
        {
            string description = obj.Description;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            string isadminCheck = obj.AdminCheck;
            bool admCheck = Boolean.Parse(isadminCheck);
            var AutoAssign = GetAutoAssignStatus();
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = description,
                Sector = obj.Sector,
                Region = obj.Edition,
                NewsLabels = obj.NewsLabel,
                Country = obj.Country,
                Source = "Reuters",
                OriginalSource = "Reuters",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "Reuters",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            var edition_ml = ML_Edition(description);
            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (edition_ml.Any())
            {
                foreach (var item in edition_ml)
                {
                    var regObj = new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    };
                    newsRankingList.Add(regObj);
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }

            PostMessageToTwitter(newsObj.Title + " " + "https://www.devdiscourse.com/article/" + (newsObj.NewsLabels ?? "agency-wire") + "/" + newsObj.GenerateSecondSlug().ToString());
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.NewsLabel);
            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }

        [Route("api/ReutersAutoNews")]
        [HttpPost]
        public async Task<string> ReutersAutoNews(MLSourceNewsView obj)
        {
            string description = obj.Description;
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            string isadminCheck = obj.AdminCheck;
            bool admCheck = Boolean.Parse(isadminCheck);
            var AutoAssign = GetAutoAssignStatus();
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            if (isRepeatNews == true)
            {
                AutoAssign = false;
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = obj.SubTitle,
                Description = description,
                Sector = obj.Sector,
                Region = obj.Edition,
                NewsLabels = obj.NewsLabel,
                Country = obj.Country,
                Source = "Reuters",
                OriginalSource = "Reuters",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl,
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                Author = "Reuters",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();

            List<RegionNewsRanking> newsRankingList = new List<RegionNewsRanking>();
            if (obj.NewsRanking != null)
            {
                foreach (var item in obj.NewsRanking)
                {
                    newsRankingList.Add(new RegionNewsRanking()
                    {
                        RegionId = item.RegionId,
                        NewsId = newsObj.Id,
                        Ranking = item.Ranking
                    });
                };
                db.RegionNewsRankings.AddRange(newsRankingList);
                db.SaveChanges();
            }

            PostMessageToTwitter(newsObj.Title + " " + "https://www.devdiscourse.com/article/" + (newsObj.NewsLabels ?? "agency-wire") + "/" + newsObj.GenerateSecondSlug().ToString());
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.NewsLabel);
            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }
        public IActionResult PostMessageToTwitter(string message)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                     | SecurityProtocolType.Tls11
                                     | SecurityProtocolType.Tls12;
            //The facebook json url to update the status
            string twitterURL = "https://api.twitter.com/1.1/statuses/update.json";

            //set the access tokens (REQUIRED)
            string oauth_consumer_key = "VBDXixgcoAnlrqF7zDBBKprQJ";
            string oauth_consumer_secret = "rACbZ7Kp61J3qH4phMlcJ9dcX85ITrefDN4BKvEZdEI8Thbuh5";
            string oauth_token = "937962626222497793-NedCqOxMpHFzod8kNkSbMPw5sdM5Qkh";
            string oauth_token_secret = "KKuZM8lFWk2XkI3zbRJplQU5fwYRvvUmVvr0xTpCvQ3fn";

            // set the oauth version and signature method
            string oauth_version = "1.0";
            string oauth_signature_method = "HMAC-SHA1";

            // create unique request details
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            System.TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

            string baseString = string.Format(
                baseFormat,
                oauth_consumer_key,
                oauth_nonce,
                oauth_signature_method,
                oauth_timestamp, oauth_token,
                oauth_version,
                Uri.EscapeDataString(message)
            );

            string oauth_signature = null;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(Uri.EscapeDataString(oauth_consumer_secret) + "&" + Uri.EscapeDataString(oauth_token_secret))))
            {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes("POST&" + Uri.EscapeDataString(twitterURL) + "&" + Uri.EscapeDataString(baseString))));
            }

            // create the request header
            string authorizationFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + "oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " + "oauth_version=\"{6}\"";

            string authorizationHeader = string.Format(
                authorizationFormat,
                Uri.EscapeDataString(oauth_consumer_key),
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(oauth_token),
                Uri.EscapeDataString(oauth_version)
            );

            HttpWebRequest objHttpWebRequest = (HttpWebRequest)WebRequest.Create(twitterURL);
            objHttpWebRequest.Headers.Add("Authorization", authorizationHeader);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream objStream = objHttpWebRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes("status=" + Uri.EscapeDataString(message));
                objStream.Write(content, 0, content.Length);
            }

            var responseResult = "";
            try
            {
                //success posting
                WebResponse objWebResponse = objHttpWebRequest.GetResponse();
                StreamReader objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                responseResult = objStreamReader.ReadToEnd().ToString();
            }
            catch (Exception ex)
            {
                //throw exception error
                responseResult = "Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authorizationHeader;
            }
            return Ok(responseResult);
        }
        [Route("api/IANSNews")]
        [HttpPost]
        public async Task<string> IANSNews(SourceNewsView obj)
        {
            string description = obj.Description;
            string region = "";
            string sector = obj.Sector;
            bool AutoAssign = GetAutoAssignStatus();
            string isRepeat = obj.IsRepeat;
            bool isRepeatNews = Boolean.Parse(isRepeat);
            string isadminCheck = obj.AdminCheck;
            bool admCheck = Boolean.Parse(isadminCheck);
            if (obj.Category.ToUpper() == "INTERNATIONAL")
            {
                region = "Global Edition";
            }
            else
            {
                region = "South Asia";
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = "",
                Description = description,
                Sector = sector,
                Region = region,
                Country = "",
                NewsLabels = obj.NewsLabel,
                Source = "IANS",
                OriginalSource = "IANS",
                Tags = obj.Tags,
                AdminCheck = admCheck,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl, //"/images/newstheme.jpg",
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = !isRepeatNews,
                Author = "IANS",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                WorkStage = "",
                SourceUrl = obj.Origin,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.NewsLabel);

            if (userId != "No User" && isRepeatNews != true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }
        [Route("api/AFPNews")]
        [HttpPost]
        public async Task<string> AFPNews(SourceNewsView obj)
        {
            string description = obj.Description + " (This story has not been edited by Devdiscourse staff and is auto-generated from a syndicated feed.)";
            string region = "";
            string country = "";
            bool AutoAssign = GetAutoAssignStatus();
            if (obj.Category.ToUpper() == "INTERNATIONAL")
            {
                region = "Global Edition";
            }
            else
            {
                region = "South Asia";
                country = "India";
            }
            if (String.IsNullOrEmpty(obj.ImageUrl))
            {
                obj.ImageUrl = "/images/newstheme.jpg";
            }
            DevNews newsObj = new DevNews
            {
                Title = obj.Title,
                SubTitle = "",
                Description = description,
                Sector = "0",
                Region = region,
                Country = country,
                NewsLabels = obj.NewsLabel,
                Source = "AFP",
                OriginalSource = "AFP",
                Tags = obj.Tags,
                AdminCheck = true,
                IsGlobal = false,
                ImageUrl = obj.ImageUrl, //"/images/newstheme.jpg",
                FileMimeType = "image/jpg",
                FileSize = "88,651",
                IsSponsored = false,
                EditorPick = false,
                IsInfocus = false,
                IsVideo = false,
                IsStandout = false,
                IsIndexed = AutoAssign,
                WorkStage = "",
                Author = "Press Trust of India",
                Type = "News",
                ViewCount = 0,
                LikeCount = 0,
                Creator = "3df123f7-0a8f-43c1-967d-bc26c4463b56",
            };
            db.DevNews.Add(newsObj);
            db.SaveChanges();
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            await context.Clients.All.SendAsync("SendNewsNotification", "New News Added on Admin Panel");
            // Assign To User
            var newsId = newsObj.NewsId;
            var userId = GetShiftUser(obj.NewsLabel);

            if (userId != "No User" && AutoAssign == true)
            {
                AssignedNews(userId, newsId);
            }
            return newsObj.NewsId.ToString();
        }
        public bool GetAutoAssignStatus()
        {
            bool status = false;
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwrootPath, "Content", "status.txt");
            if (System.IO.File.Exists(filePath))
            {
                string statusText = System.IO.File.ReadAllText(filePath);
                status = bool.Parse(statusText);
            }
            return status;
        }
        public string GetShiftUser(string label)
        {
            var labelUser = db.UserNewsLabels.Where(o => o.NewsLabel == label && o.ApplicationUsers.isActive == true).OrderBy(s => s.ApplicationUsers.CreatedOn).Select(s => s.ApplicationUsers.Id).ToArray();
            if (labelUser.Length > 0)
            {
                var fileName = Path.Combine(_webHostEnvironment.WebRootPath, "Content", "newslabel", "label.txt");
                int counter = 0;
                if (!System.IO.File.Exists(fileName))
                {
                    using (StreamWriter sw = System.IO.File.CreateText(fileName))
                    {
                        sw.WriteLine(counter.ToString());
                    }
                }
                else
                {
                    string noOfVisitors = System.IO.File.ReadAllText(fileName);
                    counter = Int32.Parse(noOfVisitors);
                }
                ++counter;
                var userlength = labelUser.Length;
                var userIndex = counter % userlength;
                counter = userIndex;
                System.IO.File.WriteAllText(fileName, counter.ToString());
                return labelUser[userIndex].ToString();
            }
            else
            {
                var User = db.Users.Where(a => a.isActive == true).OrderByDescending(o => o.CreatedOn).Select(s => s.Id).ToArray();
                int counter = 0;
                if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "Content", "counter.txt")))
                {
                    string noOfVisitors = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "Content", "counter.txt"));
                    counter = Int32.Parse(noOfVisitors);
                }
                ++counter;
                System.IO.File.WriteAllText(Path.Combine(_webHostEnvironment.WebRootPath, "Content", "counter.txt"), counter.ToString());
                if (User.Length > 0)
                {
                    var userlength = User.Length;
                    var userIndex = counter % userlength;
                    counter = userIndex;
                    System.IO.File.WriteAllText(Path.Combine(_webHostEnvironment.WebRootPath, "Content", "counter.txt"), counter.ToString());
                    return User[userIndex].ToString();
                }
                return "No User";
            }

        }
        public void AssignedNews(string userId, long NewsId)
        {
            AssignNews obj = new AssignNews()
            {
                UserId = userId,
                NewsId = NewsId,
                CreatedOn = DateTime.UtcNow,
                Creator = userManager.GetUserId(User),
            };
            db.AssignNews.Add(obj);
            db.SaveChanges();
        }
        // GET: api/DevNewsApi/5
        //[ResponseType(typeof(DevNews))]
        public IActionResult GetDevNews(Guid id)
        {
            DevNews? devNews = db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }

            return Ok(devNews);
        }

        // PUT: api/DevNewsApi/5
        //[ResponseType(typeof(void))]
        public IActionResult PutDevNews(Guid id, DevNews devNews)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != devNews.Id)
            {
                return BadRequest();
            }

            db.Entry(devNews).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DevNewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DevNewsApi
        //[ResponseType(typeof(DevNews))]
        public IActionResult PostDevNews(DevNews devNews)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DevNews.Add(devNews);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = devNews.Id }, devNews);
        }
        // DELETE: api/DevNewsApi/5
        //[ResponseType(typeof(DevNews))]
        public IActionResult DeleteDevNews(Guid id)
        {
            DevNews? devNews = db.DevNews.Find(id);
            if (devNews == null)
            {
                return NotFound();
            }

            db.DevNews.Remove(devNews);
            db.SaveChanges();

            return Ok(devNews);
        }
        [Route("api/GetPreviousNews/{id}/{label}/{reg}/{skip}")]
        [HttpGet]
        public IActionResult GetPreviousNews(long id, string label, string reg = "Global Edition", int skip = 0)
        {
            var search = db.DevNews.FirstOrDefault(a => a.NewsId == id);
            DateTime tenDays = search.CreatedOn.AddHours(-8);
            List<DevNews> newsList = new List<DevNews>();
            List<ApiNewsView> resultNewsList = new List<ApiNewsView>();
            if (reg == "Global Edition")
            {
                if (label != "agency-wire")
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.NewsLabels == label && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(9).ToList();
                }
                else
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.NewsLabels == null && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(9).ToList();
                }
            }
            else
            {
                if (label != "agency-wire")
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == label && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(9).ToList();
                }
                else
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == null && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(9).ToList();
                }
            }
            foreach (var news in newsList)
            {
                var tags = news.Tags ?? "";
                var tagArray = tags.Split(',').ToList();
                var DataDesc = news.Description;
                var regex = new Regex(string.Join("|", tagArray.Where(l => l.Length > 3).OrderBy(a => a.Length).Select(a => a.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Trim())));
                if (tagArray.Any())
                {
                    DataDesc = regex.Replace(DataDesc, match => " <a href='/news?tag=" + match.Value.Trim() + "'>" + match.Value.Trim() + "</a> ");
                }
                resultNewsList.Add(new ApiNewsView()
                {
                    Title = news.Title,
                    Description = DataDesc,
                    Subtitle = news.SubTitle,
                    ImageUrl = news.ImageUrl,
                    Country = news.Country,
                    Type = news.Type,
                    SubType = news.SubType,
                    Source = news.Source,
                    SourceUrl = news.SourceUrl,
                    Tags = news.Tags,
                    Label = news.NewsLabels,
                    Slug = news.GenerateSecondSlug(),
                    ModifiedOn = news.ModifiedOn,
                    CreatedOn = news.CreatedOn,
                    PublishedOn = news.PublishedOn,
                    NewsId = news.NewsId,
                    Id = news.Id,
                    ImageCopyright = news.ImageCopyright
                });
            }
            return Ok(resultNewsList);
        }

        [Route("api/DevNewsApi/GetPreviousSectorNews/{id}/{sector}/{reg}/{skip}")]
        [HttpGet]
        public IActionResult GetPreviousSectorNews(long id, string sector, string reg = "Global Edition", int skip = 0)
        {

            var regs = (from c in db.Countries
                        join r in db.Regions on c.RegionId equals r.Id
                        where c.Title == reg
                        select new
                        {
                            r.Title
                        }).FirstOrDefault();
            string regionTitle = "Global Edition";

            var region = regs != null && regs.Title != null ? regionTitle = regs.Title : regionTitle = reg;

            var search = db.DevNews.FirstOrDefault(a => a.NewsId == id);
            DateTime tenDays = search.CreatedOn.AddHours(-8);

            var newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && (a.Sector == sector) && a.AdminCheck == true);
            //var sectorIds = sector.Split(',').Select(int.Parse).ToList();
            //var newsList = db.DevNews.Where(a => a.SectorMapping.Any(ns => sectorIds.Contains(ns.SectorId)));

            if (region != "Global Edition")
            {
                newsList = newsList.Where(a => a.Region.Contains(region));
            }
            newsList = newsList.OrderByDescending(o => o.CreatedOn).Skip(skip).Take(9);
            List<ApiNewsView> resultNewsList = new List<ApiNewsView>();
            foreach (var news in newsList.ToList())
            {
                var tags = news.Tags ?? "";
                var tagArray = tags.Split(',').Where(l => !string.IsNullOrEmpty(l)).ToList();
                var DataDesc = news.Description;
                var regex = new Regex(string.Join("|", tagArray.Select(a => a.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Trim()).Where(l => l.Length > 3).OrderBy(a => a.Length)));
                if (tagArray.Any())
                {
                    DataDesc = regex.Replace(DataDesc, match => " <a href='/news?tag=" + match.Value.Trim() + "'>" + match.Value.Trim() + "</a> ");
                }
                resultNewsList.Add(new ApiNewsView()
                {
                    Title = news.Title,
                    Description = DataDesc,
                    Subtitle = (news.SubTitle != null) ? news.SubTitle : "Default",
                    ImageUrl = news.ImageUrl,
                    Country = (news.Country != null) ? news.Country : "Default",
                    Type = (news.Type != null) ? news.Type : "Default",
                    SubType = (news.SubType != null) ? news.SubType : "Default",
                    Source = (news.Source != null) ? news.Source : "Default",
                    Themes = (news.Themes != null) ? news.Themes : "Default",
                    Avatar = news.ApplicationUsers?.ProfilePic ?? "",
                    SourceUrl = (news.SourceUrl != null) ? news.SourceUrl : "Default",
                    Tags = (news.Tags != null) ? news.Tags : "Default",
                    Label = (news.NewsLabels != null) ? news.NewsLabels : "Default",
                    Slug = news.GenerateSecondSlug(),
                    ModifiedOn = news.ModifiedOn,
                    CreatedOn = news.CreatedOn,
                    PublishedOn = news.PublishedOn,
                    NewsId = news.NewsId,
                    Id = news.Id,
                    ImageCopyright = (news.ImageCopyright != null) ? news.ImageCopyright : "Default"
                });
            }
            return Ok(resultNewsList);
        }

        [Route("api/GetSearchedNews/{text}")]
        [HttpGet]
        public IActionResult GetSearchedNews(string text)
        {
            DateTime daydiff = DateTime.Today.AddDays(-15);
            var search = db.DevNews.Where(a => a.Title.Contains(text) && a.CreatedOn > daydiff && a.AdminCheck == true).Select(a => new { Label = a.NewsLabels, a.NewsId, a.Title, a.CreatedOn }).OrderByDescending(a => a.CreatedOn).Distinct().Take(10).ToList();
            return Ok(search);
        }
        [Route("api/GetuserbyRole/{role}")]
        [HttpGet]
        public async Task<IActionResult> GetuserbyRole(string role)
        {
            var Role = db.Roles.FirstOrDefault(m => m.Name == role);
            if (Role != null)
            {
                var users = await userManager.GetUsersInRoleAsync(role);
                var result = users.Select(a => new ShiftUser { FirstName = a.FirstName, LastName = a.LastName, UserId = a.Id, isActice = a.isActive }).ToList();
                return Ok(result);
            }
            return Ok("NoUser");
        }
        [Route("api/DevNewsApi/GetTagNews/{id}/{tag}")]
        [HttpGet]
        public IActionResult GetTagNews(long id, string tag)
        {

            var tagList = tag.Split(',').ToList();
            DateTime threemonths = DateTime.Today.AddDays(-10);
            var result = (from a in db.DevNews
                          from s in tagList
                          where a.CreatedOn > threemonths && a.NewsId != id && a.AdminCheck == true
                          && (a.Title.Contains(s))
                          orderby a.ModifiedOn descending
                          select new { Title = a.Title, NewId = a.NewsId, Label = a.NewsLabels }).Distinct().Take(5).ToList();
            return Ok(result);
        }
        private bool DevNewsExists(Guid id)
        {
            return db.DevNews.Count(e => e.Id == id) > 0;
        }
        [Route("api/News/UploadFile")]
        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var httpRequest = HttpContext.Request;
            string Title = httpRequest.Form["Title"];
            string Caption = httpRequest.Form["Caption"];
            string ImageCopyright = httpRequest.Form["ImageCopyright"];
            if (httpRequest.Form.Files.Count > 0)
            {
                string returnFileUrl = "";
                for (var i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    var file = HttpContext.Request.Form.Files[i];

                    if (file != null && file.Length > 0)
                    {
                        var postedFile = file;
                        var fileName = RandomName();
                        var fileExtension = Path.GetExtension(postedFile.FileName);
                        string mimeType = GetMimeType(file.FileName);
                        string fileSize = file.Length.ToString();

                        CloudBlobContainer blobContainer;
                        CloudBlockBlob blob;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            ms.Position = 0;

                            blobContainer = await GetCloudBlobContainer();
                            blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
                            await blob.UploadFromStreamAsync(ms);
                            returnFileUrl = blob.Uri.ToString();
                        }

                        ImageGallery fileobj = new ImageGallery()
                        {
                            Title = Title,
                            ImageUrl = returnFileUrl,
                            ImageCopyright = ImageCopyright,
                            Caption = Caption,
                            FileMimeType = mimeType,
                            FileSize = fileSize,
                            Sector = "",
                            Tags = "",
                            UseCount = 1,
                        };
                        db.ImageGalleries.Add(fileobj);
                        db.SaveChanges();
                    }
                }
                return Ok(returnFileUrl);
            }
            else
            {
                return BadRequest();
            }
            return NoContent();
        }
        [Route("api/News/UpdateCount")]
        [HttpPost]
        public IActionResult UpdateCount()
        {
            var httpRequest = HttpContext.Request;
            string Id = httpRequest.Form["Id"];
            var search = db.DevNews.Find(Id);
            search.ViewCount = search.ViewCount + 1;
            db.Entry(search).State = EntityState.Modified;
            db.SaveChanges();
            return Ok("Success");
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
            CloudBlobContainer container = blobClient.GetContainerReference("devnews");
            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return container;
        }
        private async Task<CloudBlobContainer> GetCloudBlobimagegalleryContainer()
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
        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }

        [OutputCache(Duration = 120)]
        [Route("api/getAllNews/{Edition}")]
        public IActionResult getAllNews(string Edition)
        {
            //DateTime twoDays = DateTime.Today.AddDays(-2);

            //var infocus = (from m in db.Infocus
            //               where m.Edition == Edition
            //               join s in db.DevNews on m.NewsId equals s.NewsId
            //               where s.AdminCheck == true && s.Type == "News"
            //               orderby m.SrNo
            //               select new 
            //               {
            //                   Country = s.Country,
            //                   DateTime = s.ModifiedOn,
            //                   NewsId = s.NewsId,
            //                   NewsType = s.Type,
            //                   PImageUrl = s.ImageUrl,
            //                   PTitle = s.Title,
            //                   Sector = s.Sector
            //               }).Take(6).ToList();

            DateTime twoDays = DateTime.Today.AddDays(-2);
            var lastThreeHour = DateTime.UtcNow.AddHours(-12);
            var infocus = db.RegionNewsRankings.Where(a => a.DevNews.AdminCheck == true && a.Region.Title == Edition && a.DevNews.CreatedOn > lastThreeHour && !a.DevNews.Title.Contains("News Summary") && !a.DevNews.Title.Contains("Highlights") && a.DevNews.NewsLabels != "Newsalert" && a.DevNews.Sector != "14" && a.DevNews.Sector != "18" && a.DevNews.Sector != "19" && a.DevNews.Sector != "9").Select(s => new
            {
                NewsId = s.DevNews.NewsId,
                PTitle = s.DevNews.Title,
                PImageUrl = s.DevNews.ImageUrl,
                DateTime = s.DevNews.ModifiedOn,
                NewsType = s.DevNews.Type,
                Country = s.DevNews.Country,
                Sector = s.DevNews.Sector,
                Ranking = s.Ranking
            }).OrderByDescending(a => a.DateTime).Take(65).ToList();
            var infcousResult = infocus.GroupBy(s => s.PTitle).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList();

            var latest = db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(Edition)).OrderByDescending(m => m.CreatedOn).Select(a => new { Country = a.Country, DateTime = a.CreatedOn, NewsId = a.NewsId, NewsType = a.Type, PImageUrl = a.ImageUrl, PTitle = a.Title, Sector = a.Sector }).Take(50).ToList();

            var trending = (from m in db.DevNews
                            where Edition.Contains(m.Region) && m.AdminCheck == true && m.IsSponsored == false && m.CreatedOn > twoDays
                            orderby m.ViewCount descending
                            select new
                            {
                                Country = m.Country,
                                DateTime = m.ModifiedOn,
                                NewsId = m.NewsId,
                                NewsType = m.Type,
                                PImageUrl = m.ImageUrl,
                                PTitle = m.Title,
                                Sector = m.Sector
                            }).Take(4).ToList();

            var livediscourse = (from m in db.LiveDiscourseInfocus
                                 where m.Edition == Edition
                                 join s in db.Livediscourses on m.LivediscourseId equals s.Id
                                 where s.AdminCheck == true && s.ParentId == 0
                                 orderby m.SrNo
                                 select new
                                 {
                                     Id = s.Id,
                                     PTitle = s.Title,
                                     PImageUrl = s.ImageUrl,
                                     SrNo = m.SrNo,
                                     Country = s.Country,
                                     DateTime = s.ModifiedOn,
                                     Sector = s.Sector,
                                     children = db.Livediscourses.Where(a => a.ParentId == s.Id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new DiscourseChildViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl }).Take(2).ToList()
                                 }).Take(3);

            DateTime thirtyDays = DateTime.Today.AddDays(-90);

            var opinionblogs = (from a in db.DevNews
                                where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.Region.Contains(Edition) && a.SubType != "Interview"
                                orderby a.PublishedOn descending
                                select new
                                {
                                    PTitle = a.Title,
                                    DateTime = a.PublishedOn,
                                    PImageUrl = a.ImageUrl,
                                    SubType = a.Themes,
                                    Subtitle = a.Description,
                                    Country = a.Country,
                                    NewsId = a.NewsId,
                                    Label = a.NewsLabels,
                                    NewsType = a.Type,
                                    Sector = a.Sector,
                                    Author = a.Author
                                }).Take(5).ToList();

            var interviews = (from m in db.Infocus
                              where m.Edition == Edition && m.ItemType == "Interview"
                              join a in db.DevNews on m.NewsId equals a.NewsId
                              where a.AdminCheck == true
                              select new
                              {
                                  PTitle = a.Title,
                                  DateTime = a.PublishedOn,
                                  PImageUrl = a.ImageUrl,
                                  SubType = a.Themes,
                                  Subtitle = a.Description,
                                  Country = a.Country,
                                  NewsId = a.NewsId,
                                  Label = a.NewsLabels,
                                  NewsType = a.Type,
                                  Sector = a.Sector,
                                  SrNo = m.SrNo,
                                  Author = a.Author
                              }).OrderBy(a => a.SrNo).Take(5).ToList();

            var videos = db.VideoNews.Where(a => a.AdminCheck == true && a.VideoNewsRegions.Any(r => r.Edition.Title == Edition)).Select(a => new VideoViewModel { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, FileThumbUrl = a.VideoThumbUrl, Duration = a.Duration, Country = a.Country }).OrderByDescending(m => m.CreatedOn).Take(5).ToList();

            return Json(new { infocus = infcousResult, latest = latest, trending = trending, livediscourse = livediscourse, opinionblogs = opinionblogs, interviews = interviews, videos = videos });
        }

        [OutputCache(Duration = 120)]
        [HttpPost]
        [Route("api/CodeVerify")]
        public IActionResult CodeVerify(CodeVerifyView obj)
        {
            var search = db.Users.FirstOrDefault(a => a.Email == obj.Email && a.DigitCode == obj.code);
            if (search != null)
            {
                return Json(new { msg = "Verified" });
            }
            else
            {
                return Json(new { msg = "Invalid Code" });
            }
        }

        [OutputCache(Duration = 120)]
        [HttpPost]
        [Route("api/UpdateProfile")]
        public IActionResult UpdateProfile(UpdateProfileVIew obj)
        {
            var search = db.Users.Find(obj.Id);
            var isEmailAlreadyExist = db.Users.Any(a => a.Id != obj.Id && a.Email == obj.Email);
            if (isEmailAlreadyExist)
            {
                return Json(new { msg = "Email is already Registered." });
            }
            search.FirstName = obj.FirstName;
            search.LastName = obj.LastName;
            search.Email = obj.Email;
            search.PhoneNumber = obj.Contact;
            search.DateOfBirth = obj.DateOfBirth;
            search.ProfilePic = obj.ProfilePic;
            db.Entry(search).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { msg = "Profile Updated Successfully" });
        }

        [OutputCache(Duration = 120)]
        [HttpGet]
        [Route("api/GetBlogItems/{Edition}/{page}")]
        public IActionResult GetBlogItems(string Edition, int? page)
        {
            var pageCount = page ?? 1;
            var skipCount = (pageCount - 1) * 20;
            var search = db.DevNews.Where(a => a.Type == "Blog" && a.AdminCheck == true && a.Region.Contains(Edition)).Select(a => new { Id = a.Id, NewsId = a.NewsId, PTitle = a.Title, SubType = a.SubType, PImageUrl = a.ImageUrl, Region = a.Region, IsGlobal = a.IsGlobal, DateTime = a.PublishedOn, Label = a.NewsLabels, Country = a.Country, NewsType = a.Type, Sector = a.Sector, Author = a.Author }).OrderByDescending(m => m.DateTime).Skip(skipCount).Take(20).ToList();
            return Json(search);
        }

        [OutputCache(Duration = 120)]
        [HttpGet]
        [Route("api/GetLatestStories/{Edition}/{page}")]
        public IActionResult GetLatestStories(string Edition, int? page)
        {
            var pageCount = page ?? 1;
            var skipCount = (pageCount - 1) * 20;
            DateTime tenDays = DateTime.Today.AddDays(-10);
            var infocusData = db.Infocus.Where(a => a.Edition == Edition).Select(a => a.NewsId);
            var result = db.DevNews.Where(a => !(infocusData.Contains(a.NewsId)) && a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Region.Contains(Edition) && a.Sector != null && a.NewsLabels != null).OrderByDescending(a => a.ModifiedOn).Select(a => new { Id = a.Id, PTitle = a.Title, DateTime = a.ModifiedOn, PImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewsId = a.NewsId, NewsType = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Skip(skipCount).Take(20).ToList();
            return Json(result);
        }

        [OutputCache(Duration = 120)]
        [HttpGet]
        [Route("api/GetAllInterviews/{Edition}/{page}")]
        public IActionResult GetAllInterviews(string Edition, int? page)
        {
            var pageCount = page ?? 1;
            var skipCount = (pageCount - 1) * 20;
            var search = db.DevNews.Where(a => a.Type == "Blog" && a.SubType == "Interview" && a.AdminCheck == true && a.Region.Contains(Edition)).Select(a => new { Id = a.Id, NewsId = a.NewsId, PTitle = a.Title, SubType = a.SubType, PImageUrl = a.ImageUrl, Region = a.Region, IsGlobal = a.IsGlobal, DateTime = a.PublishedOn, Label = a.NewsLabels, Country = a.Country, NewsType = a.Type, Sector = a.Sector }).OrderByDescending(m => m.DateTime).Skip(skipCount).Take(20).ToList();
            return Json(search);
        }

        [OutputCache(Duration = 120)]
        [HttpGet]
        [Route("api/GetLatestDiscourse/{Edition}/{page}")]
        public IActionResult GetLatestDiscourse(string Edition, int? page)
        {
            var pageCount = page ?? 1;
            var skipCount = (pageCount - 1) * 20;
            var LatestDiscourse = db.Livediscourses.Where(a => a.AdminCheck == true && a.ParentId == 0 && a.Region.Contains(Edition)).OrderByDescending(o => o.ModifiedOn).Select(a => new
            {
                Id = a.Id,
                PTitle = a.Title,
                PImageUrl = a.ImageUrl,
                Country = a.Country,
                DateTime = a.ModifiedOn,
                Sector = a.Sector,
                children = db.Livediscourses.Where(y => y.ParentId == a.Id && y.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new DiscourseChildViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    ImageUrl = s.ImageUrl,
                }).Take(2).ToList()
            }).Skip(skipCount).Take(20).ToList();
            return Json(LatestDiscourse);
        }

        [OutputCache(Duration = 120)]
        [Route("api/GetAllVideoNews/{Edition}/{page}")]
        public IActionResult GetAllVideoNews(string Edition, int? page)
        {
            var pageCount = page ?? 1;
            var skipCount = (pageCount - 1) * 20;
            var result = db.VideoNews.Where(a => a.AdminCheck == true && a.VideoNewsRegions.Any(r => r.Edition.Title == Edition)).Select(a => new VideoViewModel { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, FileThumbUrl = a.VideoThumbUrl, Duration = a.Duration, Country = a.Country }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20).ToList();
            return Json(result);

            //var result = db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(Edition) && a.IsVideo == true).OrderByDescending(m => m.CreatedOn).Select(a => new { Id = a.Id, PTitle = a.Title, DateTime = a.ModifiedOn, PImageUrl = a.ImageUrl, NewsId = a.NewsId, Label = a.NewsLabels, Country = a.Country,Sector = a.Sector ,NewsType = a.Type,UserNewsFiles = a.UserNewsFiles.OrderByDescending(o => o.CreatedOn).Select(k => new { Id = k.Id, Title = k.Title, FilePath = k.FilePath, FileType = k.FileType, FileCaption = k.FileCaption, NewsId = k.NewsId, ThumbnailUrl = k.FileThumbUrl, Duration = k.Duration }) }).Skip(skipCount).Take(20).ToList();
            //return Json(result);
        }

        [HttpGet]
        [Route("api/getUserUpdatedDetails/{Id}")]
        public IActionResult getUserUpdatedDetails(string Id)
        {
            var search = db.Users.Where(a => Id == a.Id).Select(a => new { Id = a.Id, Name = a.FirstName + " " + a.LastName, Email = a.Email, Phone = a.PhoneNumber, DateOfBirth = a.DateOfBirth, Profile = a.ProfilePic }).SingleOrDefault();
            return Json(search);
        }

        [HttpPost]
        [Route("api/devnews/Register")]
        public IActionResult Register(MobileUserViewModel obj)
        {
            var search = db.Users.Where(a => a.Email == obj.email).SingleOrDefault();
            if (search != null)
            {
                return Json(new { msg = "Email is already Registered." });
            }

            var user = new ApplicationUser();
            user.UserName = obj.email;
            user.EmailConfirmed = true;
            user.LastName = "";
            user.FirstName = obj.name;
            user.Email = obj.email;
            user.PasswordHash = encrypt(obj.password);
            user.PhoneNumber = obj.phonenumber;
            user.Country = "";
            user.ProfilePic = "/AdminFiles/Logo/img_avatar.png";
            user.DateOfBirth = DateTime.UtcNow.AddYears(-20);
            db.Users.Add(user);
            db.SaveChanges();

            return Json(new { Id = user.Id, FirstName = user.FirstName, Email = user.Email, msg = "Success" });
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
