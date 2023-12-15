using Devdiscourse.Models.VideoNewsModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class Region
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int SrNo { get; set; } = 0;
        [Display(Name = "Region")]
        public string? Title { get; set; }
        public virtual ICollection<Country>? Countries { get; set; }
        public virtual ICollection<Website>? Websites { get; set; }
        public virtual ICollection<VideoNewsSector>? VideoNewsSectors { get; set; }
        public virtual ICollection<RegionNewsRanking>? RegionNewsRankings { get; set; }
    }
}