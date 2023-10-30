using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Devdiscourse.Models.ResearchModels
{
    public class Rank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<SDGSamurai> SDGSamurais { get; set; }
    }
}