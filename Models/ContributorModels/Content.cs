using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
//using System.Web.Mvc;

namespace Devdiscourse.Models.ContributorModels
{
    public class Content
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Display(Name = "SubTitle")]
        public string SubTitle { get; set; }
        [Required]
        //[AllowHtml]
        public string Description { get; set; }
        public string Type { get; set; }
        [Display(Name = "Labels")]
        public string NewsLabels { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        public string FileMimeType { get; set; }        // File Mime Type
        [Display(Name = "Image Copyright")]
        public string ImageCopyright { get; set; }
        [Display(Name = "Video")]
        public string VideoUrl { get; set; }
        public bool IsVideo { get; set; }
        public string Tags { get; set; }
        public string Sector { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        [Display(Name = "Author Name")]
        public string Source { get; set; }
        public ContentStage ContentStatus { get; set; }
        [Display(Name = "Reason of Reject")]
        public string ReasonofReject { get; set; }
        public string Creator { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public virtual ICollection<Earnings> Earnings { get; set; }
        public Content()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
            if (ModifiedOn == new DateTime())
            {
                ModifiedOn = DateTime.UtcNow;
            }
        }
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