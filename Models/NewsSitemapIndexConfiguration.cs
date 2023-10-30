//using SimpleMvcSitemap;
//using SimpleMvcSitemap.News;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Devdiscourse.Models
//{
//    public class NewsSitemapIndexConfiguration : SitemapIndexConfiguration<SitemapVM>
//    {
//        private readonly UrlHelper urlHelper;
//        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
//        public NewsSitemapIndexConfiguration(IQueryable<SitemapVM> dataSource, int? currentPage, UrlHelper urlHelper)
//            : base(dataSource, currentPage)
//        {
//            this.urlHelper = urlHelper;
//        }

//        public override SitemapIndexNode CreateSitemapIndexNode(int currentPage)
//        {
//            return new SitemapIndexNode(urlHelper.RouteUrl("NewsSitemap", new { currentPage }));
//        }

//        public override SitemapNode CreateNode(SitemapVM source)
//        {            
//            return new SitemapNode(urlHelper.RouteUrl("ArticleDetailswithprefix", new { prefix= source.Label??"agency-wire", id = source.GenerateSecondSlug() }))
//            {             
//            News = new SitemapNews(newsPublication: new NewsPublication(name: "Devdiscourse", language: "en"),
//                           publicationDate: DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(source.CreatedOn,INDIAN_ZONE).ToString("yyyy-MM-ddTHH:mm:sszzzzz")),
//                           title: source.Title)
//            };
//        }
//    }
//}