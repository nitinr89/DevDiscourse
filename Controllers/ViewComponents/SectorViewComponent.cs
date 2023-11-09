﻿

using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public SectorViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sector, string reg = "", string filter = "")
        {
            await Task.Yield();
            ViewBag.reg = reg;
            ViewBag.filter = filter;
            if (!string.IsNullOrEmpty(sector))
            {
                var idList = sector.Split(',').ToList().Select(int.Parse).ToList();
                var search = from m in _db.DevSectors
                             where m.Id != 8 && m.Id != 16
                             join s in idList on m.Id equals s
                             select new ItemView
                             {
                                 Id = m.Id,
                                 Title = m.Title,
                             };
                if (filter == "Single")
                {
                    search = search.Take(1);
                }
                return View(search.OrderBy(a => a.Title));
            }
            return View();
        }
    }
}
