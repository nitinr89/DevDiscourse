using System;
using System.Text.RegularExpressions;

namespace Devdiscourse.Models.ViewModel
{
    public class NewsListView
    {
        public Guid Id { get;  set; }
        public long NewsId { get;  set; }
        public string? Sector { get;  set; }
        public string? Category { get;  set; }
        public string? Title { get;  set; }
        public string? SubTitle { get;  set; }
        public string? Creator { get;  set; }
        public string? CreatorName { get;  set; }
        public string? Region { get;  set; }
        public string? Country { get;  set; }
        public string? ImageUrl { get;  set; }
        public string? Source { get;  set; }
        public string? SourceUrl { get;  set; }
        public bool AdminCheck { get;  set; }
        public bool IsInfocus { get;  set; }
        public bool EditorPick { get;  set; }
        public bool? IsGlobal { get;  set; }
        public DateTime CreatedOn { get;  set; }
        public DateTime ModifiedOn { get;  set; }
        public bool IsFifa { get;  set; }
        public string? Label { get;  set; }
        public bool IsIndex { get;  set; }
        public string? WorkStage { get;  set; }
        public int ViewCount { get;  set; }

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
    }
}