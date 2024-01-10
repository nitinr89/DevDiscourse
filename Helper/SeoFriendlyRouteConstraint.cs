using System.Text.RegularExpressions;

namespace Devdiscourse.Helper
{
    public class SeoFriendlyRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var value) && value is string stringValue)
            {
                // Your constraint logic here...

                // For example, check if the value follows a certain format
                var regex = new Regex(@"^[a-zA-Z0-9]+$");
                return regex.IsMatch(stringValue);
            }

            return false; // Return false if the constraint is not satisfied
        }
    }
}
