using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class DiscourseTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
       
        [DisplayName("Sector")]
        public int SectorId { get; set; }
        
        public long ParentId { get; set; }
        [ForeignKey("SectorId")]
        public virtual DevSector? DevSector { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<FollowTag>? FollowTags { get; set; }
    }
}