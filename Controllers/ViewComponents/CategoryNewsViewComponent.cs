using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class CategoryNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public CategoryNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int category)
        {
            try
            {
                DateTime todayDate = DateTime.Today.AddDays(3).AddTicks(-1);
                var search = _db.DevNews
                    .Where(a => a.AdminCheck == true && a.CreatedOn > todayDate && (a.Category == category.ToString()))
                    .OrderByDescending(a => a.CreatedOn).Select(a => new SearchView
                    {
                        Title = a.Title,
                        CreatedOn = a.ModifiedOn,
                        Country = a.Country,
                        ImageUrl = a.ImageUrl,
                        NewsId = a.NewsId,
                        Sector = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
                    })
                    .OrderByDescending(m => m.CreatedOn).Take(20).ToList();
                return View(search.ToList());
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
    }
}
