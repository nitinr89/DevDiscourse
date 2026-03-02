using System;
using System.Collections.Generic;

namespace Devdiscourse.Models.ViewModel
{
    public class BlogsView
    {
        public DateTime CreatedOn { get; internal set; }
        public string Description { get; internal set; }
        public long Id { get; internal set; }
        public object ImageUrl { get; internal set; }
        public int LikeCount { get; internal set; }
        public string Sector { get; internal set; }
        public List<SubBlogsView> SectorList { get; set; }
        public string Tags { get; internal set; }
        public string Title { get; internal set; }
    }
    public class SubBlogsView
    {
        public string title { get; set; }
    }
}