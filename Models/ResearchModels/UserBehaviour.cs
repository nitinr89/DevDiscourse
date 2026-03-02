using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ResearchModels
{
    public class UserBehaviour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public Behaviour BehaviourType { get; set; }
        [Required]
        public Guid ResponseId { get; set; }
        public string ResponseTitle { get; set; }
        public string Creator { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}