using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class ImageGallery : BaseClass
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
        [Display(Name = "Image Url")]
        [Required]
        public string ImageUrl { get; set; }
        public string FileMimeType { get; set; }
        public string FileSize { get; set; }
        public string Sector { get; set; }
        public string ImageCopyright { get; set; }
        public string Caption { get; set; }
        public int UseCount { get; set; }
        public string Tags { get; set; }
    }
}