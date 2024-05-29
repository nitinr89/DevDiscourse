using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public SectorViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sector, string reg = "", string filter = "")
        {
            try
            {
                ViewBag.reg = reg;
                ViewBag.filter = filter;
                if (!string.IsNullOrEmpty(sector))
                {
                    List<int> idList = new();
                    var stringidList = sector.Split(',');
                    foreach (var id in stringidList)
                    {
                        bool result = int.TryParse(id, out int number);
                        if (result) idList.Add(number);
                    }
                    var devSectors = await _db.DevSectors
                                        .Where(m => m.Id != 8 && m.Id != 16)
                                        .ToListAsync();

                    var finteredSector = devSectors.Where(m => idList.Contains(m.Id))
                         .Select(m => new ItemView
                         {
                             Id = m.Id,
                             Title = m.Title ?? "",
                         }).ToList();
                    if (filter == "Single")
                    {
                        finteredSector = finteredSector.Take(1).ToList();
                    }
                    return View(finteredSector.OrderBy(a => a.Title));
                }
                return View();
            }
            catch (Exception _)
            {
                return View(new List<ItemView> { new() { Id = 0, Title = "" } });
            }
        }
    }
}
