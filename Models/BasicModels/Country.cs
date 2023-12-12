using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Region")]
        public Guid RegionId { get; set; }
        [Display(Name = "Country")]
        public string? Title { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region? Regions { get; set; }
    }
}