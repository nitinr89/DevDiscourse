using System;

namespace Devdiscourse.Models.ViewModel
{
    public class SamuraiView
    {
        public string Name { get; internal set; }
        public string Profession { get; internal set; }
        public string AboutUs { get; internal set; }
        public string ProfilePic { get; internal set; }
        public string Gender { get; internal set; }
        public string Rank { get; internal set; }
        public double SDGPoints { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
        public string UserId { get; internal set; }
    }
}