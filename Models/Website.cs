using Devdiscourse.Models.BasicModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models
{
    public class Website
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Region")]
        public Guid? RegionId { get; set; }
        public string? Country { get; set; }
        [Display(Name = "Website Url")]
        [Required]
        [Url(ErrorMessage = "Please enter a valid url")]
        public string? SiteUrl { get; set; }
        public string? Type { get; set; }
        [Display(Name = "Title")]
        public string? PressRelease { get; set; }
        [Display(Name = "Press Release Url")]
        [Required]
        [Url(ErrorMessage = "Please enter a valid url")]
        public string? PressReleaseUrl { get; set; }
        public DateTime? CreatedOn { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region? Regions { get; set; }
    }
}