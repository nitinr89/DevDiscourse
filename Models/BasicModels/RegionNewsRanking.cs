using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class RegionNewsRanking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RegionNewsId { get; set; }
        public Guid RegionId { get; set; }
        public Guid NewsId { get; set; }
        public float Ranking { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
        [ForeignKey("NewsId")]
        public virtual DevNews DevNews { get; set; }

    }
}