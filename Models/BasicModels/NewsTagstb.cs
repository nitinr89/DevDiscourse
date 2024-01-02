using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class NewsTagstb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tagstb? Tagstb { get; set; }
        public Guid DevNewsId { get; set; }
        [ForeignKey("DevNewsId")]
        public virtual DevNews?DevNews { get; set; }
    }
}