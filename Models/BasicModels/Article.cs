using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Web.Mvc;

namespace Devdiscourse.Models.BasicModels
{
    public class Article : BaseClass
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
        public string Title { get; set; }
        [Required]
        [Display(Name = "Summary")]
        //[AllowHtml]
        public string Description { get; set; }
        [Display(Name = "Attach File")]
        [Required]
        public string FileUrl { get; set; }
        public string FileMimeType { get; set; }
        public string Tags { get; set; }
        public string Country { get; set; }
        [Display(Name = "Admin Check")]
        public bool AdminCheck { get; set; }
        public string Creator { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
    }
}