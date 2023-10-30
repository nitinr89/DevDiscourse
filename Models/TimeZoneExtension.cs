using System;
using System.Web;

namespace Devdiscourse.Models
{
    public static class TimeZoneExtension
    {
        /// <summary>
        /// Convert the passed datetime into client timezone.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToClientTime(this DateTime dt)
        {
            //var timeOffSet = HttpContext.Current.Session["timezoneoffset"];  // read the value from session

            //if (timeOffSet != null)
            //{
            //    var offset = int.Parse(timeOffSet.ToString());
            //    dt = dt.AddMinutes(-1 * offset);

            //    return dt;
            //}

            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime();
        }
    }
}