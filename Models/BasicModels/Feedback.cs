using System;

namespace Devdiscourse.Models.BasicModels
{
    public class Feedback : BaseClass
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
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}