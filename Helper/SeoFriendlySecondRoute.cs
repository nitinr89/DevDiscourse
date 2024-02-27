using System.Text.RegularExpressions;

namespace Devdiscourse.Helper
{
    public class SeoFriendlySecondRoute
    {
        private readonly RequestDelegate _next;

        public SeoFriendlySecondRoute(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the request path is for an article page
            var path = context.Request.Path.Value;

            // Assuming the article pages follow the pattern "/article/..."
            if (path != null && (path.StartsWith("/article/") || path.StartsWith("/news/")))
            {
                var routeData = context.GetRouteData();

                if (routeData != null && routeData.Values.ContainsKey("id"))
                {
                    // Modify the "id" value only if the request is for an article page
                    routeData.Values["id"] = GetIdValue(routeData.Values["id"]);
                }
            }
            await _next(context);
        }

        private object GetIdValue(object id)
        {
            if (id != null)
            {
                string idValue = id.ToString();
                var regex = new Regex(@"^(?<id>\d+).*$");
                var match = regex.Match(idValue);

                if (match.Success)
                {
                    return match.Groups["id"].Value;
                }
            }

            return id;
        }
    }

}
