﻿using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;

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
            await Task.Yield();
            try
            {
                var region = (from c in _db.Countries
                              join r in _db.Regions on c.RegionId equals r.Id
                              where c.Title == reg
                              select new
                              {
                                  r.Title
                              }).FirstOrDefault();
                string regionTitle = "Global Edition";

                if (region != null && region.Title != null) regionTitle = region.Title;
                var resultList = _db.RegionNewsRankings
    .Where(dn => dn.DevNews.AdminCheck == true && dn.DevNews.Sector == Convert.ToString(sector) && dn.Region.Title==regionTitle && dn.DevNews.IsSponsored == false)
    .OrderByDescending(dn => dn.DevNews.CreatedOn)
    .Take(65)
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
        Ranking = 0
    })
    .ToList();

                var groupedResult = resultList
                    .GroupBy(s => s.Title)
                    .Select(group => group.OrderByDescending(a => a.Ranking).FirstOrDefault())
                    .OrderByDescending(o => o.Ranking)
                    .Take(20)
                    .ToList();

                return View(groupedResult);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
