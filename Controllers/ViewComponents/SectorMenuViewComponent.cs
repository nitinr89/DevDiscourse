﻿
using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorMenuViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public SectorMenuViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string filter = "", string reg = "Global Edition")
        {
            await Task.Yield();
            ViewBag.reg = reg;
            ViewBag.filter = filter;
            var search = _db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.Title).ToList();
            return View(search);
        }

    }
}
