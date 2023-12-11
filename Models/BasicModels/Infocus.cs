using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class Infocus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "News")]
        public long NewsId { get; set; }
        [Display(Name = "Order")]
        [Required(ErrorMessage ="The Sr. No. is required")]
        [Range(1,11)]
        public int SrNo { get; set; }
        [Required]
        public string Edition { get; set; }
        [Display(Name = "Type")]
        public string? ItemType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? Creator { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser? ApplicationUsers { get; set; }
        public Infocus()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}