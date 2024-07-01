using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace DevDiscourse.Controllers
{
    public class RSSController : Controller, IDisposable
    {
        private readonly ApplicationDbContext db;

        public RSSController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // Generate a slug based on the news ID and title
        public string GenerateSecondSlug(long NewsId, string Title)
        {
            string phrase = string.Format("{0}-{1}", NewsId, Title);
            string str = RemoveAccent(phrase).ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space
            str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim(); // cut and trim 
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        // Remove accent from text
        private string RemoveAccent(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Encoding.ASCII.GetString(bytes);
        }

        // Generate Google News RSS feed
        public async Task<IActionResult> GoogleNews(int? id)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true
            };

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                await writer.WriteStartDocumentAsync();
                await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                await writer.WriteAttributeStringAsync("xmlns", "news", null, "http://www.google.com/schemas/sitemap-news/0.9");
                await writer.WriteAttributeStringAsync("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");
                await writer.WriteAttributeStringAsync("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");

                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime twoDays = DateTime.Today.AddDays(-2).AddHours(12);
                var search = (from m in db.DevNews
                              where (m.AdminCheck == true && m.CreatedOn > twoDays) || (m.NewsId == 809210 || m.NewsId == 806669)
                              orderby m.ModifiedOn descending
                              select new
                              {
                                  title = m.Title,
                                  id = m.NewsId,
                                  label = m.NewsLabels,
                                  modified = m.ModifiedOn,
                                  published = m.PublishedOn,
                                  keywords = m.Tags,
                                  image = m.ImageUrl
                              }).Take(500).ToList();

                foreach (var item in search)
                {
                    var dataTags = (item.keywords ?? "").Split(',').Where(tags => tags != "").Select(s => s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Take(10);
                    var slug = GenerateSecondSlug(item.id, item.title);
                    var url = Url.RouteUrl("ArticleDetailswithprefix", new { id = slug, prefix = item.label ?? "agency-wire" });
                    DateTime publishDate = TimeZoneInfo.ConvertTimeFromUtc(item.published, INDIAN_ZONE);
                    DateTime modifiedDate = TimeZoneInfo.ConvertTimeFromUtc(item.modified, INDIAN_ZONE);
                    var imageUrl = !string.IsNullOrEmpty(item.image) ? item.image : "";
                    //var newsImage = imageUrl.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? "https://www.devdiscourse.com/remote.axd?" + imageUrl : "https://www.devdiscourse.com" + imageUrl;
                    var newsImage = imageUrl.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? $"https://www.devdiscourse.com/Experiment/Img?imageUrl={imageUrl}" : "https://www.devdiscourse.com/Experiment/Img?imageUrl=" + string.Concat("https://www.devdiscourse.com", imageUrl);
                    await writer.WriteStartElementAsync(null, "url", null);
                    await writer.WriteElementStringAsync(null, "loc", null, "https://www.devdiscourse.com" + url);
                    await writer.WriteStartElementAsync("news", "news", "http://www.google.com/schemas/sitemap-news/0.9");
                    await writer.WriteStartElementAsync("news", "publication", "http://www.google.com/schemas/sitemap-news/0.9");
                    await writer.WriteElementStringAsync("news", "name", "http://www.google.com/schemas/sitemap-news/0.9", "Devdiscourse");
                    await writer.WriteElementStringAsync("news", "language", "http://www.google.com/schemas/sitemap-news/0.9", "en");
                    await writer.WriteEndElementAsync();
                    await writer.WriteElementStringAsync("news", "publication_date", "http://www.google.com/schemas/sitemap-news/0.9", publishDate.ToString("yyyy-MM-ddTHH:mm:sszzzz"));
                    await writer.WriteStartElementAsync("news", "title", "http://www.google.com/schemas/sitemap-news/0.9");
                    await writer.WriteCDataAsync(item.title);
                    await writer.WriteEndElementAsync();
                    await writer.WriteElementStringAsync("news", "keywords", "http://www.google.com/schemas/sitemap-news/0.9", string.Join(", ", dataTags));
                    await writer.WriteEndElementAsync();
                    await writer.WriteElementStringAsync(null, "lastmod", null, modifiedDate.ToString("yyyy-MM-ddTHH:mm:sszzzz"));
                    await writer.WriteStartElementAsync("image", "image", "http://www.google.com/schemas/sitemap-image/1.1");
                    await writer.WriteStartElementAsync("image", "loc", "http://www.google.com/schemas/sitemap-image/1.1");
                    await writer.WriteCDataAsync(newsImage + "&width=960");
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndElementAsync();
                }

                await writer.WriteEndElementAsync();
                await writer.WriteEndDocumentAsync();
            }

            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }

        // Generate Livediscourse RSS feed
        public async Task<IActionResult> Livediscourse()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true
            };

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                await writer.WriteStartDocumentAsync();
                await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                await writer.WriteAttributeStringAsync("xmlns", "news", null, "http://www.google.com/schemas/sitemap-news/0.9");
                await writer.WriteAttributeStringAsync("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");
                await writer.WriteAttributeStringAsync("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");

                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var search = (from m in db.Livediscourses
                              where m.AdminCheck == true && m.ParentId == 0
                              join c in db.Livediscourses on m.Id equals c.ParentId
                              orderby m.ModifiedOn descending
                              select new
                              {
                                  title = c.Title,
                                  id = c.Id,
                                  modified = c.ModifiedOn,
                                  published = c.PublishedOn,
                                  parentId = c.ParentId,
                                  parentTitle = m.Title,
                                  keywords = c.Tags,
                                  image = c.ImageUrl
                              }).Take(300).ToList();

                foreach (var item in search)
                {
                    var slug = item.parentId == 0 ? GenerateSecondSlug(item.id, item.title) : GenerateSecondSlug(item.parentId, item.parentTitle);
                    var url = Url.RouteUrl("LivediscourseArticle", new { id = slug });
                    var publishDate = TimeZoneInfo.ConvertTimeFromUtc(item.published, INDIAN_ZONE).ToString("yyyy-MM-ddTHH:mm:sszzzz");
                    var modifiedDate = TimeZoneInfo.ConvertTimeFromUtc(item.modified, INDIAN_ZONE).ToString("yyyy-MM-ddTHH:mm:sszzzz");
                    var imageUrl = !string.IsNullOrEmpty(item.image) ? item.image : "";
                    //var newsImage = imageUrl.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? "https://www.devdiscourse.com/remote.axd?" + imageUrl : "https://www.devdiscourse.com" + imageUrl;
                    var newsImage = imageUrl.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? $"https://www.devdiscourse.com/Experiment/Img?imageUrl={imageUrl}" : "https://www.devdiscourse.com/Experiment/Img?imageUrl=" + string.Concat("https://www.devdiscourse.com", imageUrl);

                    await writer.WriteStartElementAsync(null, "url", null);
                    await writer.WriteElementStringAsync(null, "loc", null, item.parentId == 0 ? "https://www.devdiscourse.com" + url : "https://www.devdiscourse.com" + url + "#post_" + item.id);
                    await writer.WriteStartElementAsync("news", "news", "http://www.google.com/schemas/sitemap-news/0.9");
                    await writer.WriteStartElementAsync("news", "publication", "http://www.google.com/schemas/sitemap-news/0.9");
                    await writer.WriteElementStringAsync("news", "name", "http://www.google.com/schemas/sitemap-news/0.9", "Devdiscourse");
                    await writer.WriteElementStringAsync("news", "language", "http://www.google.com/schemas/sitemap-news/0.9", "en");
                    await writer.WriteEndElementAsync();
                    await writer.WriteElementStringAsync("news", "publication_date", "http://www.google.com/schemas/sitemap-news/0.9", publishDate);
                    await writer.WriteStartElementAsync("news", "title", "http://www.google.com/schemas/sitemap-news/0.9");
                    await writer.WriteCDataAsync(item.title);
                    await writer.WriteEndElementAsync();
                    await writer.WriteElementStringAsync("news", "keywords", "http://www.google.com/schemas/sitemap-news/0.9", item.keywords);
                    await writer.WriteEndElementAsync();
                    await writer.WriteElementStringAsync(null, "lastmod", null, modifiedDate);
                    await writer.WriteStartElementAsync("image", "image", "http://www.google.com/schemas/sitemap-image/1.1");
                    await writer.WriteStartElementAsync("image", "loc", "http://www.google.com/schemas/sitemap-image/1.1");
                    await writer.WriteCDataAsync(newsImage + "&width=960");
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndElementAsync();
                }

                await writer.WriteEndElementAsync();
                await writer.WriteEndDocumentAsync();
            }

            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
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
