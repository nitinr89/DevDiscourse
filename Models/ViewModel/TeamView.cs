using System;

namespace Devdiscourse.Models.ViewModel
{
    public class TeamView
    {
        public DateTime CreatedOn { get; internal set; }
        public string Desgination { get; internal set; }
        public object Profile { get; internal set; }
        public int SrNo { get; internal set; }
        public Guid TeamId { get; internal set; }
        public string TeamMember { get; internal set; }
    }
}