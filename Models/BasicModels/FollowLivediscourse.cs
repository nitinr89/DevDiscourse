using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class FollowLivediscourse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string FollowBy { get; set; }
        public DateTime FollowOn { get; set; }
        [DisplayName("Is Moderator")]
        public bool IsModerator { get; set; }
        public long LivediscourseId { get; set; }
        [ForeignKey("FollowBy")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        [ForeignKey("LivediscourseId")]
        public virtual Livediscourse Livediscourses { get; set; }
    }
}