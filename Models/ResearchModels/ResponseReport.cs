using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ResearchModels
{
    public class ResponseReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string ReportText { get; set; }
        [Required]
        public Guid ResponseId { get; set; }
        public string ResponseTitle { get; set; }
        public string ReportBy { get; set; }
        public DateTime ReportedOn { get; set; }
        [ForeignKey("ReportBy")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}