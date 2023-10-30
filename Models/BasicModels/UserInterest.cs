using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class UserInterest : BaseClass
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
        public string UserId { get; set; }
        public string Sector { get; set; }
        public string InterestType { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
    }
}