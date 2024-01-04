using System.Text.RegularExpressions;

namespace Devdiscourse.Helper
{
    public class SeoFriendlyRoute : Route
    {
        private readonly RequestDelegate _next;

        public SeoFriendlyRoute(IRouter target, string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver, RequestDelegate next)
            : base(target, url, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _next = next;
        }

        //public override Task RouteAsync(RouteContext context)
        //{
        //    var routeData = context.RouteData;

        //    if (routeData != null)
        //    {
        //        if (routeData.Values.ContainsKey("id"))
        //        {
        //            routeData.Values["id"] = GetIdValue(routeData.Values["id"]);
        //        }
        //    }

        //    return base.RouteAsync(context);
        //}
        public async Task Invoke(HttpContext context)
        {
            var routeData = context.GetRouteData();

            if (routeData != null && routeData.Values.ContainsKey("id"))
            {
                routeData.Values["id"] = GetIdValue(routeData.Values["id"]);
            }

            await _next(context);
        }
        private object GetIdValue(object id)
        {
            if (id != null)
            {
                string? idValue = id.ToString();
                var regex = new Regex(@"^(?<id>[{(]?[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}[)}]?).*$");
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
