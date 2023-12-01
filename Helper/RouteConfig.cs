namespace Devdiscourse.Helper
{
    public static class RouteConfig
    {
        public static void ConfigureRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "ArticleDetailswithprefix", 
                //new SeoFriendlySecondRoute("article/{prefix}/{id}",
                pattern: "ArticleDetailswithprefix/{Index}/{id?}",
                defaults: new { controller = "Article", action = "Index" });

            // Additional routes can be configured here
        }



    }
}
