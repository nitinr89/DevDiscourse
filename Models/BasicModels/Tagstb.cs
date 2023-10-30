using Devdiscourse.Models.VideoNewsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class Tagstb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name ="Tag Title")]
        public string TagTitle { get; set; }
        public virtual ICollection<NewsTagstb> NewsTagstb { get; set; }

        public virtual ICollection<VideoNewsTag> VideoNewsTags { get; set; }

    }
}