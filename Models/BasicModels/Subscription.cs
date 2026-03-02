using System;
using System.ComponentModel.DataAnnotations;

namespace Devdiscourse.Models.BasicModels
{
    public class Subscription : BaseClass
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
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int Type { get; set; }   // 0 : Mobile,  1 : Desktop
    }
}