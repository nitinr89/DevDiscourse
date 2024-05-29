using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class OpinionBlogInterviewViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public OpinionBlogInterviewViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            DateTime oneMonth = DateTime.Today.AddDays(-30);
            if (reg == "Global Edition")
            {
                var search = await (from a in _db.DevNews
                                    where a.CreatedOn > oneMonth && a.AdminCheck == true && a.Type == "Blog"
                                    orderby a.PublishedOn descending
                                    select new NewsViewModel
                                    {
                                        Title = a.Title,
                                        CreatedOn = a.PublishedOn,
                                        ImageUrl = a.ImageUrl,
                                        SubType = a.Themes,
                                        Subtitle = a.SubTitle,
                                        Country = a.Author,
                                        NewsId = a.NewsId,
                                        Label = a.NewsLabels
                                    }).Take(4).ToListAsync();
                return View(search);
            }
            else
            {
                var search = await (from a in _db.DevNews
                                    where a.CreatedOn > oneMonth && a.AdminCheck == true && a.Type == "Blog"
                                     && a.Region != null && a.Region.ToLower().Contains(reg.ToLower())
                                    orderby a.PublishedOn descending
                                    select new NewsViewModel
                                    {
                                        Title = a.Title,
                                        CreatedOn = a.PublishedOn,
                                        ImageUrl = a.ImageUrl,
                                        SubType = a.Themes,
                                        Subtitle = a.SubTitle,
                                        Country = a.Author,
                                        NewsId = a.NewsId,
                                        Label = a.NewsLabels
                                    }).Take(4).ToListAsync();
                return View(search);
            }
        }
    }
}
