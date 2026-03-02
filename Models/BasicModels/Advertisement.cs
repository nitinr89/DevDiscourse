using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class Advertisement : BaseClass
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
        [Required]
        [Display(Name = "Name")]
        public string Advertisor { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        [Display(Name = "Advertisement Details")]
        public string Description { get; set; }
        public string Phone { get; set; }
    }
}