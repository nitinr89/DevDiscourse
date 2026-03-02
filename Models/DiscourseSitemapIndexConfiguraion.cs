//using SimpleMvcSitemap;
//using SimpleMvcSitemap.News;
//using System;
//using System.Linq;
//using System.Web.Mvc;

//namespace Devdiscourse.Models
//{
//    public class DiscourseSitemapIndexConfiguraion : SitemapIndexConfiguration<SitemapVM>
//    {
//        private readonly UrlHelper urlHelper;
//        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
//        public DiscourseSitemapIndexConfiguraion(IQueryable<SitemapVM> dataSource, int? currentPage, UrlHelper urlHelper)
//            : base(dataSource, currentPage)
//        {
//            this.urlHelper = urlHelper;
//        }

//        public override SitemapIndexNode CreateSitemapIndexNode(int currentPage)
//        {
//            return new SitemapIndexNode(urlHelper.RouteUrl("DiscourseSitemap", new { currentPage }));
//        }

//        public override SitemapNode CreateNode(SitemapVM source)
//        {
//            return new SitemapNode(urlHelper.RouteUrl("LivediscourseArticle", new { id = source.GenerateSecondSlug() }))
//            {
//                News = new SitemapNews(newsPublication: new NewsPublication(name: "Devdiscourse", language: "en"),
//                           publicationDate: DateTime.Parse(source.CreatedOn.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
//                           title: source.Title)
//            };
//        }
//    }
//}