using Devdiscourse.Data;
using Devdiscourse.Hubs;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Html2Amp;
using Html2Amp.Sanitization;
using Html2Amp.Sanitization.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Devdiscourse.Controllers.API
{
    [ApiController]
    [Route("api/Search")]
    public class SearchApiController : Controller
    {
        public ApplicationDbContext db;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _config;
        public SearchApiController(ApplicationDbContext _db, IDistributedCache cache, IConfiguration config)
        {
            db = _db;
            _cache = cache;
            _config = config;
        }
        [HttpGet]
        [Route("GetVideoNews/{reg}/{page}")]
        public IQueryable<VideoViewModel> GetVideoNews(string reg = "Global Edition", int page = 1)
        {
            var skipCount = (page - 1) * 20;
            if (reg == "Global Edition")
            {
                var result = db.VideoNews
                    .Where(a => a.AdminCheck == true)
                    .Select(a => new VideoViewModel { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, FileThumbUrl = a.VideoThumbUrl, Duration = a.Duration }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
                return result;
            }
            else
            {
                var result = db.VideoNews.
                    Where(a => a.AdminCheck == true && a.VideoNewsRegions.Any(r => r.Edition.Title == reg))
                    .Select(a => new VideoViewModel { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, FileThumbUrl = a.VideoThumbUrl, Duration = a.Duration }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
                return result;
            }
        }

        [HttpGet]
        [Route("GetHomeVideoNews/{reg}")]
        public IQueryable<VideoViewModel> GetHomeVideoNews(string reg = "Global Edition")
        {
            if (reg == "Global Edition")
            {
                var result = db.VideoNews
                    .Where(a => a.AdminCheck == true)
                    .Select(a => new VideoViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        CreatedOn = a.CreatedOn,
                        FileThumbUrl = a.VideoThumbUrl,
                        Duration = a.Duration
                    }).OrderByDescending(m => m.CreatedOn).Take(5);
                return result;
            }
            else
            {
                var result = db.VideoNews
                    .Where(a => a.AdminCheck == true && a.VideoNewsRegions.Any(r => r.Edition.Title == reg))
                    .Select(a => new VideoViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        CreatedOn = a.CreatedOn,
                        FileThumbUrl = a.VideoThumbUrl,
                        Duration = a.Duration
                    }).OrderByDescending(m => m.CreatedOn).Take(5);
                return result;
            }
        }

        [HttpGet]
        [OutputCache(Duration = 60, VaryByRouteValueNames = new[] { "sector", "reg", "page" })]
        [Route("GetSectorNews/{sector}/{reg}/{page}")]
        public async Task<IActionResult> GetSectorNews(string sector, string reg = "Global Edition", int page = 1)
        {
            const int pageSize = 10;
            var skipCount = (page - 1) * pageSize;

            // Redis cache – short TTL so latest news still appears quickly for SEO
            string cacheKey = $"api:sectorNews:{sector}:{reg}:{page}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return Ok(JsonSerializer.Deserialize<List<NewsViewModel>>(cached));

            DateTime ninetyDays = DateTime.UtcNow.AddDays(-90);

            var result = await db.RegionNewsRankings
                .AsNoTracking()
                .Where(a => a.DevNews.AdminCheck == true && a.DevNews.Sector == sector
                && a.Region.Title == reg
                && a.DevNews.IsSponsored == false
                && a.DevNews.CreatedOn > ninetyDays)
                .OrderByDescending(a => a.DevNews.CreatedOn)
                .ThenByDescending(s => s.Ranking)
                .Skip(skipCount)
                .Select(a => new NewsViewModel
                {
                    Title = a.DevNews.Title,
                    NewsId = a.DevNews.NewsId,
                    ImageUrl = a.DevNews.ImageUrl,
                    Subtitle = a.DevNews.SubTitle,
                    Country = a.DevNews.Country,
                    CreatedOn = a.DevNews.ModifiedOn,
                    Sector = a.DevNews.Type,
                    SubType = a.DevNews.SubType,
                    Label = a.DevNews.NewsLabels,
                    Ranking = a.Ranking
                })
                .Take(pageSize)
                .ToListAsync();

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetTagsNews/{tag}/{reg}/{page}")]
        [OutputCache(Duration = 60)]
        public IQueryable<LatestNewsView> GetTagsNews(string tag, string reg = "Global Edition", int page = 1)
        {
            DateTime oneMonth = DateTime.UtcNow.AddDays(-40);
            var skipCount = (page - 1) * 20;

            var result = db.DevNews
                .Where(a => a.CreatedOn > oneMonth && a.AdminCheck == true && a.Tags.Contains(tag))
                .Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels, Country = a.Country }).Distinct().OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
            return result;

        }

        [HttpGet]
        [Route("GetSDGNews/{page}")]
        public IQueryable<LatestNewsView> GetSDGNews(int page = 1)
        {
            DateTime oneMonth = DateTime.Today.AddDays(-60);
            var skipCount = (page - 1) * 20;
            var result = db.DevNews
                .Where(a => a.AdminCheck == true && a.Category.Contains("13"))
                .Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels, Country = a.Country }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
            return result;

        }

        [HttpGet]
        [Route("NewsForMongo")]
        public IActionResult NewsForMongo(int page = 1)
        {
            DateTime oneMonth = DateTime.Today.AddDays(-45);
            var result = db.DevNews.Where(a => a.CreatedOn > oneMonth).Select(a => new { Id = a.NewsId, keywords = a.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) }).ToList();//some change
            return Ok(result);
        }
        [HttpGet]
        [Route("GetAllNews/{page}")]
        public IQueryable<LatestNewsView> GetAllNews(int page = 1)
        {
            var skipCount = (page - 1) * 20;
            var result = db.DevNews.Where(a => a.AdminCheck == true).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels, Country = a.Country }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
            return result;
        }
        [HttpGet]
        [Route("GetAmpLatestNewsItems/{__amp_source_origin?}")]
        public IActionResult GetAmpLatestNewsItems(string __amp_source_origin = "")
        {
            DateTime threeDays = DateTime.Today.AddDays(-2);
            var resultData = (db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > threeDays).OrderByDescending(m => m.CreatedOn)
                .Select(b => new LatestNewsView
                {
                    Title = b.Title,
                    NewId = b.NewsId,
                    Label = b.NewsLabels,
                    ImageUrl = b.ImageUrl,
                    Country = b.Country
                })).Take(5);

            var returnData = resultData.AsEnumerable()
                .Select(a => new
                {
                    a.Title,
                    a.Label,
                    a.ImageUrl,
                    a.Country,
                    Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString()
                });
            return Ok(new { items = returnData, hasMorePages = resultData.Any() });
        }

        [Route("GetPreviousSectorAmpNews/{id}/{sector}/{reg}/{__amp_source_origin?}")]
        [HttpGet]
        public IActionResult GetPreviousSectorAmpNews(long id, string sector, string reg = "Global Edition", string __amp_source_origin = "")
        {

            var search = db.DevNews.FirstOrDefault(a => a.NewsId == id);
            DateTime tenDays = search.CreatedOn.AddDays(-2);
            List<ApiNewsView> resultNewsList = new List<ApiNewsView>();
            var newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && (a.Sector == sector) && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Take(9).ToList();
            //}
            foreach (var news in newsList)
            {
                var tags = news.Tags ?? "";
                var tagArray = tags.Split(',').ToList();
                var DataDesc = GetAmpHtml(news.Description);
                var newsSector = GetSectorTitle(news.Sector);
                var newsSourceUrl = GetSourceUrl(news.Source);
                foreach (var tag in tagArray.Where(a => a.Length > 2).OrderBy(a => a.Length))
                {
                    var replaceTag = tag.Replace("(", "");
                    replaceTag = replaceTag.Replace(")", "");
                    DataDesc = Regex.Replace(DataDesc, " " + replaceTag.Trim() + " ", " <a href=\"/news?tag=" + replaceTag.Trim() + "\">" + replaceTag.Trim() + "</a> ");
                }
                var authorName = "";
                var authorImage = "";

                if (!string.IsNullOrEmpty(news.Themes))
                {
                    authorImage = news.Themes.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? news.Themes : news.Themes;
                    authorName = news.Author?.Trim();

                }
                else
                {
                    authorImage = news.ApplicationUsers?.ProfilePic;
                    authorName = news.ApplicationUsers?.FirstName;
                }
                var disclaimer = "";
                if ((news.OriginalSource == "Reuters" || news.OriginalSource == "PTI" || news.OriginalSource == "IANS" || news.OriginalSource == "ANI") && (news.Source ?? "").Trim() != "Devdiscourse News Desk")
                { disclaimer = "(This story has not been edited by Devdiscourse staff and is auto-generated from a syndicated feed.)"; }
                else if ((news.OriginalSource == "Reuters" || news.OriginalSource == "PTI" || news.OriginalSource == "IANS" || news.OriginalSource == "ANI") && (news.Source ?? "").Trim() == "Devdiscourse News Desk")
                { disclaimer = "(With inputs from agencies.)"; }
                resultNewsList.Add(new ApiNewsView()
                {
                    Title = news.Title,
                    Description = DataDesc,
                    Subtitle = news.SubTitle,
                    Sector = newsSector.Title,
                    SectorSlug = newsSector.Slug,
                    ImageUrl = (news.ImageUrl ?? "").IndexOf("devdiscourse.blob.core.windows.net") != -1 ? news.ImageUrl : news.ImageUrl,
                    Country = news.Country,
                    Type = news.Type,
                    SubType = news.SubType,
                    Source = (news.Source ?? "").Trim(),
                    Themes = authorName,
                    Avatar = authorImage,
                    SourceUrl = GetCity(news.SourceUrl),
                    Tags = news.Tags,
                    Label = news.NewsLabels ?? "agency-wire",
                    Slug = news.GenerateSecondSlug(),
                    ModifiedOn = news.ModifiedOn,
                    ModifiedOnString = news.ModifiedOn.ToLocalTime().ToString("dd-MM-yyyy HH:mm"),
                    CreatedOn = news.CreatedOn,
                    PublishedOn = news.PublishedOn,
                    PublishedOnString = news.PublishedOn.ToLocalTime().ToString("dd-MM-yyyy HH:mm"),
                    NewsId = news.NewsId,
                    Id = news.Id,
                    TagArray = GetTagFormat(news.Tags),
                    ImageCopyright = news.ImageCopyright,
                    IsBlog = news.Type == "News" ? false : true,
                    Disclaimer = disclaimer
                });
            }
            return Ok(new { items = resultNewsList, hasMorePages = false });
        }
        public string GetCity(string city)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            string titlecase = !string.IsNullOrEmpty(city) ? textInfo.ToTitleCase(city.ToLower()) : "";
            if (!string.IsNullOrEmpty(titlecase))
            {
                titlecase = titlecase == "Washington Dc" ? "Washington DC" : titlecase;
            }
            return titlecase;
        }
        public DevSector GetSectorTitle(string sector)
        {
            var id = sector.Split(',').ToList().Select(int.Parse).FirstOrDefault();
            var search = db.DevSectors.Find(id);
            if (search != null)
            {
                return search;
            }
            return new DevSector();
        }
        public string GetTagFormat(string tags)
        {
            StringBuilder sb = new StringBuilder();
            tags = tags ?? "";
            var tagArray = tags.Split(',').ToList();
            foreach (var tag in tagArray.Where(a => a.Length > 2).OrderBy(a => a.Length))
            {
                sb.Append("<a href=\"/news?tag=" + tag.Trim() + "\" class=\"tag\">" + tag.Trim() + "</a>");
            }
            return sb.ToString();
        }
        public string GetSourceUrl(string source)
        {
            var sourceUrl = "";
            switch (source)
            {
                case "PTI":
                    sourceUrl = "/pti-stories";
                    break;
                case "Reuters":
                    sourceUrl = "/reuters-stories";
                    break;
                case "IANS":
                    sourceUrl = "/ians-stories";
                    break;
                case "Devdiscourse News Desk":
                    sourceUrl = "/devdiscourse-stories";
                    break;
                case "ANI":
                    sourceUrl = "/ani-stories";
                    break;
                case "PR Newswire":
                    sourceUrl = "/pr-newswire";
                    break;
                case "": break;
                case null: break;
                default:
                    sourceUrl = "/news-source/" + source.Trim();
                    break;
            }
            return sourceUrl;
        }
        public string GetAmpHtml(string Description)
        {
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
            string ampHtml = converter.ConvertFromHtml(Description).AmpHtml;
            return ampHtml;
        }
        [HttpGet]
        [Route("AmpRelatedNews/{id}/{reg}/{sector}/{__amp_source_origin?}")]
        public IActionResult AmpRelatedNews(long id, string reg, string sector, string __amp_source_origin = "")
        {
            DateTime threeDays = DateTime.Today.AddDays(-3);
            var secFirst = sector.Split(',')[0];
            if (reg == "Global Edition")
            {
                var search = db.DevNews.Where(m => m.AdminCheck == true && m.CreatedOn > threeDays && m.NewsId != id && (m.Sector.StartsWith(secFirst + ",") || m.Sector.Contains("," + secFirst + ",") || m.Sector.EndsWith("," + secFirst) || m.Sector == secFirst))
                    .OrderByDescending(o => o.CreatedOn)
                    .Select(s => new LatestNewsView
                    {
                        Title = s.Title,
                        NewId = s.NewsId,
                        Label = s.NewsLabels,
                        ImageUrl = s.ImageUrl,
                        Country = s.Country
                    }).Distinct().Take(5);
                var returnData = search.AsEnumerable()
                    .Select(a => new
                    {
                        a.Title,
                        a.Label,
                        a.ImageUrl,
                        a.Country,
                        Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString()
                    });
                return Ok(new { items = returnData, hasMorePages = search.Any() });
            }
            else
            {
                var search = db.DevNews.Where(m => m.AdminCheck == true && m.Region.Contains(reg) && m.CreatedOn > threeDays && m.NewsId != id && (m.Sector.StartsWith(secFirst + ",") || m.Sector.Contains("," + secFirst + ",") || m.Sector.EndsWith("," + secFirst) || m.Sector == secFirst)).OrderByDescending(o => o.CreatedOn).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Distinct().Take(5);
                var returnData = search.AsEnumerable()
                    .Select(a => new
                    {
                        a.Title,
                        a.Label,
                        a.ImageUrl,
                        a.Country,
                        Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString()
                    });
                return Ok(new { items = returnData, hasMorePages = search.Any() });
            }

        }
        [HttpGet]
        [Route("GetampTrends/{reg}/{__amp_source_origin?}")]
        public IActionResult GetampTrends(string reg, string __amp_source_origin)
        {
            DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
            if (reg == "Global Edition")
            {
                var search = db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.IsSponsored == false).OrderByDescending(o => o.ViewCount).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(5);
                var returnData = search.AsEnumerable().Select((a, sr) => new
                {
                    a.Title,
                    a.Label,
                    a.Country,
                    Url = "/mobilearticle/" + a.GenerateSecondSlug().ToString(),
                    defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                    ImageUrl = $"/img?imageUrl={a.ImageUrl}",
                    SrNo = sr + 1
                });
                return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
            }
            else
            {
                var search = db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.Region.Contains(reg) && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.IsSponsored == false).OrderByDescending(o => o.ViewCount).Select((s, sr) => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(5);
                var returnData = search.AsEnumerable().Select((a, sr) => new
                {
                    a.Title,
                    a.Label,
                    a.Country,
                    Url = "/mobilearticle/" + a.GenerateSecondSlug().ToString(),
                    defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                    ImageUrl = $"/img?imageUrl={a.ImageUrl}",
                    SrNo = sr + 1
                });
                return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
            }

        }
        [HttpGet]
        [Route("GetAmpVideos/{__amp_source_origin?}")]
        public IActionResult GetAmpVideos(string __amp_source_origin)
        {
            DateTime todayDate = DateTime.Today.AddDays(-30);
            var resultList = db.VideoNews.Where(a => a.AdminCheck == true).Select(a => new VideoViewModel { Id = a.Id, Title = a.Title, FileThumbUrl = a.VideoThumbUrl, CreatedOn = a.CreatedOn, Duration = a.Duration }).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
            var returnData = resultList.AsEnumerable().Select(a => new
            {
                a.Title,
                a.Country,
                Url = "/mobilevideo/" + a.GenerateSecondSlug().ToString(),
                ImageUrl = $"/img?imageUrl={a.FileThumbUrl}",
                CreatedOn = a.CreatedOn.ToString("dd MMMM yyyy"),
                Duration = a.Duration
            });
            return (IActionResult)Ok(new { items = returnData, hasMorePages = resultList.Any() });
        }
        [HttpGet]
        [Route("GetAmpVideoNews/{reg}/{__amp_source_origin?}")]
        public IActionResult GetAmpVideoNews(string reg, string __amp_source_origin = "")
        {
            if (reg == "Global Edition")
            {
                var search = db.DevNews.Where(a => a.AdminCheck == true && a.IsVideo == true && a.IsSponsored == false).OrderByDescending(o => o.CreatedOn).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(1);
                var returnData = search.AsEnumerable().Select(a => new
                {
                    a.Title,
                    a.Label,
                    a.Country,
                    Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString(),
                    defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                    ImageUrl = $"/img?imageUrl={a.ImageUrl}"
                });
                return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
            }
            else
            {
                var search = db.DevNews.Where(a => a.AdminCheck == true && a.Region.Contains(reg) && a.IsVideo == true && a.IsSponsored == false).OrderByDescending(o => o.CreatedOn).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(1);
                var returnData = search.AsEnumerable().Select(a => new
                {
                    a.Title,
                    a.Label,
                    a.Country,
                    Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString(),
                    defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                    ImageUrl = $"/img?imageUrl={a.ImageUrl}"
                });
                return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
            }

        }

        [HttpGet]
        [Route("GetAdvancedSearch/{text}/{type}/{sector}/{country}/{region}/{beforeDate?}/{afterDate?}/{page}")]
        public IQueryable<AdvancedSearchView> GetAdvancedSearch(string text, string type, string sector, string country, string region = "Global Edition", string? beforeDate = "", string? afterDate = "", int page = 1)
        {
            var skipCount = (page - 1) * 20;
            var stringSearch = text == "all" ? "" : text;
            var typeSearch = type == "all" ? "" : type;
            var countrySearch = country == "all" ? "" : country;
            var searchRegion = region == "Global Edition" ? "" : region;
            // var sectorId = Convert.ToInt32(sector);

            var newsSearch = db.DevNews
                .Where(a => a.AdminCheck == true && a.Country.Contains(countrySearch) && a.Type.Contains(typeSearch)
                  && a.Title.Contains(stringSearch) && a.Region.Contains(searchRegion))
                .Select(a => new AdvancedSearchView
                {
                    Id = a.Id,
                    Title = a.Title,
                    CreatedOn = a.CreatedOn,
                    ImageUrl = a.ImageUrl,
                    Sector = a.Sector,
                    Country = a.Country,
                    Region = a.Region,
                    Type = a.Type,
                    SubType = a.SubType,
                    NewsId = a.NewsId,
                    Label = a.NewsLabels
                });
            if (sector != "all")
            {
                newsSearch = newsSearch.Where(s => s.Sector.Contains("," + sector + ",") || s.Sector.StartsWith(sector + ",") || s.Sector.EndsWith("," + sector) || s.Sector == sector);
            }
            if (beforeDate != "null")
            {
                // DateTime filterDate = DateTime.Parse(beforeDate.AsDateTime(DateTime.UtcNow.AddDays(-60)).ToString());
                DateTime filterDate = DateTime.Parse(beforeDate);
                newsSearch = newsSearch.Where(s => s.CreatedOn < filterDate);
                //newsSearch = newsSearch.Take(50);
            }
            if (afterDate != "null")
            {
                //  DateTime filterDate2 = DateTime.Parse(afterDate.AsDateTime(DateTime.UtcNow.AddDays(-80)).ToString());
                DateTime filterDate2 = DateTime.Parse(afterDate);
                newsSearch = newsSearch.Where(s => s.CreatedOn > filterDate2);
                // newsSearch = newsSearch.Take(50);
            }
            return newsSearch.OrderByDescending(a => a.CreatedOn).Skip(skipCount).Take(20);
            //return newsSearch.Skip(skipCount).Take(50);
        }

        [HttpGet]
        [Route("GetSearchImage/{text}")]
        public IQueryable<ImageView> GetSearchImage(string text)
        {
            var search = db.ImageGalleries.Where(a => a.Title.ToUpper() == text.ToUpper() || a.Tags.ToUpper() == text.ToUpper()).OrderByDescending(a => a.CreatedOn).Select(a => new ImageView { Title = a.Title, ImageUrl = a.ImageUrl, ImageCopyright = a.ImageCopyright }).Take(1);
            return search;
        }
        [HttpGet]
        [Route("GetSubBlogs/{id}/{page}")]
        public IQueryable<LiveBlog> GetSubBlogs(long id, int page)
        {
            var skip = (page - 1) * 10;
            var search = db.LiveBlogs.Where(a => a.ParentId == id);
            return search.OrderByDescending(a => a.CreatedOn).Skip(skip).Take(10);
        }
        [HttpGet]
        [Route("GetLatestNews/{reg}")]
        public IQueryable<LatestNewsView> GetLatestNews(string reg = "Global Edition")
        {
            DateTime threemonths = DateTime.Today.AddDays(-5);
            if (reg == "Global Edition")
            {

                var result = db.DevNews
                    .Where(a => (a.IsGlobal == true || a.Region.Contains(reg)) && a.CreatedOn > threemonths && a.AdminCheck == true && a.Sector != null).OrderByDescending(a => a.ModifiedOn)
                    .Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Take(6);
                return result;
            }
            else
            {
                var result = db.DevNews
                    .Where(a => a.AdminCheck == true && a.CreatedOn > threemonths && a.Region.Contains(reg) && a.Sector != null).OrderByDescending(a => a.ModifiedOn)
                    .Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.CreatedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).AsNoTracking().Take(6);
                return result;
            }
        }
        [HttpGet]
        [Route("AppRelatedNews/{id}/{reg}/{sector}/{__amp_source_origin?}")]
        public IActionResult AppRelatedNews(long id, string sector, string __amp_source_origin)
        {
            DateTime threeDays = DateTime.Today.AddDays(-3);
            var secFirst = sector.Split(',')[0];
            var search = db.DevNews.Where(m => m.AdminCheck == true && m.CreatedOn > threeDays && m.NewsId != id && (m.Sector.StartsWith(secFirst + ",") || m.Sector.Contains("," + secFirst + ",") || m.Sector.EndsWith("," + secFirst) || m.Sector == secFirst)).OrderByDescending(o => o.CreatedOn).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Distinct().Take(5);
            var returnData = search.AsEnumerable().Select(a => new { a.Title, a.Label, ImageUrl = $"/img?imageUrl={a.ImageUrl}", a.Country, Url = "/mobilearticle/" + a.GenerateSecondSlug().ToString() });
            return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
        }
        [HttpGet]
        [HttpGet]
        [Route("AppLatestNews/{__amp_source_origin?}")]
        public IActionResult AppLatestNews(string __amp_source_origin)
        {
            DateTime threeDays = DateTime.Today.AddDays(-2);
            var resultData = (db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > threeDays).OrderByDescending(m => m.CreatedOn).Select(b => new LatestNewsView { Title = b.Title, NewId = b.NewsId, ImageUrl = b.ImageUrl, Country = b.Country })).Take(5);
            var returnData = resultData.AsEnumerable().Select(a => new { a.Title, a.Country, Url = "/mobilearticle/" + a.GenerateSecondSlug().ToString() });
            return (IActionResult)Ok(new { items = returnData, hasMorePages = resultData.Any() });
        }

        [HttpGet]
        [Route("GetAppTrends/{__amp_source_origin?}")]
        public IActionResult GetAppTrends(string __amp_source_origin)
        {
            DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
            var search = db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.IsSponsored == false).OrderByDescending(o => o.ViewCount).Select(s => new LatestNewsView { Title = s.Title, NewId = s.NewsId, Label = s.NewsLabels, ImageUrl = s.ImageUrl, Country = s.Country }).Take(5);
            var returnData = search.AsEnumerable().Select(a => new
            {
                a.Title,
                a.Label,
                a.Country,
                Url = "/mobilearticle/" + a.GenerateSecondSlug().ToString(),
                defaultImage = a.ImageUrl == "/images/sector/all_sectors.jpg" ? true : false,
                ImageUrl = $"/img?imageUrl={a.ImageUrl}"
            });
            return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
        }

        [HttpGet]
        [Route("liveblogComments/{itemId}/{parentId}/{page}")]
        public IActionResult liveblogComments(long itemId, long parentId, int page = 1)
        {
            var defaultSkip = 1;
            if (parentId != 0)
            {
                defaultSkip = 0;
            }
            var skipItem = ((page - 1) * 25) + defaultSkip;
            var comment = db.DiscourseComments
                .Where(a => a.ItemId == itemId && a.ParentId == parentId && a.IsHidden == false).OrderBy(o => o.CommentOn)
                .Select(a => new { name = a.ApplicationUser.FirstName + " " + a.ApplicationUser.LastName, commentText = a.CommentText, parentId = a.ParentId, commentId = a.CommentId, itemId = a.ItemId, isHidden = a.IsHidden, childCount = a.ChildCount, rootParentId = a.RootParentId, replyText = a.ReplyText, likeCount = a.LikeCount, dislikeCount = a.DislikeCount, endorseCount = a.EndorseCount, rejectCount = a.RejectCount }).Skip(skipItem).Take(25);
            return Ok(comment);
        }

        [HttpGet]
        [Route("GetNewsAlert")]
        public async Task<IActionResult> GetNewsAlert()
        {
            DateTime threeDays = DateTime.Today.AddDays(-2);
            var newsAlerts = await db.DevNews.Where(a => a.NewsLabels == "Newsalert" && a.CreatedOn > threeDays && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn)
                                             .Select(s => new
                                             {
                                                 s.NewsId,
                                                 s.Title,
                                                 s.CreatedOn,
                                                 Label = s.NewsLabels
                                             }).Take(10).ToListAsync();
            return Ok(newsAlerts);
        }

        [HttpGet]
        [Route("GetInfographics")]
        public IActionResult GetInfographics()
        {
            var Infographics = db.Memes.Where(a => a.IsActive == true).OrderByDescending(o => o.CreatedOn).Select(s => new { s.Title, s.ImageUrl }).Take(10);
            return (IActionResult)Ok(Infographics);
        }

        [Route("GetOpinion/{reg}")]
        [OutputCache(Duration = 300)]
        public IActionResult GetOpinion(string reg)
        {
            reg = reg == "Global Edition" ? "" : reg;
            DateTime thirtyDays = DateTime.Today.AddDays(-30);
            if (reg == "")
            {
                var search = (from m in db.Infocus
                              where m.Edition == "Universal Edition" && m.ItemType == "Blog"
                              join a in db.DevNews on m.NewsId equals a.NewsId
                              where a.AdminCheck == true
                              orderby a.PublishedOn descending
                              select new
                              {
                                  a.Id,
                                  a.Title,
                                  a.CreatedOn,
                                  ImageUrl = a.ApplicationUsers.ProfilePic,
                                  AuthorImage = a.Themes,
                                  a.Description,
                                  Name = a.Author,
                                  a.NewsId,
                                  Label = a.NewsLabels
                              }).Take(10);

                if (!search.Any())
                {
                    search = db.DevNews.Where(a => a.Type == "Blog" && a.CreatedOn > thirtyDays && a.AdminCheck == true).OrderByDescending(o => o.PublishedOn).Select(a => new { a.Id, a.Title, a.CreatedOn, ImageUrl = a.ApplicationUsers.ProfilePic, AuthorImage = a.Themes, a.Description, Name = a.Author, a.NewsId, Label = a.NewsLabels }).AsNoTracking().Take(10);
                }
                return (IActionResult)Ok(search);
            }
            else
            {
                var search = (from m in db.Infocus
                              where m.Edition == reg && m.ItemType == "Blog"
                              join a in db.DevNews on m.NewsId equals a.NewsId
                              where a.AdminCheck == true
                              orderby a.PublishedOn descending
                              select new
                              {
                                  a.Id,
                                  a.Title,
                                  a.CreatedOn,
                                  ImageUrl = a.ApplicationUsers.ProfilePic,
                                  AuthorImage = a.Themes,
                                  a.Description,
                                  Name = a.Author,
                                  a.NewsId,
                                  Label = a.NewsLabels
                              }).Take(10);
                if (!search.Any())
                {
                    var regionNewsIds = db.RegionNewsRankings
                        .AsNoTracking()
                        .Where(r => r.Region.Title == reg)
                        .Select(r => r.NewsId);
                    search = db.DevNews.Where(a => a.Type == "Blog" && regionNewsIds.Contains(a.Id) && a.CreatedOn > thirtyDays && a.AdminCheck == true).OrderByDescending(o => o.PublishedOn).Select(a => new { a.Id, a.Title, a.CreatedOn, ImageUrl = a.ApplicationUsers.ProfilePic, AuthorImage = a.Themes, a.Description, Name = a.Author, a.NewsId, Label = a.NewsLabels }).AsNoTracking().Take(10);
                }
                return (IActionResult)Ok(search);
            }
        }
        [OutputCache(Duration = 120, VaryByRouteValueNames = new[] { "reg", "edition" })]
        [Route("GetEditionNews/{reg}/{edition}")]
        public async Task<IActionResult> GetEditionNews(string reg = "South Asia", string edition = "")
        {
            // Redis cache – edition sidebar, 5 min TTL
            string cacheKey = $"api:editionNews:{reg}:{edition}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return Ok(JsonSerializer.Deserialize<object>(cached));

            DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime weekend = todayDate.AddDays(-3).AddTicks(1);
            var regionNewsIds = db.RegionNewsRankings
                .AsNoTracking()
                .Where(r => r.Region.Title == reg)
                .Select(r => r.NewsId);
            var result = await db.DevNews
                .AsNoTracking()
                .Where(a => a.AdminCheck == true && a.CreatedOn > weekend && regionNewsIds.Contains(a.Id))
                .Select(a => new
                {
                    Id = a.Id,
                    Title = a.Title,
                    CreatedOn = a.CreatedOn,
                    ImageUrl = a.ImageUrl,
                    NewId = a.NewsId,
                    Label = a.NewsLabels,
                    Country = a.Country,
                    a.Region,
                    a.Type,
                    a.SubType
                }).OrderByDescending(m => m.CreatedOn).Take(5).ToListAsync();

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            var responseData = new { news = result, edition };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(responseData), cacheOptions);
            return Ok(responseData);
        }

        [OutputCache(Duration = 120, VaryByRouteValueNames = new[] { "reg", "id" })]
        [Route("GetAnalysis/{reg}/{id?}")]
        public async Task<IActionResult> GetAnalysis(long? id, string reg)
        {
            // Redis cache – analysis/opinion rarely changes, cache 10 minutes
            string cacheKey = $"api:analysis:{reg}:{id ?? 0}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return Ok(System.Text.Json.JsonSerializer.Deserialize<object>(cached));

            DateTime thirtyDays = DateTime.Today.AddDays(-90);
            object result;
            if (reg == "Global Edition")
            {
                result = await db.DevNews.Where(a => a.Type == "Blog" && a.CreatedOn > thirtyDays && a.NewsId != id && a.AdminCheck == true).OrderByDescending(o => o.ModifiedOn).Select(a => new { a.Id, a.Title, a.CreatedOn, ImageUrl = a.ApplicationUsers.ProfilePic, Image = a.ImageUrl, a.Description, Name = a.Author, a.NewsId, Label = a.NewsLabels }).AsNoTracking().Take(5).ToListAsync();
            }
            else
            {
                var regionNewsIds = db.RegionNewsRankings
                    .AsNoTracking()
                    .Where(r => r.Region.Title == reg)
                    .Select(r => r.NewsId);
                result = await db.DevNews
                    .Where(a => a.Type == "Blog" && regionNewsIds.Contains(a.Id) && a.CreatedOn > thirtyDays && a.NewsId != id && a.AdminCheck == true)
                    .OrderByDescending(o => o.ModifiedOn)
                    .Select(a => new { a.Id, a.Title, a.CreatedOn, ImageUrl = a.ApplicationUsers.ProfilePic, Image = a.ImageUrl, a.Description, Name = a.Author, a.NewsId, Label = a.NewsLabels })
                    .AsNoTracking()
                    .Take(5)
                    .ToListAsync();
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(result), cacheOptions);
            return Ok(result);
        }
        [Route("GetInterview/{reg}")]
        public IActionResult GetInterview(string reg)
        {
            DateTime thirtyDays = DateTime.Today.AddDays(-30);
            if (reg == "Global Edition")
            {
                var result = (from m in db.Infocus
                              where m.Edition == "Universal Edition" && m.ItemType == "Interview"
                              join a in db.DevNews on m.NewsId equals a.NewsId
                              where a.AdminCheck == true
                              select new
                              {
                                  a.Title,
                                  a.CreatedOn,
                                  Image = a.ImageUrl,
                                  a.NewsId,
                                  Label = a.NewsLabels,
                                  a.Country,
                                  a.ModifiedOn,
                                  m.SrNo
                              }).OrderBy(a => a.SrNo).AsNoTracking().Take(10);
                return (IActionResult)Ok(result);
            }
            else
            {
                var result = (from m in db.Infocus
                              where m.Edition == reg && m.ItemType == "Interview"
                              join a in db.DevNews on m.NewsId equals a.NewsId
                              where a.AdminCheck == true
                              select new
                              {
                                  a.Title,
                                  a.CreatedOn,
                                  Image = a.ImageUrl,
                                  a.NewsId,
                                  Label = a.NewsLabels,
                                  a.Country,
                                  a.ModifiedOn,
                                  m.SrNo
                              }).OrderBy(a => a.SrNo).AsNoTracking().Take(10);
                return (IActionResult)Ok(result);
            }
        }

        [Route("GetAmpEditionNews/{reg}/{__amp_source_origin?}")]
        public IActionResult GetAmpEditionNews(string reg = "South Asia", string __amp_source_origin = "")
        {
            DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime weekend = todayDate.AddDays(-3).AddTicks(1);
            var result = db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > weekend && a.Region.Contains(reg)).OrderByDescending(m => m.CreatedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl, NewId = a.NewsId, Label = a.NewsLabels, Country = a.Country }).Take(5);
            var returnData = result.AsEnumerable().Select(a => new { a.Title, a.Label, ImageUrl = $"/img?imageUrl={a.ImageUrl}", a.Country, Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString() });
            return (IActionResult)Ok(new { items = returnData, hasMorePages = result.Any() });
        }
        [HttpGet]
        [Route("GetAmpOpinion/{reg}/{__amp_source_origin?}")]
        [OutputCache(Duration = 300)]
        public IActionResult GetAmpOpinion(string reg, string __amp_source_origin = "")
        {
            DateTime thirtyDays = DateTime.Today.AddDays(-90);
            reg = reg == "Global Edition" ? "" : reg;
            if (reg == "")
            {
                var search = (from a in db.DevNews
                              where a.AdminCheck == true && a.Type == "Blog" && a.CreatedOn > thirtyDays
                              orderby a.CreatedOn descending
                              select new LatestNewsView
                              {
                                  Id = a.Id,
                                  Title = a.Title,
                                  CreatedOn = a.PublishedOn,
                                  ImageUrl = a.ImageUrl,
                                  Sector = a.Themes,
                                  Country = a.Author,
                                  NewId = a.NewsId,
                                  Label = a.NewsLabels
                              }).Take(5);
                var returnData = search.AsEnumerable().Select(a => new
                {
                    a.Title,
                    a.Label,
                    Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString(),
                    AuthorImage = $"/img?imageUrl={a.Sector}",
                    ImageUrl = $"/img?imageUrl={a.ImageUrl}",
                    Author = a.Country
                });
                return Ok(new { items = returnData, hasMorePages = search.Any() });
            }
            else
            {
                var search = (from a in db.DevNews
                              where a.AdminCheck == true && a.Type == "Blog" && a.Region.Contains(reg) && a.CreatedOn > thirtyDays
                              orderby a.CreatedOn descending
                              select new LatestNewsView
                              {
                                  Id = a.Id,
                                  Title = a.Title,
                                  CreatedOn = a.PublishedOn,
                                  ImageUrl = a.ImageUrl,
                                  Sector = a.Themes,
                                  Country = a.Author,
                                  NewId = a.NewsId,
                                  Label = a.NewsLabels
                              }).Take(5);
                var returnData = search.AsEnumerable().Select(a => new
                {
                    a.Title,
                    a.Label,
                    Url = "/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString(),
                    AuthorImage = $"/img?imageUrl={a.Sector}",
                    ImageUrl = $"/img?imageUrl={a.ImageUrl}",
                    Author = a.Country
                });
                return (IActionResult)Ok(new { items = returnData, hasMorePages = search.Any() });
            }

        }
        [HttpGet]
        [Route("AmpInfocusMore")]
        public IActionResult GetAmpInfocusMore(string __amp_source_origin = "")
        {
            var halfDay = DateTime.UtcNow.AddHours(-12);
            var infocus = db.RegionNewsRankings.Where(a => a.DevNews.AdminCheck == true && a.DevNews.CreatedOn > halfDay && a.Region.Title == "Global Edition" && !a.DevNews.Title.Contains("News Summary") && !a.DevNews.Title.Contains("Highlights") && a.DevNews.NewsLabels != "Newsalert" && a.DevNews.Sector != "14" && a.DevNews.Sector != "18" && a.DevNews.Sector != "19" && a.DevNews.Sector != "9").Select(s => new LatestNewsView
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
            }).OrderByDescending(a => a.CreatedOn).AsNoTracking().Take(65).ToList();
            var search = infocus.OrderByDescending(o => o.Ranking).Skip(10).Take(12).ToList();
            var returnData = search.AsEnumerable().Select(a => new
            {
                type = "small",
                title = a.Title,
                url = "https://www.devdiscourse.com/article/" + (a.Label ?? "agency-wire") + "/" + a.GenerateSecondSlug().ToString() + "?amp",
                image = $"/img?imageUrl={a.ImageUrl}&width=240&height=240&mode=crop",
            });
            return Ok(new
            {
                bookendVersion = "v1.0",
                shareProviders = new string[] { "facebook", "twitter", "email" },
                components = returnData
            });
        }
        [HttpGet]
        [Route("GetBudgetNews/{page}")]
        public IQueryable<LatestNewsView> GetBudgetNews(int page = 1)
        {
            var skipCount = (page - 1) * 20;
            var result = db.DevNews.Where(a => a.AdminCheck == true && (a.Category.Contains(",33,") || a.Category.StartsWith(",33") || a.Category.EndsWith("33,") || a.Category.Equals("33"))).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Title = a.Title, CreatedOn = a.ModifiedOn, Country = a.Country, ImageUrl = a.ImageUrl, NewId = a.NewsId, Sector = a.Type, SubType = a.SubType }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
            return result;
        }
        [HttpGet]
        [Route("getNewsAnalysis")]
        public IActionResult getNewsAnalysis()
        {
            List<string> editionList = new List<string>() { "Central Africa", "East Africa", "Southern Africa", "West Africa", "East and South East Asia", "Europe and Central Asia", "Latin America and Caribbean", "Middle East and North Africa", "North America", "South Asia", "Pacific" };
            DateTime threeDays = DateTime.Today.AddDays(-3);
            var news = db.DevNews.Where(a => a.CreatedOn > threeDays).Select(s => new { s.NewsId, s.Sector, s.Region }).ToList();
            var arraydata = news.Select(a => new { a.NewsId, sectorList = a.Sector.Split(','), editions = a.Region.Split(',') });
            var sector = db.DevSectors.Where(a => a.Id != 8 && a.Id != 5 && a.Id != 16).Select(s => new
            {
                s.Id,
                s.Title
            });
            List<NewsAnalysis> NewsAnalysisList = new List<NewsAnalysis>();
            foreach (var ed in editionList)
            {
                foreach (var sec in sector)
                {
                    var newsCount = arraydata.Count(a => a.sectorList.All(s => s == sec.Id.ToString()) && a.editions.Any(e => e == ed));
                    NewsAnalysisList.Add(new NewsAnalysis
                    {
                        Edition = ed,
                        Sector = sec.Title,
                        NewsCount = newsCount
                    });
                }
            }
            var results = NewsAnalysisList.GroupBy(p => p.Edition).OrderBy(o => o.Key);
            return (IActionResult)Ok(new { total = news.Count(), results });
        }
        [HttpGet]
        [Route("GetPreviousNews/{id}/{label}/{reg}/{__amp_source_origin?}")]
        [HttpGet]
        public IActionResult GetPreviousNews(long id, string label, string reg, string __amp_source_origin)
        {
            var search = db.DevNews.FirstOrDefault(a => a.NewsId == id);
            DateTime tenDays = search.CreatedOn.AddDays(-5);
            List<SearchView> newsList = new List<SearchView>();
            List<ApiNewsView> resultNewsList = new List<ApiNewsView>();
            if (reg == "Global Edition")
            {
                if (label != "agency-wire")
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.NewsLabels == label && a.AdminCheck == true).Select(a => new SearchView { NewsId = a.NewsId, Id = a.Id, Title = a.Title, Label = a.NewsLabels, CreatedOn = a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(1).ToList();
                }
                else
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.NewsLabels == null && a.AdminCheck == true).Select(a => new SearchView { NewsId = a.NewsId, Id = a.Id, Title = a.Title, Label = a.NewsLabels, CreatedOn = a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(1).ToList();
                }
            }
            else
            {
                if (label != "agency-wire")
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == label).Select(a => new SearchView { NewsId = a.NewsId, Id = a.Id, Title = a.Title, Label = a.NewsLabels, CreatedOn = a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(1).ToList();
                }
                else
                {
                    newsList = db.DevNews.Where(a => a.NewsId < id && a.CreatedOn > tenDays && a.Region.Contains(reg) && a.NewsLabels == null).Select(a => new SearchView { NewsId = a.NewsId, Id = a.Id, Title = a.Title, Label = a.NewsLabels, CreatedOn = a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(1).ToList();
                }
            }
            foreach (var news in newsList)
            {
                resultNewsList.Add(new ApiNewsView()
                {
                    Title = news.Title,
                    Label = news.Label,
                    Slug = news.GenerateSecondSlug(),
                    NewsId = news.NewsId,
                    Id = news.Id,
                });
            }
            return (IActionResult)Ok(new { items = resultNewsList });
        }
        [HttpGet]
        [Route("CategoryNews/{Id}/{page}")]
        public IQueryable<LatestNewsView> GetCategoryNews(int Id, int page = 1)
        {
            var skipCount = (page - 1) * 20;
            var result = db.DevNews.Where(a => a.AdminCheck == true && (a.Category == a.Id.ToString())).OrderByDescending(a => a.CreatedOn).Select(a => new LatestNewsView { Title = a.Title, CreatedOn = a.ModifiedOn, Country = a.Country, ImageUrl = a.ImageUrl, NewId = a.NewsId, Sector = a.Type, SubType = a.SubType }).OrderByDescending(m => m.CreatedOn).Skip(skipCount).Take(20);
            return result;
        }
        [HttpGet]
        [Route("GetAllInfocus")]
        public IActionResult GetAllInfocus()
        {
            var search = (from m in db.Infocus
                          join s in db.DevNews on m.NewsId equals s.NewsId
                          where s.AdminCheck == true && m.ItemType == "News"
                          orderby m.SrNo
                          select new
                          {
                              newsId = "\"" + s.NewsId + "\"",
                              title = s.Title
                          }).AsNoTracking().Distinct().OrderByDescending(a => Guid.NewGuid()).Skip(1).Take(1);
            return (IActionResult)Ok(search);
        }
        [HttpGet]
        [Route("GetAllData")]
        public IActionResult GetAllData()
        {
            var search = (from s in db.DevNews
                          where s.Sector != "0" && !s.Sector.Contains(",") && !string.IsNullOrEmpty(s.WorkStage)
                          select new
                          {
                              label = s.Sector,
                              title = s.Title
                          }).Take(50000);
            return (IActionResult)Ok(search);
        }
        [Route("GetAllSector")]
        public IActionResult GetAllSector()
        {
            var search = from s in db.DevSectors
                         select new
                         {
                             Id = s.Id,
                             title = s.Title
                         };
            return (IActionResult)Ok(search);
        }
        [HttpGet]
        [Route("GetSingleSector/{id}")]
        public IActionResult GetAllSector(int id)
        {
            var search = (from s in db.DevNews
                          where s.Sector.Contains(",") && !string.IsNullOrEmpty(s.WorkStage)
                          orderby s.CreatedOn descending
                          select new
                          {
                              label = s.Sector,
                              description = s.Description
                          }).Take(5000);
            var returnData = search.AsEnumerable().Select(a => new
            {
                label = a.label,
                description = CleanDesc(a.description)
            });
            return (IActionResult)Ok(returnData);
        }
        public string CleanDesc(string htmltext)
        {
            htmltext = htmltext.Replace("&nbsp;", " ");
            htmltext = htmltext.Replace("&rsquo;", "'");
            htmltext = htmltext.Replace("&lsquo;", "'");
            htmltext = htmltext.Replace("&#39;", "'");
            htmltext = htmltext.Replace("&rdquo;", "\"");
            htmltext = htmltext.Replace("&ldquo;", "\"");
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
            const string stripFormatting = @"<[^>]*(>|$)";
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var text = (htmltext ?? "").Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"\r\n?|\n", " ");
            text = System.Net.WebUtility.HtmlDecode(text);
            text = tagWhiteSpaceRegex.Replace(text, "><");
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            text = stripFormattingRegex.Replace(text, string.Empty);
            text = Regex.Replace(text, @"[^,.A-Za-z0-9\s-]", "");
            return text;

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // ── AI Summary endpoint ──
        [HttpGet]
        [Route("GetAiSummary/{sector}/{reg}")]
        public async Task<IActionResult> GetAiSummary(string sector, string reg = "Global Edition")
        {
            try
            {
                // Check cache first (10-minute TTL)
                string cacheKey = $"api:aiSummary:{sector}:{reg}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached != null)
                    return Ok(new { success = true, summary = cached });

                DateTime thirtyDays = DateTime.UtcNow.AddDays(-30);

                // Get top news titles for this sector
                var topNews = await db.RegionNewsRankings
                    .AsNoTracking()
                    .Where(a => a.DevNews.AdminCheck == true
                        && a.DevNews.Sector == sector
                        && a.Region.Title == reg
                        && a.DevNews.IsSponsored == false
                        && a.DevNews.CreatedOn > thirtyDays)
                    .OrderByDescending(a => a.DevNews.CreatedOn)
                    .ThenByDescending(s => s.Ranking)
                    .Take(10)
                    .Select(a => a.DevNews.Title)
                    .ToListAsync();

                if (topNews.Count == 0)
                    return Ok(new { success = false, summary = "No recent news available for this sector." });

                var titlesText = string.Join("\n", topNews.Select((t, i) => $"{i + 1}. {t}"));

                var prompt = $"You are a concise news analyst for Devdiscourse. Based on these recent headlines, write exactly 4 short bullet-point summaries (one sentence each) capturing the key developments. Return ONLY a JSON array of 4 strings, no extra text.\n\nHeadlines:\n{titlesText}";

                string ApiKey = $"Bearer {_config["OpenAI:ApiKey"]}";
                var messages = new[] {
                    new { role = "system", content = "You are a helpful news summarization assistant. Always respond with valid JSON only." },
                    new { role = "user", content = prompt }
                };
                var payload = new { model = "gpt-4o-mini", messages, temperature = 0.5, max_tokens = 300 };
                string jsonString = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", ApiKey);
                client.Timeout = TimeSpan.FromSeconds(15);

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Fallback: generate a simple extractive summary from titles
                    var fallback = GenerateFallbackSummary(topNews);
                    return Ok(new { success = true, summary = fallback, fallback = true });
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                var aiText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "[]";

                // Clean markdown fences if present
                aiText = aiText.Trim();
                if (aiText.StartsWith("```json")) aiText = aiText[7..];
                if (aiText.StartsWith("```")) aiText = aiText[3..];
                if (aiText.EndsWith("```")) aiText = aiText[..^3];
                aiText = aiText.Trim();

                // Cache for 10 minutes
                await _cache.SetStringAsync(cacheKey, aiText, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return Ok(new { success = true, summary = aiText });
            }
            catch (Exception)
            {
                // Fallback: use titles as bullet points
                try
                {
                    DateTime thirtyDays = DateTime.UtcNow.AddDays(-30);
                    var topNews = await db.RegionNewsRankings
                        .AsNoTracking()
                        .Where(a => a.DevNews.AdminCheck == true
                            && a.DevNews.Sector == sector
                            && a.Region.Title == reg
                            && a.DevNews.IsSponsored == false
                            && a.DevNews.CreatedOn > thirtyDays)
                        .OrderByDescending(a => a.DevNews.CreatedOn)
                        .ThenByDescending(s => s.Ranking)
                        .Take(4)
                        .Select(a => a.DevNews.Title)
                        .ToListAsync();

                    var fallback = GenerateFallbackSummary(topNews);
                    return Ok(new { success = true, summary = fallback, fallback = true });
                }
                catch
                {
                    return Ok(new { success = false, summary = "Unable to generate summary at this time." });
                }
            }
        }

        [HttpGet]
        [Route("GetArticleAiSummary/{newsId:long}")]
        public async Task<IActionResult> GetArticleAiSummary(long newsId, bool refresh = false)
        {
            try
            {
                // Check cache first (7-day TTL — article content rarely changes)
                string cacheKey = $"api:articleAiSummary:{newsId}";
                if (!refresh)
                {
                    var cached = await _cache.GetStringAsync(cacheKey);
                    if (cached != null)
                        return Ok(new { success = true, summary = cached, cached = true });
                }

                var article = await db.DevNews
                    .AsNoTracking()
                    .Where(a => a.NewsId == newsId && a.AdminCheck == true)
                    .Select(a => new { a.Title, a.Description, a.SubTitle, a.Sector, a.Tags })
                    .FirstOrDefaultAsync();

                if (article == null)
                    return Ok(new { success = false, summary = "Article not found." });

                // Strip HTML from description to get plain text
                var plainText = Regex.Replace(article.Description ?? "", "<[^>]*>", " ");
                plainText = System.Net.WebUtility.HtmlDecode(plainText);
                plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

                // Limit to 1200 chars to minimize token usage
                if (plainText.Length > 1200)
                    plainText = plainText[..1200];

                if (string.IsNullOrWhiteSpace(plainText))
                    return Ok(new { success = false, summary = "Article content not available." });

                var prompt = $"Summarize this news in exactly 4 bullet points (1 sentence each). Return ONLY a JSON array of 4 strings.\n\nTitle: {article.Title}\n\n{plainText}";

                string apiKey = $"Bearer {_config["OpenAI:ApiKey"]}";
                var messages = new[] {
                    new { role = "system", content = "Respond with valid JSON array only." },
                    new { role = "user", content = prompt }
                };
                var payload = new { model = "gpt-4o-mini", messages, temperature = 0.0, max_tokens = 200 };
                string jsonString = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", apiKey);
                client.Timeout = TimeSpan.FromSeconds(15);

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Fallback: extract first sentences (no OpenAI cost)
                    var fallback = GenerateArticleFallbackSummary(article.Title, plainText);
                    // Cache fallback for 1 day so we don't keep retrying
                    await _cache.SetStringAsync(cacheKey, fallback, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
                    return Ok(new { success = true, summary = fallback, fallback = true });
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                var aiText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "[]";

                // Clean markdown fences if present
                aiText = aiText.Trim();
                if (aiText.StartsWith("```json")) aiText = aiText[7..];
                if (aiText.StartsWith("```")) aiText = aiText[3..];
                if (aiText.EndsWith("```")) aiText = aiText[..^3];
                aiText = aiText.Trim();

                // Cache for 7 days — article content is static
                await _cache.SetStringAsync(cacheKey, aiText, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });

                return Ok(new { success = true, summary = aiText });
            }
            catch (Exception)
            {
                return Ok(new { success = false, summary = "Unable to generate summary at this time." });
            }
        }

        private static string GenerateArticleFallbackSummary(string title, string plainText)
        {
            // Extract first few sentences as fallback
            var sentences = Regex.Split(plainText, @"(?<=[.!?])\s+")
                .Where(s => s.Length > 20)
                .Take(4)
                .Select(s => s.Trim())
                .ToList();

            if (sentences.Count == 0)
                sentences.Add(title);

            return JsonSerializer.Serialize(sentences);
        }

        private static string GenerateFallbackSummary(List<string> titles)
        {
            var bullets = titles.Take(4).Select(t =>
            {
                // Trim to one sentence
                var idx = t.IndexOfAny(new[] { '.', '!', '?' });
                return idx > 0 && idx < t.Length - 1 ? t[..(idx + 1)] : t;
            });
            return JsonSerializer.Serialize(bullets);
        }
    }
}
