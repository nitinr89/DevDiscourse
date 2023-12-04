using Devdiscourse.Models.BasicModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
//using System.Web.Mvc;

namespace Devdiscourse.Models.VideoNewsModels
{
    public class VideoNews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
       
        [Required]
        public string Title { get; set; }
        [Display(Name = "Alternate Headline")]
        public string AlternateHeadline { get; set; }
        //[AllowHtml]
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        //news detail
        public string Source { get; set; }        
        public string Label { get; set; }        
        public int ViewCount { get; set; }
        public string Creator { get; set; }
        public bool AdminCheck { get; set; }
        public bool EditorPick { get; set; }
        public string VideoName { get; set; }
        public string BlobName { get; set; }
        [Required(ErrorMessage ="Video is Required")]
        public string VideoUrl { get; set; }
        public string MimeType { get; set; }
        public string Size { get; set; }
        public string Caption { get; set; }
        public string Copyright { get; set; }
        public string VideoThumbUrl { get; set; }
        public string Duration { get; set; }
        //author
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public DateTime? PublishedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public virtual ICollection<VideoNewsTag> VideoNewsTags { get; set; }
        public virtual ICollection<VideoNewsRegion> VideoNewsRegions { get; set; }
        public virtual ICollection<VideoNewsSector> VideoNewsSectors { get; set; }

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
            //byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
        public VideoNews()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}