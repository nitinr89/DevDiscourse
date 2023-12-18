using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.Others
{
    public class CampaignPetition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CampaignPetitionId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PledgeType { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}