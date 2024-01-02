using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class FollowTag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string FollowBy { get; set; }
        public DateTime FollowOn { get; set; }
        public long TagId { get; set; }
        [ForeignKey("FollowBy")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        [ForeignKey("TagId")]
        public virtual DiscourseTag DiscourseTags { get; set; }
    }
}