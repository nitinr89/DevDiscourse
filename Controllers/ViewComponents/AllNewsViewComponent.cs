using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class AllNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public AllNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg="Global Edition")
        {
            await Task.Yield();
            var resultList = _db.DevNews.Where(a => a.AdminCheck == true)
                .Select(a => new SearchView 
                { 
                    Id = a.Id,
                    NewsId = a.NewsId, 
                    Title = a.Title, 
                    ImageUrl = a.ImageUrl, 
                    Country = a.Country, 
                    CreatedOn = a.CreatedOn, 
                    Region = a.Region, 
                    Sector = a.Sector, 
                    IsGlobal = a.IsGlobal, 
                    IsVideo = a.IsVideo, 
                    IsSponsored = a.IsSponsored, 
                    EditorPick = a.EditorPick, 
                    Tags = a.Tags, 
                    Type = a.Type, 
                    SubType = a.SubType, 
                    Category = a.Category, 
                    Label = a.NewsLabels, 
                    Source = a.Source }).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
            return View( resultList);
        }
    }
}
