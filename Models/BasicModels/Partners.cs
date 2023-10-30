using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
//using System.Web.Mvc;

namespace Devdiscourse.Models.BasicModels
{
    public class Partners
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Description")]
        //[AllowHtml]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public PartnerType Type { get; set; }
        public string SubType { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string Creator { get; set; }
        public Partners()
        {
            if (CreatedOn == new DateTime())
            {
                CreatedOn = DateTime.UtcNow;
            }
        }
    }
}