using System;

namespace Devdiscourse.Models.ViewModel
{
    public class ResearchView
    {
        public DateTime CreatedOn { get; internal set; }
        public string Description { get; internal set; }
        public Guid Id { get; internal set; }
        public object ImageUrl { get; internal set; }
        public object LikeCount { get; internal set; }
        public string Tags { get; internal set; }
        public string Title { get; internal set; }
    }
}