using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class Career
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        [Required]
        public string Qualification { get; set; }
        [DisplayName("Current Full Time Employment")]
        public string CurrentEmployment { get; set; }
        [Required]
        [DisplayName("Area Of Expertise")]
        public string AreaOfExpertise { get; set; }
        [DisplayName("Upload CV")]
        public string UploadCV { get; set; }
        public string FileExtension { get; set; }
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Career()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
            if (ModifiedOn == new DateTime())
            {
                ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}