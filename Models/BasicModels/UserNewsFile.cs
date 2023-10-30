using Devdiscourse.Models.BasicModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models
{
    public class UserNewsFile : BaseClass
    {
        public override Guid Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }

        public string Title { get; set; }

        public string FilePath { get; set; }

        public string FileMimeType { get; set; }

        public string FileSize { get; set; }

        public string FileType { get; set; }

        public string FileCaption { get; set; }
        public string FileThumbUrl { get; set; }
        public string Duration{get; set; }

        public string Source { get; set; }

        public Guid NewsId { get; set; }
        [ForeignKey("NewsId")]
        public virtual DevNews DevNews { get; set; }

    }
}