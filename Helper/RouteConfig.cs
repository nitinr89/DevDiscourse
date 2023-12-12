using System.Web.Mvc;

namespace Devdiscourse.Helper
{
    public static class RouteConfig
    {
        public static void ConfigureRoutes(IEndpointRouteBuilder endpoints)
        {

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
              name: "DefaulApi",
              pattern: "api/controller/{id?}");

            endpoints.MapControllerRoute(
                       name: "ArticleDetailswithprefix",
                       pattern: "ArticleDetailswithprefix/{Index}/{id?}",
                       defaults: new { controller = "Article", action = "Index", prefix = UrlParameter.Optional, reg = UrlParameter.Optional });

            endpoints.MapControllerRoute(
                          name: "NewsSector",
                          pattern: "news/{sector}",
                          defaults: new { controller = "Search", action = "Index", sector = UrlParameter.Optional }
                        );

            endpoints.MapControllerRoute(
                name: "NewsAnalysis",
                pattern: "news-analysis/{type}",
                defaults: new { controller = "Agencywire", action = "NewsAnalysis", type = UrlParameter.Optional }
            );

            endpoints.MapControllerRoute(
                name: "LivediscourseHome",
                pattern: "live-discourse",
                  defaults: new { controller = "Livediscourse", action = "Home" }
            );

            endpoints.MapControllerRoute(
                name: "LivediscourseArticle",
                pattern: "live-discourse",
                  defaults: new { controller = "Livediscourse", action = "Home" }
            );

            endpoints.MapControllerRoute(
                name: "Blogs",
                pattern: "blogs/{type}",
                  defaults: new { controller = "Home", action = "DevBlogs", type = UrlParameter.Optional }
            );
            endpoints.MapControllerRoute(
               name: "SouthAsiaEdition",
               pattern: "south-asia",
               defaults: new { controller = "Home", action = "SouthAsia" }
           );

            endpoints.MapControllerRoute(
                      name: "EastAndSouthEastAsiaEdition",
                      pattern: "south-east-asia",
                      defaults: new { controller = "Home", action = "EastAndSouthEastAsia" }
                      );

            endpoints.MapControllerRoute(
                        name: "EuropeAndCentralAsiaEdition",
                        pattern: "europe-central-asia",
                        defaults: new { controller = "Home", action = "EuropeAndCentralAsia" }
                        );
            endpoints.MapControllerRoute(
                          name: "CentralAfricaEdition",
                          pattern: "central-africa",
                          defaults: new { controller = "Home", action = "CentralAfrica" }
                      );
            endpoints.MapControllerRoute(
                          name: "EastAfricaEdition",
                          pattern: "east-africa",
                          defaults: new { controller = "Home", action = "EastAfrica" }
                      );
            endpoints.MapControllerRoute(
                           name: "SouthernAfricaEdition",
                           pattern: "southern-africa",
                           defaults: new { controller = "Home", action = "SouthernAfrica" }
                       );
            endpoints.MapControllerRoute(
                            name: "WestAfricaEdition",
                            pattern: "west-africa",
                            defaults: new { controller = "Home", action = "WestAfrica" }
                        );
            endpoints.MapControllerRoute(
                        name: "MiddleEastAndNorthAfricaEdition",
                        pattern: "middle-east-north-africa",
                        defaults: new { controller = "Home", action = "MiddleEastAndNorthAfrica" }
                        );
            endpoints.MapControllerRoute(
                        name: "LatinAmericaAndCaribbeanEdition",
                        pattern: "latin-america",
                        defaults: new { controller = "Home", action = "LatinAmericaAndCaribbean" }
                        );

            endpoints.MapControllerRoute(
                           name: "NorthAmericaEdition",
                           pattern: "north-america",
                           defaults: new { controller = "Home", action = "NorthAmerica" }
                       );

            endpoints.MapControllerRoute(
                          name: "NewsSector",
                          pattern: "news/{sector}",
                          defaults: new { controller = "Search", action = "Index", sector = UrlParameter.Optional }
                        );
            endpoints.MapControllerRoute(
                     name: "Contribute",
                     pattern: "Writeforus",
                     defaults: new { controller = "Home", action = "Contribute" }
                     );
            endpoints.MapControllerRoute(
                          name: "NewsLabel",
                          pattern: "stories/{label}",
                          defaults: new { controller = "Home", action = "Search", label = UrlParameter.Optional }
                        );
            endpoints.MapControllerRoute(
                        name: "PressRelease",
                        pattern: "press-release",
                        defaults: new { controller = "NewsWire", action = "Index" }
                        );

            endpoints.MapControllerRoute(
                       name: "ArticleDetailswithprefix",
                       pattern: "ArticleDetailswithprefix/{Index}/{id?}",
                       defaults: new { controller = "Article", action = "Index", prefix = UrlParameter.Optional, reg = UrlParameter.Optional });

            endpoints.MapControllerRoute(
                           name: "AboutUs",
                           pattern: "aboutus/{id}",
                           defaults: new { controller = "About", action = "Index", id = UrlParameter.Optional }
                       );
            endpoints.MapControllerRoute(
                         name: "Advertisement",
                         pattern: "advertisewithus",
                         defaults: new { controller = "Advertisements", action = "AdvertiseWithUs" }
                       );
            endpoints.MapControllerRoute(
                          name: "Career_Create",
                          pattern: "career",
                          defaults: new { controller = "Careers", action = "Create" }
                        );
            endpoints.MapControllerRoute(
                         name: "KnowledgePartnership",
                         pattern: "partners/knowledge-partners",
                         defaults: new { controller = "MediaPartnership", action = "Index" }
                       );

            endpoints.MapControllerRoute(
                      name: "MediaPartnership",
                      pattern: "partners/media-partners",
                      defaults: new { controller = "MediaPartnership", action = "MediaPartners" }
                    );

            endpoints.MapControllerRoute(
                        name: "MediaPartnershipLink",
                        pattern: "partnership/{action}/{type}",
                        defaults: new { controller = "MediaPartnership", action = "KnowledgePartners", type = UrlParameter.Optional }
               );
         
         endpoints.MapControllerRoute(
                 name: "PacificEdition",
                 pattern: "pacific",
                 defaults: new { controller = "Home", action = "Pacific" }
           );
        }
    }
}
