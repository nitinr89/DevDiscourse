using System;
using System.Text.RegularExpressions;

namespace Devdiscourse.Models.ViewModel
{
    public class CricketWorldCupNews
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ImageUrl { get; set; }
        public string Label { get; set; }
        public string GenerateSecondSlug()
        {
            string phrase = string.Format("{0}-{1}", Id, Title);

            string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 130 ? str.Length : 130).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }
        private string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
        public string TimeAgo()
        {
            string result = string.Empty;
            var timeSpan = DateTime.UtcNow.ToLocalTime().Subtract(ModifiedOn.ToLocalTime());

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} Seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("About {0} minutes ago", timeSpan.Minutes) :
                    "About a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("About {0} hours ago", timeSpan.Hours) :
                    "About an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(3))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("About {0} days ago", timeSpan.Days) :
                    "Yesterday";
            }
            else
            {
                result = ModifiedOn.ToLocalTime().ToString("dd MMMM yyyy");
            }
            return result;
        }
    }
}