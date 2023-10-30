using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class TeamViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public TeamViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var search = _db.Teams.Where(a => a.Active == true).OrderBy(a => a.SrNo).ToList();
            return View(search);
        }
    }
}
