﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Devdiscourse.Models.BasicModels
{
    public class React
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string ReactBy { get; set; }
        public DateTime ReactOn { get; set; }
        public ReactType ReactType { get; set; }
        public ReactItemType ReactItemType { get; set; }
        public long ItemId { get; set; }
        [ForeignKey("ReactBy")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
    }
}