using System;

namespace Devdiscourse.Models.BasicModels
{
    public class AssignNews 
    {
        public long Id { get; set; }
        public long NewsId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Creator { get; set; }
    }
}