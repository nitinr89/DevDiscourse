using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class Team : BaseClass
    {
        public override Guid Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }
        public int SrNo { get; set; }
        [Required]
        [Display(Name = "Member Name")]
        public string TeamMember { get; set; }
        [Required]
        public string Designation { get; set; }
        [Required]
        public TeamType Type { get; set; }
        [Display(Name = "About Me")]
        public string? AboutMe { get; set; }
        [Required]
        [Display(Name = "Profile Image")]
        public string ProfilePic { get; set; } = "/AdminFiles/TeamProfile/user.jpg";
        public bool Active { get; set; }
    }
}