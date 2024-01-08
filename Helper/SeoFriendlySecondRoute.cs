using ServiceStack;
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
        //public SeoFriendlySecondRoute(IRouter target, string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens,
        //   IInlineConstraintResolver inlineConstraintResolver, RequestDelegate next)
        //   : base(target, url, defaults, constraints, dataTokens, inlineConstraintResolver)
        //{
        //    _next = next;
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
