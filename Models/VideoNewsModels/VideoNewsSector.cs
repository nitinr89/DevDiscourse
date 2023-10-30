using Devdiscourse.Models.BasicModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.VideoNewsModels
{
    public class VideoNewsSector
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }       
        public long VideoNewsId { get; set; }
        public int SectorId { get; set; }
        [ForeignKey("SectorId")]
        public virtual DevSector DevSectors { get; set; }

        [ForeignKey("VideoNewsId")]
        public virtual VideoNews VideoNews { get; set; }
    }
}