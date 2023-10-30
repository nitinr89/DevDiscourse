using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class DiscourseTopic
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Sector { get; set; }
        [Display(Name = "Admin Check")]
        public bool IsPublished { get; set; }
        public int DislikeCount { get; set; }
        public int LikeCount { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public DiscourseTopic()
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