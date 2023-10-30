using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.Others
{
    public class EventNavLink
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Label { get; set; }
        [Required]
        [Display(Name ="Link URL")]
        public string Href { get; set; }
        [Required]
        public string Title { get; set; }

        public long EventId { get; set; }

        [ForeignKey("EventId")]

        public virtual CommonEvent CommonEvents { get; set; }
    }
}