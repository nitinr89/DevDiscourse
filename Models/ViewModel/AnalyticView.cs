using System;

namespace Devdiscourse.Models.ViewModel
{
    public class AnalyticView
    {
        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public string Region { get; internal set; }
        public string Country { get; internal set; }
        public string Sector { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public DateTime ModifiedOn { get; internal set; }
    }
}