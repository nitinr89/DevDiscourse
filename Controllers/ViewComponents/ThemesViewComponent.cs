using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class ThemesViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ThemesViewComponent(ApplicationDbContext db)
        {
            _db= db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string theme , string filter="")
        {
            try
            {
                await Task.Yield();
                ViewBag.filter = filter;
                var idList = theme.Split(',').ToList().Select(int.Parse).ToList();
                var search = from m in _db.DevThemes
                             join s in idList on m.Id equals s
                             select new ItemView
                             {
                                 Id = m.Id,
                                 Title = m.Title,
                             };
                return View(search);
            }catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
