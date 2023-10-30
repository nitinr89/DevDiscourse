using System;
namespace Devdiscourse.Models.ViewModel
{
    public class AssignedRoleView
    {
        public string Id { get; internal set; }
        public string RoleId { get; internal set; }
        public string Role { get; internal set; }
        public string User { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string Email { get; internal set; }
        public string UserName { get; internal set; }
        public string OrganizationType { get; internal set; }
    }
}