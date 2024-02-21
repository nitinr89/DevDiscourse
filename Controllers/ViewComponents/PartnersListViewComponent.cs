using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class PartnersListViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public PartnersListViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(PartnerType type, string subType, int skip = 0)
        {
            try
            {
                await Task.Yield();
                ViewBag.type = type;
                var search = _db.Partners.Where(a => a.Type == type && a.SubType.ToUpper() == subType.ToUpper() && a.IsActive == true)
                    .OrderByDescending(a => a.CreatedOn).Skip(skip).Take(12).ToList();
                return View(search);
            }catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
