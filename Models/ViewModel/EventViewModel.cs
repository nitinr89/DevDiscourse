using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Devdiscourse.Models.ViewModel
{
    public class EventViewModel
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string CardUrl { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Featured { get; set; }

        public string AddressRegion { get; set; }
        public string Tagline { get; set; }

        public string GenerateSlug()
        {
            string phrase = string.Format("{0}-{1}", Id, Name);

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