using System.Text.RegularExpressions;

namespace Devdiscourse.Models.ViewModel
{
    public class NewsCache
    {
        public Guid Id { get; set; }
        public long NewsId { get; set; }
        public string Title { get; set; }

        public string AlternateHeadline { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string NewsLabels { get; set; }
        public string Category { get; set; }
        public string Sector { get; set; }
        public string Themes { get; set; }
        public string ImageUrl { get; set; }
        public string FileMimeType { get; set; }        // File Mime Type
        public string ImageCopyright { get; set; }
        public string ImageCaption { get; set; }
        public string Tags { get; set; }
        public bool AdminCheck { get; set; }
        public bool IsSponsored { get; set; }
        public bool EditorPick { get; set; }
        public bool IsInfocus { get; set; }
        public bool IsVideo { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsStandout { get; set; }
        public bool IsIndexed { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Source { get; set; }
        public string OriginalSource { get; set; }
        public string SourceUrl { get; set; }
        public string Author { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string FileSize { get; set; }
        public string WorkStage { get; set; }
        public string Creator { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime PublishedOn { get; set; }
        public long ReferenceId { get; set; }
        public string ProfilePic { get; set; }
        public List<NewsFileCache> UserNewsFiles { get; set; }

        public string GenerateSlug()
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
        public string GenerateSecondSlug()
        {
            string phrase = string.Format("{0}-{1}", NewsId, Title);

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
            //byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
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