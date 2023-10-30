using System;

namespace Devdiscourse.Models.ViewModel
{
    public class NewsView
    {
        public DateTime CreatedOn { get; internal set; }
        public long Id { get; internal set; }
        public string ImageUrl { get; internal set; }
        public string Sector { get; internal set; }
        public string Title { get; internal set; }
        public string Country { get; internal set; }
        public string NewsType { get; internal set; }
        public bool IsInfocus { get; internal set; }
    }
    public class SubNewsView
    {
        public string title { get; set; }
    }
}