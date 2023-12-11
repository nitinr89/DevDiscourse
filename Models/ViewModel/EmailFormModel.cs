using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.ViewModel
{
    public class EmailFormModel
    {
        [Required, Display(Name = "Name")]
        public string? FromName { get; set; }
        [Required, Display(Name = "Email"), EmailAddress]
        public string? FromEmail { get; set; }
        [Required]
        public string? Message { get; set; }
    }
}