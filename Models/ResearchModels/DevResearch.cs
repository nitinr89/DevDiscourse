using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ResearchModels
{
    public class DevResearch : BaseClass
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
        [Required]
        [Display(Name = "Title of Research")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Abstract")]
        //[AllowHtml]
        public string Description { get; set; }
        [Display(Name = "Attach File")]
        [Required]
        public string FileUrl { get; set; }
        public string FileMimeType { get; set; }
        public string Tags { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        [Display(Name = "Place")]
        public string Location { get; set; }
        [Display(Name = "Year of Publication")]
        [Required]
        public string Year { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name = "Name of Source")]
        public string Source { get; set; }
        [Display(Name = "Admin Check")]
        public bool AdminCheck { get; set; }
        public string Creator { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
    }
}