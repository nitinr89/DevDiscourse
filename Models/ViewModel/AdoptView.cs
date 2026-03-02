using System;

namespace Devdiscourse.Models.ViewModel
{
    public class AdoptView
    {
        public string Name { get; internal set; }
        public string Organisation { get; internal set; }
        public string AboutUs { get; internal set; }
        public string ProfilePic { get; internal set; }
        public string Email { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
    }
}