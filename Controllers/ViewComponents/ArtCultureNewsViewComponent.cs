﻿using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class ArtCultureNewsViewComponent : ViewComponent    {
        private readonly ApplicationDbContext _db;

        public ArtCultureNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            ViewBag.Sector = "14";
            DateTime twoMonth = DateTime.UtcNow.AddDays(-120);

            //var result = _db.RegionNewsRankings.Where(a => a.DevNews.CreatedOn > twoMonth && a.DevNews.AdminCheck == true && a.DevNews.Sector == "14" && a.Region.Title == reg && a.DevNews.IsSponsored == false).OrderByDescending(a => a.DevNews.CreatedOn).Select(a => new NewsViewModel
            //{
            //    Title = a.DevNews.Title,
            //    NewsId = a.DevNews.NewsId,
            //    ImageUrl = a.DevNews.ImageUrl,
            //    Subtitle = a.DevNews.SubTitle,
            //    Country = a.DevNews.Country,
            //    CreatedOn = a.DevNews.ModifiedOn,
            //    Sector = a.DevNews.Type,
            //    SubType = a.DevNews.SubType,
            //    Label = a.DevNews.NewsLabels,
            //    Ranking = a.Ranking
            //}).AsNoTracking().Take(30).ToList();
            //return View(result.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(a => a.Ranking).Take(6));
            //var infocus = (from dn in _db.DevNews
            //               select new NewsViewModel
            //               {
            //                  // Id = dn.Id,
            //                   NewsId = dn.NewsId,
            //                   Title = dn.Title,
            //                   ImageUrl = dn.ImageUrl,
            //                   CreatedOn = dn.ModifiedOn,
            //                   //Type = dn.Type,
            //                   SubType = dn.SubType,
            //                   Country = dn.Country,
            //                   Label = dn.NewsLabels,
            //                   Ranking = 0
            //               }).OrderByDescending(a => a.CreatedOn)
            //    .Take(65)
            //    .ToList();
            //return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());


            var resultList = _db.DevNews
.Where(dn => dn.AdminCheck == true &&
            dn.CreatedOn > twoMonth &&
            dn.Sector == "14")
.OrderByDescending(dn => dn.CreatedOn)
.Take(65)
.Select(dn => new NewsViewModel
{
   NewsId = dn.NewsId,
   Title = dn.Title,
   ImageUrl = dn.ImageUrl,
   CreatedOn = dn.ModifiedOn,
   Subtitle = dn.SubTitle,
   SubType = dn.SubType,
   Country = dn.Country,
   Sector = dn.Sector,
   Label = dn.NewsLabels,
   Ranking = 0
})
.ToList();

            var groupedResult = resultList
                .GroupBy(s => s.Title)
                .Select(group => group.OrderByDescending(a => a.Ranking).FirstOrDefault())
                .OrderByDescending(o => o.Ranking)
                .Take(30)
                .ToList();

            return View(groupedResult);
        }
    }
}
