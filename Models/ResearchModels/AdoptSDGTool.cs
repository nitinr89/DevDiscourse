using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.ResearchModels
{
    public class AdoptSDGTool : BaseClass
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
        public string Name { get; set; }
        [Required]
        [Display(Name = "Institution")]
        public string Institution { get; set; }
        [Required]
        [Display(Name = "Location of your institution")]
        public string Location { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Display(Name = "Phone")]
        public string Contact { get; set; }
        [Display(Name = "Data is needed for")]
        public string NeededData { get; set; }
        [Display(Name = "Geographical for which data required")]
        public string GeographicalData { get; set; }
        [Display(Name = "Data is needed for themetic area")]
        public string ThemeticArea { get; set; }
        [Display(Name = "Other Message")]
        public string Message { get; set; }
        [Display(Name = "Approved")]
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }
}