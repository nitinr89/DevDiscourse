using Devdiscourse.Models.BasicModels;
using System.ComponentModel.DataAnnotations;
using Devdiscourse.Models.ResearchModels;
using Devdiscourse.Models.Others;
using Devdiscourse.Models.ContributorModels;
using Devdiscourse.Models.VideoNewsModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Devdiscourse.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfilePic { get; set; }
        [Display(Name = "About Me")]
        public string AboutMe { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        // Company Details
        public string CompanyName { get; set; }
        public string OrganizationType { get; set; }
        public string Position { get; set; }    // Company Position
        public DateTime CreatedOn { get; set; }
        public string DigitCode { get; set; }
        public bool isActive { get; set; }
        public bool isPRManager { get; set; }
        // Foreign Key Members
        public virtual ICollection<DevNews> DevNews { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<DevResearch> DevResearches { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
        public virtual ICollection<UserInterest> UserInterests { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public virtual ICollection<SDGSamurai> SDGSamurais { get; set; }
        public virtual ICollection<ResponseReport> ResponseReports { get; set; }
        public virtual ICollection<UserBehaviour> UserBehaviours { get; set; }
        public virtual ICollection<UserComment> UserComments { get; set; }
        public virtual ICollection<Content> NewsContents { get; set; }
        public virtual ICollection<NewsWireModel> NewsWire { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Earnings> Earnings { get; set; }
        public virtual ICollection<PaymentHistory> PaymentHistory { get; set; }
        public virtual ICollection<DiscourseComment> DiscourseComment { get; set; }
        public virtual ICollection<Livediscourse> Livediscourse { get; set; }
        public virtual ICollection<DiscourseTopic> DiscourseTopic { get; set; }
        public virtual ICollection<FollowTag> FollowTag { get; set; }

        public virtual ICollection<FollowLivediscourse> FollowLivediscourse { get; set; }
        public virtual ICollection<React> React { get; set; }
        public virtual ICollection<UserNewsLabel> UserNewsLabel { get; set; }
        public virtual ICollection<Infocus> Infocus { get; set; }
        public virtual ICollection<LiveDiscourseInfocus> LiveDiscourseInfocus { get; set; }
        public virtual ICollection<VideoNews> VideoNews { get; set; }
        public virtual ICollection<MediaInternship> MediaInternships { get; set; }
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateAsync(this, "");
        //    // Add custom user claims here
        //    return userIdentity;
        //}
        public ApplicationUser()
        {
            if (CreatedOn == new DateTime()) CreatedOn = DateTime.UtcNow;
        }
    }
}