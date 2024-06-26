﻿using Devdiscourse.Models.BasicModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.VideoNewsModels
{
    public class VideoNewsRegion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long VideoNewsId { get; set; }
        public Guid EditionId { get; set; }
        [ForeignKey("EditionId")]
        public virtual Region? Edition { get; set; }

        [ForeignKey("VideoNewsId")]
        public virtual VideoNews? VideoNews { get; set; }
    }
}