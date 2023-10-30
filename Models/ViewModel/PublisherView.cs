using System;
using System.Text.RegularExpressions;

namespace Devdiscourse.Models.ViewModel
{
    public class PublisherView
    {
        public DateTime ModifiedOn { get; internal set; }
        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public string Url { get; internal set; }
        public string ImageUrl { get; internal set; }
        public string Country { get; internal set; }
        public string Label { get; internal set; }

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
        private string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}