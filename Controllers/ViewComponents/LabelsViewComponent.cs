using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class LabelsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public LabelsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                await Task.Yield();
                var search = _db.Labels.ToList();
                return View(search);
            }catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
