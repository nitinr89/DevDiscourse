using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Devdiscourse.Models.BasicModels
{
    public class DevNews : BaseClass
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
        public long NewsId { get; set; }
        [Required]
        public string Title { get; set; }

        [Display(Name = "Alternate Headline")]
        public string? AlternateHeadline { get; set; }

        [Display(Name = "SubTitle")]
        public string? SubTitle { get; set; }
        [Required]
        [Display(Name = "Summary")]
        //[AllowHtml]
        public string Description { get; set; }
        public string? Type { get; set; }
        public string? SubType { get; set; }
        [Display(Name = "Labels")]
        public string? NewsLabels { get; set; }
        public string? Category { get; set; }
        [Required]
        public string Sector { get; set; }
        public string? Themes { get; set; }
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
        public string? FileMimeType { get; set; }        // File Mime Type
        [Display(Name = "Image Copyright")]
        public string? ImageCopyright { get; set; }
        [Display(Name = "Image Caption")]
        public string? ImageCaption { get; set; }
        public string? Tags { get; set; }
        [Display(Name = "Admin Check")]
        public bool AdminCheck { get; set; }
        public bool IsSponsored { get; set; }
        [Display(Name = "Editor Pick")]
        public bool EditorPick { get; set; }
        [Display(Name = "In Focus")]
        public bool IsInfocus { get; set; }
        [Display(Name = "IsVideo")]
        public bool IsVideo { get; set; }
        [Display(Name = "IsGlobal")]
        public bool IsGlobal { get; set; }
        public bool IsStandout { get; set; }
        public bool IsIndexed { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? Source { get; set; }
        public string? OriginalSource { get; set; }
        public string? SourceUrl { get; set; }
        public string? Author { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string? FileSize { get; set; }
        public string? WorkStage { get; set; }
        public string Creator { get; set; }
        public DateTime PublishedOn { get; set; }
        //public virtual IList<Tag> NewsTags { get; set; }
        public long ReferenceId { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public virtual ICollection<UserNewsFile>? UserNewsFiles { get; set; }
        public virtual ICollection<NewsTagstb>? NewsTagstb { get; set; }
        public virtual ICollection<RegionNewsRanking>? RegionNewsRankings { get; set; }
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
        public DevNews()
        {
            if (PublishedOn == new DateTime())
            {
                PublishedOn = DateTime.UtcNow;
            }
        }
    }
}