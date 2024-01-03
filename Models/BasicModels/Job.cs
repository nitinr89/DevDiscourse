using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Job Title")]
        [Required]
        public string Title { get; set; }
        public string? OrganizationName { get; set; }
        [Display(Name = "Job Location")]
        [Required]
        public string Location { get; set; }
        [Required]
        [Display(Name = "About Position")]
        //[AllowHtml]
        public string? AboutPosition { get; set; }
        [Display(Name = "Email")]
        public string? EmailId { get; set; }
        public bool? IsPublished { get; set; }
        public string? PostedByUser { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Opening Date")]
        public DateTime? OpeningDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Closing Date")]
        public DateTime? ClosingDate { get; set; }
        public string? Keywords { get; set; }
        [Display(Name = "Min CTC")]
        public long? MinCTC { get; set; }
        [Display(Name = "Max CTC")]
        public long? MaxCTC { get; set; }
        public string? JobCTCType { get; set; }
        public string? CTCCurrency { get; set; }
        [Display(Name = "No. of Vacancies")]
        public int? Vacancy { get; set; }
        [Display(Name = "Min Experience")]
        public int? MinExperience { get; set; }
        [Display(Name = "Max Experience")]
        public int? MaxExperience { get; set; }
        public int? ViewCount { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Job()
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