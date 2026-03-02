using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ContributorModels
{
    public class Earnings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public long NewsId { get; set; }
        public int ViewCount { get; set; }
        public double Amount { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("NewsId")]
        public virtual Content Contents { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public Earnings()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}