using Devdiscourse.Models.Others;

namespace Devdiscourse.Models.ViewModel
{
    public class ShiftUser
    {
        
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool isActice { get; set; }
        public ICollection<UserNewsLabel> NewsLabels { get; internal set; }
    }
}