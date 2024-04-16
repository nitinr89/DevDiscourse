

using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

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
            await Task.Yield();
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
                    var filteredItems = _db.DevSectors
                                        .Where(m => m.Id != 8 && m.Id != 16)
                                        .ToList() // Retrieve data from the database
                                        .Where(m => idList.Contains(m.Id)) // Perform in-memory join
                                        .Select(m => new ItemView
                                        {
                                            Id = m.Id,
                                            Title = m.Title,
                                        });

                    if (filter == "Single")
                    {
                        filteredItems = filteredItems.Take(1);
                    }

                    return View(filteredItems.OrderBy(a => a.Title));
                }
                return View();
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
