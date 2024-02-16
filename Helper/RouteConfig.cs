using DocumentFormat.OpenXml.Wordprocessing;

namespace Devdiscourse.Helper
{
    public static class RouteConfig
    {
        public static void ConfigureRoutes(IEndpointRouteBuilder routes)
        {
            routes.MapControllerRoute(
               name: "CommonEvent",
               pattern: "media-partner-event/{id?}",
               defaults: new { controller = "CommonEvent", action = "Index" });


            routes.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            routes.MapControllerRoute(
              name: "DefaulApi",
              pattern: "api/controller/{id?}");

            routes.MapControllerRoute(
                       name: "ArticleDetailswithprefix",
                       pattern: "article/{Prefix}/{id?}",
                       defaults: new { controller = "Article", action = "Index" });

            routes.MapControllerRoute(
              name: "NewsVideo",
              pattern: "news/videos/{id}",
              defaults: new { controller = "Search", action = "Videos" });

            routes.MapControllerRoute(
                          name: "NewsSector",
                          pattern: "news/{sector?}",
                          defaults: new { controller = "Search", action = "Index" }
                        );

            routes.MapControllerRoute(
           name: "SDGStories",
           pattern: "sdg-stories",
           defaults: new { controller = "Search", action = "SDGStories" }
         );

            routes.MapControllerRoute(
                name: "NewsAnalysis",
                pattern: "news-analysis/{type?}",
                defaults: new { controller = "Agencywire", action = "NewsAnalysis" }
            );

            routes.MapControllerRoute(
                name: "LivediscourseHome",
                pattern: "live-discourse",
                  defaults: new { controller = "Livediscourse", action = "Home" }
            );

            routes.MapControllerRoute(
                name: "LivediscourseArticle",
                pattern: "live-discourse",
                  defaults: new { controller = "Livediscourse", action = "Home" }
            );

            routes.MapControllerRoute(
            name: "LivediscourseArticle",
            pattern: "live-discourse/{id?}",
            defaults: new { controller = "Livediscourse", action = "Article" }
        );

            routes.MapControllerRoute(
                       name: "Blogs",
                       pattern: "blogs/{type?}",
                         defaults: new { controller = "Home", action = "DevBlogs" }
                   );
            routes.MapControllerRoute(
               name: "SouthAsiaEdition",
               pattern: "south-asia",
               defaults: new { controller = "Home", action = "SouthAsia" }
           );

            routes.MapControllerRoute(
                      name: "EastAndSouthEastAsiaEdition",
                      pattern: "south-east-asia",
                      defaults: new { controller = "Home", action = "EastAndSouthEastAsia" }
                      );

            routes.MapControllerRoute(
                        name: "EuropeAndCentralAsiaEdition",
                        pattern: "europe-central-asia",
                        defaults: new { controller = "Home", action = "EuropeAndCentralAsia" }
                        );
            routes.MapControllerRoute(
                          name: "CentralAfricaEdition",
                          pattern: "central-africa",
                          defaults: new { controller = "Home", action = "CentralAfrica" }
                      );
            routes.MapControllerRoute(
                          name: "EastAfricaEdition",
                          pattern: "east-africa",
                          defaults: new { controller = "Home", action = "EastAfrica" }
                      );
            routes.MapControllerRoute(
                           name: "SouthernAfricaEdition",
                           pattern: "southern-africa",
                           defaults: new { controller = "Home", action = "SouthernAfrica" }
                       );
            routes.MapControllerRoute(
                            name: "WestAfricaEdition",
                            pattern: "west-africa",
                            defaults: new { controller = "Home", action = "WestAfrica" }
                        );
            routes.MapControllerRoute(
                        name: "MiddleEastAndNorthAfricaEdition",
                        pattern: "middle-east-north-africa",
                        defaults: new { controller = "Home", action = "MiddleEastAndNorthAfrica" }
                        );
            routes.MapControllerRoute(
                        name: "LatinAmericaAndCaribbeanEdition",
                        pattern: "latin-america",
                        defaults: new { controller = "Home", action = "LatinAmericaAndCaribbean" }
                        );

            routes.MapControllerRoute(
                           name: "NorthAmericaEdition",
                           pattern: "north-america",
                           defaults: new { controller = "Home", action = "NorthAmerica" }
                       );

            routes.MapControllerRoute(
                          name: "NewsSector",
                          pattern: "news/{sector?}",
                          defaults: new { controller = "Search", action = "Index" }
                        );
            routes.MapControllerRoute(
                     name: "Contribute",
                     pattern: "Writeforus",
                     defaults: new { controller = "Home", action = "Contribute" }
                     );
            routes.MapControllerRoute(
                          name: "NewsLabel",
                          pattern: "stories/{label?}",
                          defaults: new { controller = "Home", action = "Search" }
                        );
            routes.MapControllerRoute(
                        name: "PressRelease",
                        pattern: "press-release",
                        defaults: new { controller = "NewsWire", action = "Index" }
                        );

            routes.MapControllerRoute(
                       name: "ArticleDetailswithprefix",
                       pattern: "ArticleDetailswithprefix/{Index}/{id?}",
                       defaults: new { controller = "Article", action = "Index" });

            routes.MapControllerRoute(
                name: "ArticleDetailswithprefix",
                pattern: "{prefix}/ArticleDetailswithprefix/{id?}",
                defaults: new { controller = "Article", action = "Index" },
                 constraints: new { customeroute = new SeoFriendlyRouteConstraint() }
                );

            routes.MapControllerRoute(
                           name: "AboutUs",
                           pattern: "aboutus/{id?}",
                           defaults: new { controller = "About", action = "Index" }
                       );
            routes.MapControllerRoute(
                         name: "Advertisement",
                         pattern: "advertisewithus",
                         defaults: new { controller = "Advertisements", action = "AdvertiseWithUs" }
                       );
            routes.MapControllerRoute(
                          name: "Career_Create",
                          pattern: "career",
                          defaults: new { controller = "Careers", action = "Create" }
                        );
            routes.MapControllerRoute(
                         name: "KnowledgePartnership",
                         pattern: "partners/knowledge-partners",
                         defaults: new { controller = "MediaPartnership", action = "Index" }
                       );

            routes.MapControllerRoute(
                      name: "MediaPartnership",
                      pattern: "partners/media-partners",
                      defaults: new { controller = "MediaPartnership", action = "MediaPartners" }
                    );
            routes.MapControllerRoute(
               name: "GlobalWarming",
               pattern: "events/global-warming-2019",
               defaults: new { controller = "Events", action = "GlobalWarming" }
           );
            routes.MapControllerRoute(
                name: "WorldRoadCongress",
                pattern: "events/world-road-congress",
                defaults: new { controller = "Events", action = "WorldRoadCongress" }
            );
            routes.MapControllerRoute(
                        name: "MediaPartnershipLink",
                        pattern: "partnership/{action}/{type?}",
                        defaults: new { controller = "MediaPartnership", action = "KnowledgePartners" }
               );
            routes.MapControllerRoute(
            name: "InternetOfThingsWorld",
            pattern: "events/internetthingsworld",
            defaults: new { controller = "Events", action = "IOTWorld" }
           );
            routes.MapControllerRoute(
                    name: "PacificEdition",
                    pattern: "pacific",
                    defaults: new { controller = "Home", action = "Pacific" }
              );

            //Events Pages Routes
            routes.MapControllerRoute(
               name: "PlasticFreeWorldConference",
               pattern: "events/plastic-free-world-conference-and-expo",
               defaults: new { controller = "Events", action = "PlasticFreeWorldConference" }
           );

            routes.MapControllerRoute(
               name: "TestconBanglore",
               pattern: "events/testcon-2019-banglore",
               defaults: new { controller = "Events", action = "TestconBanglore" }
           );

            routes.MapControllerRoute(
               name: "TestconSingapore",
               pattern: "events/testcon-2019-singapore",
               defaults: new { controller = "Events", action = "TestconSingapore" }
           );

            routes.MapControllerRoute(
              name: "BlockchainSingapore",
              pattern: "events/blockchain-2-0-singapore",
              defaults: new { controller = "Events", action = "BlockchainSingapore" }
          );

            routes.MapControllerRoute(
              name: "TestconBanglore",
              pattern: "events/testcon-2019-banglore",
              defaults: new { controller = "Events", action = "TestconBanglore" }
          );
            routes.MapControllerRoute(
                name: "TestconSingapore",
                pattern: "events/testcon-2019-singapore",
                defaults: new { controller = "Events", action = "TestconSingapore" }
            );
            routes.MapControllerRoute(
               name: "BusinessManagement",
               pattern: "events/business-management-social-sciences-entrepreneurship",
               defaults: new { controller = "Events", action = "BusinessManagement" }
           );
            routes.MapControllerRoute(
               name: "WomenHealth",
               pattern: "events/women-health-2020",
               defaults: new { controller = "Events", action = "WomenHealth" }
           );
            routes.MapControllerRoute(
               name: "GoGreen",
               pattern: "events/go-green-summit",
               defaults: new { controller = "Events", action = "GoGreen" }
           );
            routes.MapControllerRoute(
               name: "GreenUrbanism",
               pattern: "events/green-urbanism",
               defaults: new { controller = "Events", action = "GreenUrbanism" }
           );

            routes.MapControllerRoute(
              name: "PowerWeekAfrica",
              pattern: "events/power-week-africa",
              defaults: new { controller = "Events", action = "PowerWeekAfrica" }
          );
            routes.MapControllerRoute(
             name: "AfricaOilAndPower",
             pattern: "events/africa-oil-and-power",
             defaults: new { controller = "Events", action = "AfricaOilAndPower" }
         );
            routes.MapControllerRoute(
             name: "AnnualSmartProcurementWorldIndaba",
             pattern: "events/annual-smart-procurement-world-indaba",
             defaults: new { controller = "Events", action = "AnnualSmartProcurementWorldIndaba" }
         );

            routes.MapControllerRoute(
             name: "AfricaOilWeek",
             pattern: "events/africa-oil-week",
             defaults: new { controller = "Events", action = "AfricaOilWeek" }
         );
            routes.MapControllerRoute(
              name: "AfricaReEnergyExpo",
              pattern: "events/africa-re-energy-expo",
              defaults: new { controller = "Events", action = "AfricaReEnergyExpo" }
          );
            routes.MapControllerRoute(
              name: "AfricaEnergyIndaba",
              pattern: "events/africa-energy-indaba",
              defaults: new { controller = "Events", action = "AfricaEnergyIndaba" }
          );
            routes.MapControllerRoute(
             name: "OffShoreIndiaCongress",
             pattern: "events/offshore-india-congress",
             defaults: new { controller = "Events", action = "OffShoreIndiaCongress" }
         );
            routes.MapControllerRoute(
            name: "WorldNuclearIndustryCongress",
            pattern: "events/world-nuclear-industry-congress",
            defaults: new { controller = "Events", action = "WorldNuclearIndustryCongress" }
        );
            routes.MapControllerRoute(
                      name: "WorldGasLngConference",
                      pattern: "events/world-gas-lng-conference",
                      defaults: new { controller = "Events", action = "WorldGasLngConference" }
                  );
            routes.MapControllerRoute(
                    name: "OffshoreNorthSeaEuropeCongress",
                    pattern: "events/offshore-north-sea-europe-congress",
                    defaults: new { controller = "Events", action = "OffshoreNorthSeaEuropeCongress" }
                );
            routes.MapControllerRoute(
                    name: "DiversityInEnergySummit",
                    pattern: "events/diversity-in-energy-summit",
                    defaults: new { controller = "Events", action = "DiversityInEnergySummit" }
                );
            routes.MapControllerRoute(
                 name: "BigFiveBoardAwards",
                 pattern: "events/big-five-board-awards",
                 defaults: new { controller = "Events", action = "BigFiveBoardAwards" }
             );

            routes.MapControllerRoute(
             name: "MotherNotPatient",
             pattern: "mother-not-patient",
             defaults: new { controller = "Campaign", action = "MotherNotPatient" }
           );

        }
    }
}
