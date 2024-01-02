using System.Text.RegularExpressions;

namespace Devdiscourse.Models.ViewModel
{
    public class EmailViewModel
    {
        public string Title { get; set; }
        public string ImageUrl { get;  set; }
        public long NewsId { get; set; }
        public string Description { get; set; }
        public string Sector { get; internal set; }

        public string Slug()
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
        public string GetFirstParagraph()
        {
            string phrase = string.Format("{0}",Description);
            Match m = Regex.Match(phrase, @"<p>\s*(.+?)\s*</p>");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }
    }
}