using System;

namespace Devdiscourse.Models.ContributorModels
{
	public class PublishView
	{
        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string Creator { get; internal set; }
        public bool IsVideo { get; internal set; }
        public int ViewCount { get; internal set; }
        public double Amount { get; internal set; }
        public long NewsId { get; internal set; }
    }
}