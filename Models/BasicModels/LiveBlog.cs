using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class LiveBlog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Summary")]
        //[AllowHtml]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Parent")]
        public long ParentId { get; set; }
        public string ImageUrl { get; set; }
        public string FileMimeType { get; set; }        // File Mime Type
        [Display(Name = "Image Copyright")]
        public string ImageCopyright { get; set; }
        public string FileSize { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public LiveBlog()
        {
            if(CreatedOn == new DateTime())
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