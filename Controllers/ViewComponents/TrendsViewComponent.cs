using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class TrendsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public TrendsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? id, string? reg, string filter = "")
        {
            try
            {
                ViewBag.filter = filter;
                DateTime today = DateTime.Today;
                DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
                reg ??= "Global Edition";
                if (reg == "Global Edition")
                {
                    var resultList = await (from dn in _db.DevNews
                                            where dn.CreatedOn > weekStart && dn.AdminCheck == true
                                            && dn.Id != id && dn.Sector == "14"
                                            orderby dn.ViewCount descending
                                            select new LatestNewsView
                                            {
                                                Id = dn.Id,
                                                NewId = dn.NewsId,
                                                Title = dn.Title,
                                                ImageUrl = dn.ImageUrl,
                                                CreatedOn = dn.ModifiedOn,
                                                Type = dn.Type,
                                                SubType = dn.SubType,
                                                Country = dn.Country,
                                                Label = dn.NewsLabels,
                                                Ranking = 0
                                            }).Take(4).ToListAsync();
                    return View(resultList);
                }
                else
                {
                    var resultList = await (from dn in _db.DevNews
                                            where dn.CreatedOn > weekStart && dn.AdminCheck == true
                                            && dn.Id != id && dn.Sector == "14"
                                            && dn.Region != null && dn.Region.ToLower().Contains(reg.ToLower())
                                            orderby dn.ViewCount descending
                                            select new LatestNewsView
                                            {
                                                Id = dn.Id,
                                                NewId = dn.NewsId,
                                                Title = dn.Title,
                                                ImageUrl = dn.ImageUrl,
                                                CreatedOn = dn.ModifiedOn,
                                                Type = dn.Type,
                                                SubType = dn.SubType,
                                                Country = dn.Country,
                                                Label = dn.NewsLabels,
                                                Ranking = 0
                                            }).Take(4).ToListAsync();
                    return View(resultList);
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
