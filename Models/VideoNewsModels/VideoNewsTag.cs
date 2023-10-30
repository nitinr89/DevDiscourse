using Devdiscourse.Models.BasicModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.VideoNewsModels
{
    public class VideoNewsTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tagstb Tagstb { get; set; }
        public long VideoNewsId { get; set; }
        [ForeignKey("VideoNewsId")]
        public virtual VideoNews VideoNews { get; set; }
    }
}