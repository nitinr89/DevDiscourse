using Devdiscourse.Models.VideoNewsModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class DevSector
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SrNo { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public virtual ICollection<DiscourseTag>? DiscourseTag { get; set; }
        public virtual ICollection<VideoNewsSector>? VideoNewsSectors { get; set; }
    }
}