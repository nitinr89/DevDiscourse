﻿using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class ShowCategoryViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public ShowCategoryViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var idList = id.Split(',').ToList();
            var search = _db.Categories.Where(a => idList.Contains(a.Id.ToString())).ToList();
            return View(search);
        }
    }
}
