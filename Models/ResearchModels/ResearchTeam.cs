using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.ResearchModels
{
    public class ResearchTeam : BaseClass
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
        [Display(Name = "About Me")]
        public string AboutMe { get; set; }
        [Required]
        [Display(Name = "Profile Image")]
        public string ProfilePic { get; set; }
        public bool Active { get; set; }
    }
}