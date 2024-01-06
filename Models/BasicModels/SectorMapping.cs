using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class SectorMapping
    {
        [Key, Column(Order = 0)]
        public Guid NewsId { get; set; }
        [Key, Column(Order = 1)]
        public int SectorId { get; set; }
        [ForeignKey("SectorId")]
        public virtual DevSector? DevSector { get; set; }
        [ForeignKey("NewsId")]
        public virtual DevNews? DevNews { get; set; }
    }
}
