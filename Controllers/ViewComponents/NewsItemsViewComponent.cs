using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class NewsItemsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public NewsItemsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sector, string region, string country, string tag, string cat, string label, int? page)
        {
            try
            {
                await Task.Yield();
                cat = cat ?? "";
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                var resultList = (from dn in _db.DevNews.AsNoTracking()
                                  where dn.AdminCheck == true && dn.NewsLabels == label && dn.IsSponsored == false
                                  select new NewsViewModel
                                  {
                                      NewsId = dn.NewsId,
                                      Title = dn.Title,
                                      ImageUrl = dn.ImageUrl,
                                      CreatedOn = dn.ModifiedOn,
                                      Subtitle = dn.SubTitle,
                                      Sector = dn.Sector,
                                      SubType = dn.SubType,
                                      Country = dn.Country,
                                      Label = dn.NewsLabels,
                                      Ranking = 0
                                  }).OrderByDescending(a => a.CreatedOn)
               .Take(65)
               .ToList();
                return View(resultList.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
