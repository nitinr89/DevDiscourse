﻿using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class InterviewViewComponent :ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public InterviewViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            try
            {
                await Task.Yield();
                var regs = (from c in _db.Countries join r in _db.Regions on c.RegionId equals r.Id  where c.Title == reg select new  { r.Title }).FirstOrDefault();
                string regionTitle = string.Empty;
                var region = string.Empty;
                if(reg=="")
                { 
                    region = "Global Edition";
                }
                else 
                { 
                    region = regs != null && regs.Title != null ? regionTitle = regs.Title : regionTitle = reg; 
                }
              
                DateTime thirtyDays = DateTime.Today.AddDays(-30);
                if (region == "Global Edition")
                {
                    var result = (from m in _db.Infocus
                                  where m.Edition == "Universal Edition" && m.ItemType == "Interview"
                                  join a in _db.DevNews on m.NewsId equals a.NewsId
                                  where a.AdminCheck == true
                                  select new NewsViewModel
                                  {
                                      Title = a.Title,
                                      ImageUrl = a.ImageUrl,
                                      NewsId = a.NewsId,
                                      Label = a.NewsLabels,
                                      Country = a.Country,
                                      SrNo = m.SrNo
                                  }).OrderBy(a => a.SrNo).AsNoTracking().Take(4);
                    return View( result.ToList());
                }
                else
                {
                    var result = (from m in _db.Infocus
                                  where m.Edition == region && m.ItemType == "Interview"
                                  join a in _db.DevNews on m.NewsId equals a.NewsId
                                  where a.AdminCheck == true
                                  select new NewsViewModel
                                  {
                                      Title = a.Title,
                                      ImageUrl = a.ImageUrl,
                                      NewsId = a.NewsId,
                                      Label = a.NewsLabels,
                                      Country = a.Country,
                                      SrNo = m.SrNo
                                  }).OrderBy(a => a.SrNo).AsNoTracking().Take(4);
                    return View(result.ToList());
                }
            }catch (Exception ex)
            {
                return Content("Internal Server Error: " + ex.Message);
            }
        }
    }
}
