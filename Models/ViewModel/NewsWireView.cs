using System;
using Devdiscourse.Models.ContributorModels;

namespace Devdiscourse.Models.ViewModel
{
    public class NewsWireView
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public ContentStage Status { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public string NewsLabels { get; set; }
    }
}