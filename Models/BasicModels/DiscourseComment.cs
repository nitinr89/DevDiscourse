using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class DiscourseComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CommentId { get; set; }
        [Required]
        public string CommentText { get; set; }
        [Required]
        public long ItemId { get; set; }
        public long ParentId { get; set; }
        public long RootParentId { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public int EndorseCount { get; set; }
        public int RejectCount { get; set; }
        public string CommentBy { get; set; }
        public string ReplyText { get; set; }
        public bool IsHidden { get; set; }
        public int ChildCount { get; set; }
        public DateTime CommentOn { get; set; }
        [ForeignKey("CommentBy")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}