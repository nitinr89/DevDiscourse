using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class BlogItemsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public BlogItemsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? page, string type, string region = "Global Edition")
        {
            await Task.Yield();
            DateTime LastSixMonth = DateTime.UtcNow.AddMonths(-24);
            //if (Request.IsAjaxRequest())
            //{
            //    LastSixMonth = DateTime.UtcNow.AddMonths(-60);
            //}
            try
            {
                var search = _db.DevNews.
                    Where(a => a.Type == "Blog" && a.CreatedOn > LastSixMonth && a.AdminCheck == true)
                    .Select(a => new AdvancedSearchView
                    {
                        Id = a.Id,
                        NewsId = a.NewsId,
                        Title = a.Title,
                        SubType = a.SubType,
                        ImageUrl = a.ImageUrl,
                        Region = a.Region,
                        IsGlobal = a.IsGlobal,
                        CreatedOn = a.PublishedOn,
                        Label = a.NewsLabels,
                        Country = a.Author
                    }).Take(50).ToList().OrderByDescending(m => m.CreatedOn).Take(50).ToList();
                if (region != "Global Edition")
                {

                    var reg = (from c in _db.Countries
                               join r in _db.Regions on c.RegionId equals r.Id
                               where c.Title == region
                               select new
                               {
                                   r.Title
                               }).FirstOrDefault();
                    string regionTitle = "Global Edition";

                    //if (reg != null && reg.Title != null) { regionTitle = reg.Title; region = regionTitle; }
                    var result = reg != null && reg.Title != null ? regionTitle = reg.Title : regionTitle = region;

                    search = search.Take(50)
                        .Where(a => a.Region != null && a.Region.Contains(result))
                           .ToList();
                }
                if (!string.IsNullOrEmpty(type))
                {
                    search = search.Take(50)
                        .Where(a => string.Equals(a.SubType, type, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else
                {
                    search = search.Take(50)
                        .Where(a => !string.Equals(a.SubType, "interview", StringComparison.OrdinalIgnoreCase))
                         .ToList();
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(search.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                return Content("Internel Server Error 500: " + ex.Message);
            }
        }
    }
}
