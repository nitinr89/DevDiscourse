using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public required int Id { get; set; }
        public int SrNo { get; set; }
        public string? Title { get; set; }
        //[Required]
        public string? Slug { get; set; }
       // [Required]
        public  bool IsActive { get; set; }
        [Display(Name = "Page Title")]
        public string? PageTitle { get; set; }
        [Display(Name = "Page Image")]
        public string? PageImage { get; set; }
        [Display(Name = "Banner Image")]
        public  string? BannerImage { get; set; }
        [Display(Name = "Page Description")]
        public string? PageDescription { get; set; }
        public string? Keywords { get; set; }
    }
}