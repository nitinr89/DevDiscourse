using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Devdiscourse.Models.BasicModels
{
    public class Livediscourse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [Required]
        public string Title { get; set; }
        public string? SubTitle { get; set; }
        
        [Display(Name = "Summary")]
        //[AllowHtml]
        public string? Description { get; set; }
        [Required]
        public string Sector { get; set; }
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
        [Display(Name = "Image Copyright")]
        public string? ImageCopyright { get; set; }
        [Display(Name = "Image Caption")]
        public string? ImageCaption { get; set; }
        public string? ParentStoryLink { get; set; }
        public string? Tags { get; set; }
        [Display(Name = "Admin Check")]
        public bool AdminCheck { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public int CommentCount { get; set; }
        public int FollowCount { get; set; }
        public bool IsPublic { get; set; }
        public string? Author { get; set; }
        [Display(Name ="Close Date")]
        public DateTime? Close_Date { get; set; }
        public bool Status { get; set; }
        public string? Creator { get; set; }
        [Required]
        public long LivediscourseIndex { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime PublishedOn { get; set; }
        public long ParentId { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser? ApplicationUsers { get; set; }
        public virtual ICollection<FollowLivediscourse>? FollowLivediscourse { get; set; }
        public virtual ICollection<DiscourseIndexes>? DiscourseIndex { get; set; }
        public virtual ICollection<LivediscourseVideo>? LivediscourseVideos { get; set; }

        public virtual ICollection<LiveDiscourseInfocus>? LiveDiscourseInfocus { get; set; }
        public Livediscourse()
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
    }
}