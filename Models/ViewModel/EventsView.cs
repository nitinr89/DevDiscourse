using System;

namespace Devdiscourse.Models.ViewModel
{
    public class EventsView
    {
        public DateTime CreatedOn { get; internal set; }
        public string Description { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public Guid Id { get; internal set; }
        public string ImageUrl { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public string Title { get; internal set; }
        public string Venue { get; internal set; }
    }
}