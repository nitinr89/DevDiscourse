using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SectorNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sector, string reg = "Global Edition")
        {
            try
            {
                var sectorText = sector.ToString();

                var resultList = await _db.RegionNewsRankings
                    .AsNoTracking()
                    .Where(dn =>
                        dn.Region.Title == reg &&
                        dn.DevNews.AdminCheck &&
                        !dn.DevNews.IsSponsored &&
                        dn.DevNews.Sector == sectorText)
                    .OrderByDescending(dn => dn.DevNews.CreatedOn)
                    .ThenByDescending(dn => dn.Ranking)
                    .Take(20)
                    .Select(dn => new NewsViewModel
                    {
                        NewsId = dn.DevNews.NewsId,
                        Title = dn.DevNews.Title,
                        ImageUrl = dn.DevNews.ImageUrl,
                        CreatedOn = dn.DevNews.ModifiedOn,
                        Subtitle = dn.DevNews.SubTitle,
                        SubType = dn.DevNews.SubType,
                        Country = dn.DevNews.Country,
                        Sector = dn.DevNews.Sector,
                        Label = dn.DevNews.NewsLabels,
                        Ranking = dn.Ranking
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
