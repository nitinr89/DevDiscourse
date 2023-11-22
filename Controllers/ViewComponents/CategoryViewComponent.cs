﻿using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public CategoryViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var search = _db.Categories.ToList().OrderBy(a => a.SrNo);
            return View(search);
        }
    }
}
