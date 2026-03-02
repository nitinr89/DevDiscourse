using System;

namespace Devdiscourse.Models.ViewModel
{
    public class CommentView
    {
        public string UserName { get; internal set; }
        public Guid CommentId { get; internal set; }
        public Guid ItemId { get; internal set; }
        public string Comment { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public Guid ParentCommentId { get; internal set; }
        public string CommentBy { get; internal set; }
        public string ProfilePic { get; internal set; }
    }
}