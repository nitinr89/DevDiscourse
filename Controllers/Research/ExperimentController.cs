using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.Research
{
    public class ExperimentController : Controller
    {
        private readonly ApplicationDbContext db;
        public ExperimentController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult TopNews(string jaadu, string userName = "")
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
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.Take(10).ToList();
            return Ok(result);
        }
        [HttpGet]
        public IActionResult TopNewsMonth(string jaadu, string userName = "")
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
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.Take(10).ToList();
            return Ok(result);
        }
        [HttpGet]
        public IActionResult TopNewsToday(string jaadu, string userName = "")
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
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.Take(10).ToList();
            return Ok(result);
        }
        [HttpGet]
        public IActionResult TopNewsYear(string jaadu, string userName = "")
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
                            Views = d.ViewCount,
                            NewsLabel = d.NewsLabels ?? "agency-wire",
                            Country = d.Country ?? "Global",
                            Source = d.Source ?? "",
                            CreatedOn = d.CreatedOn
                        };

            var result = query.Take(10).ToList();
            return Ok(result);
        }

    }
    public class TopNewsItem
    {
        public required Guid Id { get; set; }
        public required long NewsId { get; set; }
        public required string Title { get; set; }
        public required string SubTitle { get; set; }
        public required string ImageUrl { get; set; }
        public required string Author { get; set; }
        public required string ProfilePic { get; set; }
        public int Views { get; set; }
        public required string NewsLabel { get; set; }
        public required string Country { get; set; }
        public required string Source { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
