using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorMostPopularViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SectorMostPopularViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sector, string reg = "Global Edition")
        {
            try
            {
                var sectorText = sector.ToString();
                DateTime recentWindow = DateTime.Today.AddDays(-4);

                var query = _db.DevNews
                    .AsNoTracking()
                    .Where(a =>
                        a.AdminCheck &&
                        !a.IsSponsored &&
                        a.CreatedOn > recentWindow &&
                        a.Sector == sectorText);

                if (!string.Equals(reg, "Global Edition", StringComparison.OrdinalIgnoreCase))
                {
                    var regionNewsIds = _db.RegionNewsRankings
                        .AsNoTracking()
                        .Where(r => r.Region.Title == reg)
                        .Select(r => r.NewsId);

                    query = query.Where(a => regionNewsIds.Contains(a.Id));
                }

                var resultList = await query
                    .OrderByDescending(a => a.ViewCount)
                    .ThenByDescending(a => a.CreatedOn)
                    .Take(3)
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
                    })
                    .ToListAsync();

                return View(resultList);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
