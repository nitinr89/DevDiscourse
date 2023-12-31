﻿using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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
            await Task.Yield();
             DateTime weekend = DateTime.Today.AddDays(-121);
            var resultList = _db.DevNews.AsNoTracking()
              //  .Where(a => a.AdminCheck == true && a.CreatedOn > weekend && a.Region == reg && a.Sector == Convert.ToString(sector)).OrderByDescending(o => o.ViewCount)
              .Where(a => a.AdminCheck == true && a.CreatedOn > weekend && a.Sector == Convert.ToString(sector))
                .Select(a =>
            new SearchView
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
            }).Take(3).ToList();
            return View(resultList);

        }
    }
}
