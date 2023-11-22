using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
//using System.Web.Mvc;

namespace Devdiscourse.Models.BasicModels
{
    public class Event : BaseClass
    {
        public override Guid Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Display(Name = "SubTitle")]
        public string? SubTitle { get; set; }
        [Required]
        [Display(Name = "Summary")]
        //[AllowHtml]
        public string Description { get; set; }
        [Required]
        public string Sector { get; set; }
        public string? Themes { get; set; }
        public string? Category { get; set; }
        [Display(Name = "Image")]
        public string? FileUrl { get; set; }
        public string? Source { get; set; }
        public string? FileMimeType { get; set; }
        public string? Tags { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Display(Name = "Admin Check")]
        public bool AdminCheck { get; set; }
        public bool IsInfocus { get; set; }
        public bool IsGlobal { get; set; }
        public string? Creator { get; set; }
        public int ViewCount { get; set; }
        public string? FileSize { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser? ApplicationUsers { get; set; }

        public string GenerateSlug()
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
        public string GenerateSecondSlug()
        {
            string phrase = string.Format("{0}-{1}", EventId, Title);

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
            //byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}