using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ContributorModels
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        // Billing Address
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        public string Contact { get; set; }
        [Required]
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }
        // Bank Details
        [Display(Name = "A/c Holder Name")]
        [Required]
        public string AccountHolderName { get; set; }
        [Display(Name = "Bank Name")]
        [Required]
        public string BankName { get; set; }
        [Display(Name = "Bank Address")]
        [Required]
        public string BankAddress { get; set; }
        [Display(Name = "Bank City")]
        [Required]
        public string BankCity { get; set; }
        [Display(Name = "Bank State")]
        [Required]
        public string BankState { get; set; }
        [Display(Name = "Bank Country")]
        [Required]
        public string BankCountry { get; set; }
        [Display(Name = "PinCode")]
        public string BankPincode { get; set; }
        [Display(Name = "Account Number")]
        [Required]
        public string AccountNo { get; set; }
        [Display(Name = "Account Type")]
        [Required]
        public string AccountType { get; set; }
        [Display(Name = "IFSC Code")]
        [Required]
        public string IFSCCode { get; set; }
        [Display(Name = "Pancard Number")]
        public string PanCardNo { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public Payment()
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