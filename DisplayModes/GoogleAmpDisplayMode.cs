
using Microsoft.AspNetCore.Mvc.Razor;

namespace Devdiscourse.DisplayModes
{
    public class GoogleAmpDisplayMode : IViewLocationExpander
    {

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue("amp", out var ampValue) && ampValue.Equals("true"))
            {
                // Use a separate folder for AMP views
                return new[] { "/Views/Article/{1}/{0}.amp.cshtml", "/Views/Amp/Shared/{0}.amp.cshtml" }
                    .Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // Check if AMP flag is set in the route values
            if (context.ActionContext.RouteData.Values.TryGetValue("amp", out var ampValue))
            {
                context.Values["amp"] = ampValue.ToString();
            }
        }
    }
 

}
