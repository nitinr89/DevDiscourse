
using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class RegionViewComponent : ViewComponent
    {  
        private ApplicationDbContext _db;
        public RegionViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string filter = "")
        {
            await Task.Yield();
            try
            {
                ViewBag.filter = filter;
                var search = _db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").ToList().OrderBy(a => a.SrNo);
                return View(search);
            } catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
