using Devdiscourse.Models.ContributorModels;
using Devdiscourse;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Web.Mvc;

namespace Devdiscourse.Models.Others
{
    public class NewsWireModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Display(Name = "Short Description")]
        public string SubTitle { get; set; }
        [Display(Name = "Content")]
        //[AllowHtml]
        public string Description { get; set; }
        public string Type { get; set; }
        [Display(Name = "Labels")]
        public string NewsLabels { get; set; }
        public string Sector { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        [Display(Name = "Author Name")]
        public string Source { get; set; }
        public string AuthorImage { get; set; }
        [Display(Name = "Upload Image")]
        public string ImageUrl { get; set; }
        public string FileMimeType { get; set; }        // File Mime Type
        [Display(Name = "Image Copyright")]
        public string ImageCopyright { get; set; }
        [Display(Name = "Video")]
        public string VideoUrl { get; set; }
        public bool IsVideo { get; set; }
        [Display(Name = "Publish Date")]
        public DateTime PublishedDate { get; set; }
        public string Tags { get; set; }
        public ContentStage Status { get; set; }
        [Display(Name = "Reason of Reject")]
        public string ReasonofReject { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public NewsWireModel()
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
    }
}