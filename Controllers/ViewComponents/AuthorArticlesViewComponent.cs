using Devdiscourse.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class AuthorArticlesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public AuthorArticlesViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id, string region, int? page)
        {
            var result = _db.DevNews.Where(a => a.AdminCheck == true && a.Author == id).OrderByDescending(m => m.CreatedOn).AsNoTracking();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(result.ToPagedList(pageNumber, pageSize));
        }
    }
}
