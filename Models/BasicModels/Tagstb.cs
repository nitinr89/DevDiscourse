using Devdiscourse.Models.VideoNewsModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class Tagstb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name ="Tag Title")]
        public string? TagTitle { get; set; }
        public virtual ICollection<NewsTagstb>? NewsTagstb { get; set; }

        public virtual ICollection<VideoNewsTag>? VideoNewsTags { get; set; }

    }
}