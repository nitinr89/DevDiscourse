using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class UserComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CommentId { get; set; }
        [Required]
        public string CommentText { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        public string ItemTitle { get; set; }
        public long ParentId { get; set; }
        public string CommentBy { get; set; }
        public DateTime CommentOn { get; set; }
        [ForeignKey("CommentBy")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}