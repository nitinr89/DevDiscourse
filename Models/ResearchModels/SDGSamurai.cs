using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.ResearchModels
{
    public class SDGSamurai : BaseClass
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
        public string Profession { get; set; }
        [Display(Name = "About us")]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Display(Name = "Country")]
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string SDGCode { get; set; }
        public string ReferenceCode { get; set; }
        [Display(Name = "SDG Rank")]
        public int SDGPosition { get; set; }
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$")]
        public double SDGPoints { get; set; }
        public bool IsActive { get; set; }
        public string Creator { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        [ForeignKey("SDGPosition")]
        public virtual Rank Ranks { get; set; }
    }
}