using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Devdiscourse.Models.BasicModels
{
    public class Meme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Meme of the Week")]
        public bool IsWeekMeme { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string GenerateSecondSlug()
        {
            string phrase = string.Format("{0}-{1}", Id, Title);

            string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim();
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