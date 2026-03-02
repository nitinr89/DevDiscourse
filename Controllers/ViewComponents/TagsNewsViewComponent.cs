using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class TagsNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TagsNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string tag, string reg = "Global Edition")
        {
            await Task.Yield();
            try { 
            var userAgent = Request.Headers["User-Agent"].ToString();
            DateTime oneMonth = DateTime.Today;
           // if (Request.Browser.Crawler)
           if(IsCrawler(userAgent))
            {
                oneMonth = oneMonth.AddDays(-3);
            }
            else
            {
                oneMonth = oneMonth.AddDays(-10);
            }
                if (reg == "Global Edition")
                {
                    var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > oneMonth && a.Tags.Contains(tag))
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
                            Source = a.Source
                        }).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
                    return View(resultList);
                }
                else
                {
                    var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.CreatedOn > oneMonth && a.Tags.Contains(tag))
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
                            Source = a.Source
                        }).OrderByDescending(m => m.CreatedOn).Take(20).AsNoTracking();
                    return View(resultList);
                }
            }catch(Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        private bool IsCrawler(string userAgent)
        {
            // Your crawler detection logic here
            // For example, check if the user agent contains "bot"
            return userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase);
        }
    }
}
