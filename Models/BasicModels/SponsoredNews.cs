using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class SponsoredNews
    {
        [Key]
        public int Id { get; set; }
        public Guid NewsId { get; set; }
        public int Position { get; set; } = 0;
        public int Sector { get; set; }
        public bool IsActive { get; set; }
        public DateTime EndTime { get; set; }
    }
}
