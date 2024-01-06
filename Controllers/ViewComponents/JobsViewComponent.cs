using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class JobsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public JobsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {


                var search = await _db.Jobs.Where(a => a.IsPublished == true).OrderByDescending(a => a.CreatedOn).ToListAsync();
                return View(search);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
