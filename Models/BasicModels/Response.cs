using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class Response : BaseClass
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
        public Guid ItemId { get; set; }
        public string ItemType { get; set; }
        public bool IsLike { get; set; }
        public string Creator { get; set; }
        [ForeignKey("Creator")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
    }
}