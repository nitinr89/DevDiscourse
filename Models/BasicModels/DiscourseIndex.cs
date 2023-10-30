using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class DiscourseIndex
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        public long LivediscourseId { get; set; }
        [ForeignKey("LivediscourseId")]
        public virtual Livediscourse Livediscourse { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}