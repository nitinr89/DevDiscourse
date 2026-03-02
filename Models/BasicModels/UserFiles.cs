using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class UserFiles : BaseClass
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
        public string? Title { get; set; }
        [Display(Name = "Image Url")]
        [Required]
        public string FileUrl { get; set; }
        public string? FileMimeType { get; set; }
        public string? FileSize { get; set; }
        [Display(Name = "File For")]
        public string? FileFor { get; set; }
    }
}