using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;
using X.PagedList;

namespace Devdiscourse.Controllers.Research
{
    public class ExperimentController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public ExperimentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
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
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where (string.IsNullOrWhiteSpace(userName) || u.UserName == userName)
                    && d.CreatedOn >= firstDayOfMonth && d.CreatedOn <= lastDayOfMonth
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
            var todayEnd = todayStart.AddDays(1).AddTicks(-1);

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where (string.IsNullOrWhiteSpace(userName) || u.UserName == userName)
                    && d.CreatedOn >= todayStart && d.CreatedOn <= todayEnd
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
                            CreatedOn = d.CreatedOn
                        };

            var result = query.ToPagedList(page, 10);
            return Ok(result);
        }
        [HttpGet]
        public IActionResult TopNewsYear(string jaadu, int page = 1, string userName = "")
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();

            var currentYear = DateTime.Today.Year;
            var firstDayOfYear = new DateTime(currentYear, 1, 1);
            var lastDayOfYear = new DateTime(currentYear, 12, 31, 23, 59, 59, 999);

            var query = from d in db.DevNews
                        join u in db.Users on d.Creator equals u.Id
                        where (string.IsNullOrWhiteSpace(userName) || u.UserName == userName)
                     && d.CreatedOn >= firstDayOfYear && d.CreatedOn <= lastDayOfYear
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
                                               where a.Type == "News" && a.CreatedOn > sevenDays
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
                devNews = devNews.Where(a => a.Source.Contains(source));
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
                    HttpClient client = new();
                    string api_key = "AIzaSyCi2jYECAVn52-C_6bKcvzdU89ds3j93Pg";
                    string url = $"https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key={api_key}";

                    var requestData = new
                    {
                        contents = new[]
                        {
                                    new{
                            role = "user",
                            parts = new[]{
                                new{text = $"Give me at least 2 or at most 5 important nouns as a string separated by commas or if you don't find any then create at least 2 nouns from your understanding, here is the phrase - {title}"}
                            }
                        }
                    }
                    };

                    var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, jsonContent);
                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseContent);
                    string nounsString = jsonResponse["candidates"][0]["content"]["parts"][0]["text"].Value<string>();


                    if (nounsString == null || nounsString.ToLower().Contains(" no "))
                    {
                        return BadRequest(nounsString ?? "");
                    }

                    string[] properNouns = nounsString.Split(',');
                    string qSelect = "";
                    string qWhere = "";

                    for (int i = 0; i < properNouns.Length; i++)
                    {
                        if (i == 0)
                        {
                            qSelect += $" CASE WHEN Caption LIKE '%{properNouns[i]}%' Or Title LIKE '%{properNouns[i]}%' THEN 1 ELSE 0 END ";
                            qWhere += $" Caption LIKE '%{properNouns[i]}%' Or Title LIKE '%{properNouns[i]}%' ";
                        }
                        else
                        {
                            qSelect += $" + CASE WHEN Caption LIKE '%{properNouns[i]}%' Or Title LIKE '%{properNouns[i]}%' THEN 1 ELSE 0 END ";
                            qWhere += $" OR Caption LIKE '%{properNouns[i]}%' Or Title LIKE '%{properNouns[i]}%' ";
                        }
                    }
                    string query = $"SELECT Top 10 *, ({qSelect}) AS MatchScore FROM ImageGalleries WHERE ({qWhere}) AND ImageUrl LIKE 'https%' ORDER BY MatchScore DESC, Len(Caption)";

                    var result = db.ImageGalleries
                        .FromSqlRaw(query)
                        .ToList();

                    List<Image> images = new();
                    foreach (var item in result)
                    {
                        images.Add(new Image
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Caption = item.Caption ?? "",
                            ImageUrl = item.ImageUrl,
                            UseCount = item.UseCount ?? 0
                        });
                    }
                    return Ok(images);
                }
                else
                {
                    var result = (from g in db.ImageGalleries
                                  where g.Title.ToLower().Contains(nouns.ToLower()) || g.Caption.ToLower().Contains(nouns.ToLower())
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
        public IActionResult UpdateNews(string jaadu, Guid id, string imageUrl, string? imageCaption)
        {
            if (jaadu != "pleaseletmeaccess") return Unauthorized();
            try
            {
                int affectedRows = db.Database.ExecuteSqlRaw(
    "UPDATE DevNews SET ImageUrl = {0}, ImageCaption = {1} WHERE Id = {2}",
    imageUrl, imageCaption ?? "", id);
                return Ok(affectedRows);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class IdTitle
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
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
        public required long NewsId { get; set; }
        public required string Title { get; set; }
        public required string SubTitle { get; set; }
        public required string ImageUrl { get; set; }
        public required string Author { get; set; }
        public string? ProfilePic { get; set; }
        public int Views { get; set; }
        public required string NewsLabel { get; set; }
        public required string Region { get; set; }
        public required string Country { get; set; }
        public required string Source { get; set; }
        public required string Sector { get; set; }
        public required bool AdminCheck { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
