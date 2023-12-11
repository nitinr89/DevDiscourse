using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class BlogItemsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public BlogItemsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? page, string type, string region = "Global Edition")
        {
            await Task.Yield();
            DateTime LastSixMonth = DateTime.UtcNow.AddMonths(-24);
            //DateTime LastSixMonth = DateTime.UtcNow.AddDays(-30);
            //if (Request.IsAjaxRequest())
            //{
            //    LastSixMonth = DateTime.UtcNow.AddMonths(-60);
            //}
            var search = _db.DevNews
                //Where(a => a.Type == "Blog" && a.CreatedOn > LastSixMonth && a.AdminCheck == true)
                .Select(a => new AdvancedSearchView { 
                    Id = a.Id,
                    NewsId = a.NewsId, 
                    Title = a.Title,
                    SubType = a.SubType, 
                    ImageUrl = a.ImageUrl,
                    Region = a.Region,
                    IsGlobal = a.IsGlobal, 
                    CreatedOn = a.PublishedOn, 
                    Label = a.NewsLabels, 
                    Country = a.Author }).Take(50).ToList();//.OrderByDescending(m => m.CreatedOn).Take(10).ToList();
            if (region != "Global Edition")
            {
                search = search
                    //Where(a => a.Region != null && a.Region.Contains(region))
                       .Take(50).ToList();
            }
            if (!string.IsNullOrEmpty(type))
            {
                search = search
                    //.Where(a => string.Equals(a.SubType, type, StringComparison.OrdinalIgnoreCase))
                    .Take(50).ToList();
            }
            else
            {
                search = search
                    //Where(a => !string.Equals(a.SubType, "interview", StringComparison.OrdinalIgnoreCase))
                     .Take(50).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(search.ToPagedList(pageNumber, pageSize));
        }
    }
}
