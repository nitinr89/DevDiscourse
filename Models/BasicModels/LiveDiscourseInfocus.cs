using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class LiveDiscourseInfocus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Order")]
        [Required(ErrorMessage = "The Sr. No. is required")]
        [Range(1, 11)]
        public int SrNo { get; set; }
        [Required]
        public string Edition { get; set; }
        [Display(Name = "Type")]
        public string ItemType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Creator { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public long LivediscourseId { get; set; }
        [ForeignKey("LivediscourseId")]

        public virtual Livediscourse Livediscourse { get; set; }
        public LiveDiscourseInfocus()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}