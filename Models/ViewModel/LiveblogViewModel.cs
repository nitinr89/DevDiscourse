using Devdiscourse.Models.BasicModels;

namespace Devdiscourse.Models.ViewModel
{
    public class LiveblogViewModel
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageCopyright { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime PublishedOn { get; set; }
        public string? ParentStoryLink { get; set; }
        public List<DiscourseComment>? DiscourseComments { get; set; }
        public int CommentCount { get; set; }
        public int DislikeCount { get; set; }
        public int FollowCount { get; set; }
        public int LikeCount { get;  set; }
        public ReactType Reacted { get; set; }
        public string? Tags { get;  set; }
    }
}