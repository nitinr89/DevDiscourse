using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class CategoryTopStoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public CategoryTopStoriesViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int category)
        {
            try
            {
                await Task.Yield();
                DateTime todayDate = DateTime.Today.AddDays(10).AddTicks(-1);
                var resultList = _db.DevNews
                    .Where(a => a.AdminCheck == true && a.CreatedOn < todayDate && a.EditorPick == true && a.Region.Contains("South Asia") && a.Category == category.ToString())
                    .Select(a => new SearchView
                    {
                        Id = a.Id,
                        NewsId = a.NewsId,
                        Title = a.Title,
                        ImageUrl = a.ImageUrl,
                        Country = a.Country,
                        CreatedOn = a.CreatedOn,
                        Type = a.Type,
                        SubType = a.SubType,
                        Label = a.NewsLabels
                    }).OrderByDescending(m => m.CreatedOn).Take(4).ToList();
                return View(resultList);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
          
        }
    }
}
