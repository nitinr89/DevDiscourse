using System;
namespace Devdiscourse.Models.ContributorModels
{
	public class ViewContent
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public bool IsVideo { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid NewsId { get; set; }
        public string Type { get; set; }
    }
}