using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ContributorModels
{
    public class PaymentHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }
        [Required]
        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }
        public int WithdrawAmount { get; set; }
        public CurrencyType Currency { get; set; }
        public string Slug { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public PaymentHistory()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}