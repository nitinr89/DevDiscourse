using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class TrendingNews
    {
        [Key]
        public Guid Id { get; set; }
        public Guid NewsId { get; set; }
        public DateTime ViewedOn { get; set; }
        public string? Ipv4 { get; set; }
        public string? Country { get; set; }
    }
}
