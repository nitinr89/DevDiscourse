﻿using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Devdiscourse.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using X.PagedList;

namespace Devdiscourse.Controllers.Research
{
    public class ExperimentController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ImageResizer imageResizer = new(new HttpClient());
        public ExperimentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        [OutputCache(Duration = 86400, PolicyName = "MyOutputCachePolicy")]
        public async Task<IActionResult> Img(string imageUrl, int width = 0, int height = 0, string mode = "crop", string format = "jpeg", int quality = 80)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return BadRequest();
            if (imageUrl.StartsWith("/")) imageUrl = "https://www.devdiscourse.com" + imageUrl;
            var resizedImageStream = await imageResizer.ResizeImageFromUrlAsync(imageUrl, width, height, mode, format, quality);
            return File(resizedImageStream, $"image/{format}");
        }

        [HttpGet]
        [OutputCache(Duration = 86400, PolicyName = "MyOutputCachePolicy")]
        public IActionResult TopNews(string jaadu, int page = 1, string userName = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where string.IsNullOrWhiteSpace(userName) || u.UserName == userName
                        orderby d.ViewCount descending
                        select new TopNewsItem
                        {
                            Id = d.Id,
                            NewsId = d.NewsId,
                            Title = d.Title ?? "",
                            SubTitle = d.SubTitle ?? "",
                            ImageUrl = d.ImageUrl ?? "",
                            Author = u.FirstName + " " + u.LastName,
                            ProfilePic = u.ProfilePic,
                            Views = d.ViewCount,
                            Sector = d.Sector ?? "",
                            AdminCheck = d.AdminCheck,
                            Region = d.Region ?? "",
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            OriginalSource = d.OriginalSource ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.ToPagedList(page, 10);
            return Ok(result);
        }
        [HttpGet]
        public IActionResult TopNewsMonth(string jaadu, int page = 1, string userName = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where (string.IsNullOrWhiteSpace(userName) || u.UserName == userName)
                    && d.CreatedOn >= firstDayOfMonth
                        orderby d.ViewCount descending
                        select new TopNewsItem
                        {
                            Id = d.Id,
                            NewsId = d.NewsId,
                            Title = d.Title ?? "",
                            SubTitle = d.SubTitle ?? "",
                            ImageUrl = d.ImageUrl ?? "",
                            Author = u.FirstName + " " + u.LastName,
                            ProfilePic = u.ProfilePic,
                            Views = d.ViewCount,
                            Sector = d.Sector ?? "",
                            AdminCheck = d.AdminCheck,
                            Region = d.Region ?? "",
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            OriginalSource = d.OriginalSource ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.ToPagedList(page, 10);
            return Ok(result);
        }
        [HttpGet]
        public IActionResult TopNewsToday(string jaadu, int page = 1, string userName = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();

            var todayStart = DateTime.Today;

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where (string.IsNullOrWhiteSpace(userName) || u.UserName == userName)
                    && d.CreatedOn >= todayStart
                        orderby d.ViewCount descending
                        select new TopNewsItem
                        {
                            Id = d.Id,
                            NewsId = d.NewsId,
                            Title = d.Title ?? "",
                            SubTitle = d.SubTitle ?? "",
                            ImageUrl = d.ImageUrl ?? "",
                            Author = u.FirstName + " " + u.LastName,
                            ProfilePic = u.ProfilePic,
                            Views = d.ViewCount,
                            Sector = d.Sector ?? "",
                            AdminCheck = d.AdminCheck,
                            Region = d.Region ?? "",
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            OriginalSource = d.OriginalSource ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.ToPagedList(page, 10);
            return Ok(result);
        }
        [HttpGet]
        [OutputCache(Duration = 86400, PolicyName = "MyOutputCachePolicy")]
        public IActionResult TopNewsYear(string jaadu, int page = 1, string userName = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();

            var currentYear = DateTime.Today.Year;
            var firstDayOfYear = new DateTime(currentYear, 1, 1);

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where (string.IsNullOrWhiteSpace(userName) || u.UserName == userName)
                     && d.CreatedOn >= firstDayOfYear
                        orderby d.ViewCount descending
                        select new TopNewsItem
                        {
                            Id = d.Id,
                            NewsId = d.NewsId,
                            Title = d.Title ?? "",
                            SubTitle = d.SubTitle ?? "",
                            ImageUrl = d.ImageUrl ?? "",
                            Author = u.FirstName + " " + u.LastName,
                            ProfilePic = u.ProfilePic,
                            Sector = d.Sector ?? "",
                            AdminCheck = d.AdminCheck,
                            Region = d.Region ?? "",
                            Views = d.ViewCount,
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            OriginalSource = d.OriginalSource ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.ToPagedList(page, 10);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Authors(string jaadu)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            List<Author> authors = new();
            var roles = db.Roles.Where(a => a.Name == "Admin" || a.Name == "Upfront" || a.Name == "Author").ToList();
            foreach (var item in roles)
            {
                var users = await userManager.GetUsersInRoleAsync(item.Name);
                foreach (var user in users)
                {
                    authors.Add(new Author { Id = user.Id, UserName = user.UserName, Name = user.FirstName + " " + user.LastName });
                }
            }
            return Ok(authors);
        }
        [HttpGet]
        public IActionResult Labels(string jaadu)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            var labels = (from l in db.Labels
                          select new IdTitle { Id = l.Id, Title = l.Title ?? "" }).ToList();
            return Ok(labels);
        }
        [HttpGet]
        public IActionResult Sectors(string jaadu)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            var sectors = (from s in db.DevSectors
                           where s.Id != 8 && s.Id != 16
                           orderby s.Title
                           select new IdTitle { Id = s.Id, Title = s.Title ?? "" }).ToList();
            return Ok(sectors);
        }

        [HttpGet]
        public IActionResult News(string jaadu,
            int page = 1, string label = "",
            string sector = "", string source = "",
            string text = "", string author = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            DateTime sevenDays = DateTime.Today.AddDays(-7);
            IQueryable<TopNewsItem> devNews = (from a in db.DevNews
                                               where a.CreatedOn > sevenDays
                                               select new TopNewsItem
                                               {
                                                   Id = a.Id,
                                                   NewsId = a.NewsId,
                                                   NewsLabel = a.NewsLabels ?? "",
                                                   Sector = a.Sector ?? "",
                                                   Title = a.Title ?? "",
                                                   SubTitle = a.SubTitle ?? "",
                                                   Author = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName,
                                                   Region = a.Region ?? "",
                                                   Country = a.Country ?? "",
                                                   ImageUrl = a.ImageUrl ?? "",
                                                   Source = a.Source ?? "",
                                                   OriginalSource = a.OriginalSource ?? "",
                                                   AdminCheck = a.AdminCheck,
                                                   CreatedOn = a.CreatedOn,
                                                   Views = a.ViewCount,
                                                   ProfilePic = a.Creator
                                               });

            if (!String.IsNullOrWhiteSpace(label))
            {
                devNews = devNews.Where(a => a.NewsLabel.Contains("," + label + ",") || a.NewsLabel.StartsWith("," + label) || a.NewsLabel.EndsWith(label + ",") || a.NewsLabel.Equals(label));
            }
            if (!String.IsNullOrWhiteSpace(sector))
            {
                devNews = devNews.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
            }
            if (!String.IsNullOrWhiteSpace(source))
            {
                devNews = devNews.Where(a => a.OriginalSource.Contains(source));
            }
            if (!String.IsNullOrWhiteSpace(author))
            {
                devNews = devNews.Where(a => a.ProfilePic == author);
            }
            if (!String.IsNullOrWhiteSpace(text))
            {
                devNews = devNews.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
            }
            return Ok(devNews.OrderByDescending(a => a.CreatedOn).ToPagedList(page, 10));
        }

        [HttpGet]
        public async Task<IActionResult> Images(string jaadu, int page = 1, string title = "", string nouns = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(nouns)) return BadRequest();

            try
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    const string ApiKey = "Bearer api_key";
                    try
                    {
                        var messages = new[] { new { role = "user", content = "What noun is this material about?, Is it a name of person?, your response should be json string like '{noun:'virat kohli',isNameOfPerson:'true'}'. Content: " + title } };
                        var data = new { model = "gpt-4o", messages };
                        string jsonString = JsonConvert.SerializeObject(data);
                        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Add("Authorization", ApiKey);

                        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                        response.EnsureSuccessStatusCode();

                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic? responseData = JsonConvert.DeserializeObject(responseContent);
                        string? newsStirng = responseData?.choices[0].message.content;
                        if (newsStirng == null) return BadRequest();
                        if (newsStirng.StartsWith("```json"))
                        {
                            newsStirng = newsStirng.Replace("```json", "");
                            newsStirng = newsStirng.Replace("```", "");
                        }
                        newsStirng = newsStirng.Replace("\r", "");
                        newsStirng = newsStirng.Replace("\n", "");
                        newsStirng = newsStirng.Replace("\t", "");

                        AiResponce? aiResponce = JsonConvert.DeserializeObject<AiResponce>(newsStirng);
                        if (aiResponce == null) return BadRequest();
                        nouns = aiResponce.Noun.ToLower();
                        if (aiResponce.IsNameOfPerson)
                        {
                            var result = (from g in db.ImageGalleries
                                          where g.AI != true && (g.Title.ToLower().Contains(nouns) || g.Caption.ToLower().Contains(nouns))
                                          orderby g.CreatedOn descending
                                          select new Image
                                          {
                                              Id = g.Id,
                                              Title = g.Title,
                                              Caption = g.Caption ?? "",
                                              ImageUrl = g.ImageUrl,
                                              UseCount = g.UseCount ?? 0
                                          }).ToPagedList(page, 10);
                            return Ok(result);
                        }
                        else
                        {
                            var result = (from g in db.ImageGalleries
                                          where g.AI == true && (g.Title.ToLower().Contains(nouns) || g.Caption.ToLower().Contains(nouns))
                                          orderby g.CreatedOn descending
                                          select new Image
                                          {
                                              Id = g.Id,
                                              Title = g.Title,
                                              Caption = g.Caption ?? "",
                                              ImageUrl = g.ImageUrl,
                                              UseCount = g.UseCount ?? 0
                                          }).ToPagedList(page, 10);
                            return Ok(result);
                        }
                    }
                    catch (Exception _) { return BadRequest(); }
                }
                else
                {
                    nouns = nouns.ToLower();
                    var result = (from g in db.ImageGalleries
                                  where g.Title.ToLower().Contains(nouns) || g.Caption.ToLower().Contains(nouns)
                                  orderby g.CreatedOn descending
                                  select new Image
                                  {
                                      Id = g.Id,
                                      Title = g.Title,
                                      Caption = g.Caption ?? "",
                                      ImageUrl = g.ImageUrl,
                                      UseCount = g.UseCount ?? 0
                                  }).ToPagedList(page, 10);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UpdateNews(string jaadu, Guid id, string? imageUrl, string? imageCaption, string? sector, string? adminCheck)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            DevNews? news = db.DevNews.Find(id);
            if (news == null) return BadRequest();
            try
            {
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    int affectedRows = db.Database.ExecuteSqlRaw(
        "UPDATE DevNews SET ImageUrl = {0}, ImageCaption = {1} WHERE Id = {2}",
        imageUrl, imageCaption ?? "", id);
                    return Ok(affectedRows);
                }
                else if (!string.IsNullOrWhiteSpace(sector))
                {
                    int affectedRows = db.Database.ExecuteSqlRaw(
        "UPDATE DevNews SET Sector = {0} WHERE Id = {1}", sector, id);
                    return Ok(affectedRows);
                }
                else if (adminCheck is "true" or "false")
                {
                    bool isAdminCheck = adminCheck == "true";
                    int affectedRows = db.Database.ExecuteSqlRaw(
        "UPDATE DevNews SET AdminCheck = {0} WHERE Id = {1}", isAdminCheck, id);
                    return Ok(affectedRows);
                }
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult SponsoredNews(string jaadu)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            var sponsoredNews = (from sn in db.SponsoredNews
                                 join d in db.DevNews on sn.NewsId equals d.Id
                                 where sn.IsActive == true && sn.EndTime > DateTime.UtcNow
                                 orderby sn.CreatedOn descending
                                 select new TopNewsItem
                                 {
                                     Id = d.Id,
                                     NewsId = d.NewsId,
                                     Title = d.Title ?? "",
                                     SubTitle = d.SubTitle ?? "",
                                     ImageUrl = d.ImageUrl ?? "",
                                     Author = "",
                                     ProfilePic = "",
                                     Sector = d.Sector ?? "",
                                     AdminCheck = d.AdminCheck,
                                     Region = d.Region ?? "",
                                     Views = d.ViewCount,
                                     NewsLabel = d.NewsLabels ?? "agency-wire",
                                     Country = d.Country ?? "Global",
                                     Source = d.Source ?? "",
                                     OriginalSource = d.OriginalSource ?? "",
                                     CreatedOn = d.CreatedOn
                                 }).ToList();
            return Ok(sponsoredNews);
        }

        [HttpPost]
        public IActionResult AddSponsoredNews(string jaadu, Guid id, int position = 0, int days = 0, int hours = 0, int minutes = 0)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            if (days > 7 || position < 0 || position > 5) return BadRequest();
            try
            {
                DevNews? devNews = db.DevNews.Find(id);
                if (devNews == null) return BadRequest();
                bool success = int.TryParse(devNews.Sector?.Split(",")[0], out int sector);
                if (!success) return BadRequest();
                if (days == 0 && hours == 0 && minutes == 0) return BadRequest();
                DateTime endDate;
                if (days > 0) endDate = DateTime.Today.AddDays(days).AddTicks(-1);
                else endDate = DateTime.UtcNow.AddHours(hours).AddMinutes(minutes).AddTicks(-1);

                SponsoredNews? dbSponsoredNews = db.SponsoredNews.FirstOrDefault(f => f.NewsId == id);
                if (dbSponsoredNews == null)
                {
                    SponsoredNews sponsoredNews = new()
                    {
                        NewsId = id,
                        Position = position,
                        EndTime = endDate,
                        IsActive = true,
                        Sector = sector,
                        CreatedOn = DateTime.UtcNow
                    };
                    db.SponsoredNews.Add(sponsoredNews);
                    db.SaveChanges();
                    return Ok(1);
                }
                else
                {
                    dbSponsoredNews.IsActive = true;
                    dbSponsoredNews.Position = position;
                    dbSponsoredNews.EndTime = endDate;
                    dbSponsoredNews.Sector = sector;
                    db.SponsoredNews.Update(dbSponsoredNews);
                    db.SaveChanges();
                    return Ok(1);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeleteSponsoredNews(string jaadu, Guid id)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            try
            {
                SponsoredNews? sponsoredNews = db.SponsoredNews.FirstOrDefault(f => f.NewsId == id);
                if (sponsoredNews == null) return BadRequest();
                sponsoredNews.IsActive = false;
                db.SponsoredNews.Update(sponsoredNews);
                db.SaveChanges();
                return Ok(1);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Trending(string jaadu, string time, int page = 1)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
            DateTime dateTime;
            if (time == "day") dateTime = today;
            else if (time == "week") dateTime = weekStart;
            else dateTime = today;

            string sql = @"
            SELECT 
                d.Id,
                d.NewsId,
                d.Title,
                d.SubTitle,
                d.ImageUrl,
                (u.FirstName + ' ' + u.LastName) AS Author,
                u.ProfilePic,
                d.Sector,
                d.AdminCheck,
                d.Region,
                d.ViewCount AS Views,
                ISNULL(d.NewsLabels, 'agency-wire') AS NewsLabel,
                ISNULL(d.Country, 'Global') AS Country,
                d.Source,
                d.OriginalSource,
                d.CreatedOn,
                COUNT(*) AS TodayViews
            FROM TrendingNews tn
            INNER JOIN DevNews d ON tn.NewsId = d.Id
            INNER JOIN AspNetUsers u ON d.Creator = u.Id
            WHERE tn.ViewedOn >= @todayStart
            GROUP BY 
                d.Id,
                d.NewsId,
                d.Title,
                d.SubTitle,
                d.ImageUrl,
                u.FirstName,
                u.LastName,
                u.ProfilePic,
                d.Sector,
                d.AdminCheck,
                d.Region,
                d.ViewCount,
                d.NewsLabels,
                d.Country,
                d.Source,
                d.OriginalSource,
                d.CreatedOn
            ORDER BY TodayViews DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var parameters = new[]
            {
                new SqlParameter("@todayStart", dateTime),
                new SqlParameter("@offset", (page - 1) * 10),
                new SqlParameter("@pageSize", 10)
            };
            var topNewsItems = db.TopNewsItems
                .FromSqlRaw(sql, parameters)
                .ToList();
            return Ok(topNewsItems);
        }
        [HttpGet]
        public IActionResult TrendingNow(string jaadu)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            DateTime dateTime = DateTime.UtcNow.AddHours(-1);

            string sql = @"
            SELECT TOP 1
                d.Id,
                d.NewsId,
                d.Title,
                d.SubTitle,
                d.ImageUrl,
                (u.FirstName + ' ' + u.LastName) AS Author,
                u.ProfilePic,
                d.Sector,
                d.AdminCheck,
                d.Region,
                d.ViewCount AS Views,
                ISNULL(d.NewsLabels, 'agency-wire') AS NewsLabel,
                ISNULL(d.Country, 'Global') AS Country,
                d.Source,
                d.OriginalSource,
                d.CreatedOn,
                COUNT(*) AS TodayViews
            FROM TrendingNews tn
            INNER JOIN DevNews d ON tn.NewsId = d.Id
            INNER JOIN AspNetUsers u ON d.Creator = u.Id
            WHERE tn.ViewedOn >= @todayStart
            GROUP BY 
                d.Id,
                d.NewsId,
                d.Title,
                d.SubTitle,
                d.ImageUrl,
                u.FirstName,
                u.LastName,
                u.ProfilePic,
                d.Sector,
                d.AdminCheck,
                d.Region,
                d.ViewCount,
                d.NewsLabels,
                d.Country,
                d.Source,
                d.OriginalSource,
                d.CreatedOn
            ORDER BY TodayViews DESC;";

            var parameters = new[]
            {
                new SqlParameter("@todayStart", dateTime)
            };
            var topNewsItems = db.TopNewsItems
                .FromSqlRaw(sql, parameters)
                .ToList();
            return Ok(topNewsItems);
        }

        [HttpGet]
        public IActionResult Views(string jaadu, string time, string? newsId)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
            DateTime dateTime;
            if (time == "day") dateTime = today;
            else if (time == "week") dateTime = weekStart;
            else dateTime = today;

            var query = db.TrendingNews
                .Where(f => f.ViewedOn >= dateTime && (string.IsNullOrWhiteSpace(newsId) || f.NewsId == new Guid(newsId)))
                .GroupBy(o => o.Country)
                .Select(g => new IdTitle
                {
                    Id = g.Count(),
                    Title = g.Key == null ? "Others" : g.Key == "Not found" ? "Others" : g.Key
                }).OrderByDescending(o => o.Id).ToList();
            return Ok(query);
        }

        [HttpGet]
        public IActionResult Controlls(string jaadu)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            return Ok(db.Controlls.ToList());
        }
        [HttpGet]
        public IActionResult UpdateControlls(string jaadu, string name, string value)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            Controlls? controll = db.Controlls.FirstOrDefault(o => o.Name == name);
            if (controll == null) return BadRequest();
            controll.Value = value;
            db.Controlls.Update(controll);
            db.SaveChanges();
            return Ok(1);
        }

        [HttpGet("/news/{sector}")]
        [OutputCache(Duration = 60, PolicyName = "MyOutputCachePolicy")]
        public async Task<List<NewsViewModel>> News([FromRoute] string sector)
        {
            DateTime twoDays = DateTime.UtcNow.AddDays(-2);
            var resultList = await db.RegionNewsRankings
                .Where(dn => dn.DevNews.CreatedOn > twoDays &&
                   dn.DevNews.AdminCheck == true &&
                   dn.DevNews.Sector == sector &&
                   dn.Region.Title == "Global Edition" &&
                   dn.DevNews.IsSponsored != true
              ).OrderByDescending(dn => dn.DevNews.CreatedOn).Take(30).Select(dn => new NewsViewModel
              {
                  NewsId = dn.DevNews.NewsId,
                  Title = dn.DevNews.Title,
                  ImageUrl = dn.DevNews.ImageUrl,
                  CreatedOn = dn.DevNews.ModifiedOn,
                  Subtitle = dn.DevNews.SubTitle,
                  SubType = dn.DevNews.SubType,
                  Country = dn.DevNews.Country,
                  Sector = dn.DevNews.Sector,
                  Label = dn.DevNews.NewsLabels,
                  Ranking = dn.Ranking
              }).AsNoTracking().ToListAsync();
            return resultList.GroupBy(s => s.Title).Select(a => a.First()).OrderByDescending(a => a.Ranking).Take(6).ToList();
        }
    }

    public class AiResponce
    {
        public required string Noun { get; set; }
        public bool IsNameOfPerson { get; set; }
    }
    public class IdTitle
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }
    public class Image
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Caption { get; set; }
        public required int UseCount { get; set; }
        public required string ImageUrl { get; set; }
    }
    public class Author
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Name { get; set; }
    }
    public class TopNewsItem
    {
        public required Guid Id { get; set; }
        public long? NewsId { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? ImageUrl { get; set; }
        public string? Author { get; set; }
        public string? ProfilePic { get; set; }
        public int? Views { get; set; }
        public int? TodayViews { get; set; }
        public string? NewsLabel { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? Source { get; set; }
        public string? OriginalSource { get; set; }
        public string? Sector { get; set; }
        public bool? AdminCheck { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
