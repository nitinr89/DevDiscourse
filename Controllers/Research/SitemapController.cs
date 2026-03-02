using Devdiscourse.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.Research
{
    public class SitemapController : Controller
    {
        private readonly SitemapService _sitemapService;

        public SitemapController(SitemapService sitemapService)
        {
            _sitemapService = sitemapService;
        }

        [HttpGet("sitemap.xml")]
        public IActionResult Sitemap()
        {
            var urls = new List<SitemapUrl>
        {
            new () { Loc = "https://www.devdiscourse.com", ChangeFreq = "hourly", Priority = "1.00" },
            new () { Loc = "https://www.devdiscourse.com/pacific", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/south-asia", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/europe-central-asia", ChangeFreq = "hourly", Priority = "1.00" },
            new () { Loc = "https://www.devdiscourse.com/central-africa", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/east-africa", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/southern-africa", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/middle-east-north-africa", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/north-america", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/latin-america", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/career", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/advertisewithus", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/partners", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news-analysis", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/Research", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/live-discourse", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/Writeforus", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/press-release", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/agro-forestry", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/art-culture", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/economy-business", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/education", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/energy-extractives", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/politics", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/law-governance", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/health", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/science-environment", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/socialgender", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/sports", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/transport", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/urban-development", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news/wash", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/aboutus", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/news-analysis", ChangeFreq = "hourly", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/Home/Disclaimer", LastMod = "2019-04-23T05:18:57+00:00", Priority = "0.80" },
            new () { Loc = "https://www.devdiscourse.com/Home/PrivacyPolicy", LastMod = "2019-04-23T05:18:58+00:00", Priority = "0.80" },
        };

            var sitemap = _sitemapService.GenerateSitemap(urls);
            return Content(sitemap, "application/xml");
        }
    }

}
