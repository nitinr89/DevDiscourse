using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class OpinionBlogInterviewViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public OpinionBlogInterviewViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            DateTime todayDate = DateTime.Today.AddDays(-30);
            reg = reg == "Global Edition" ? "" : reg;
            if (reg == "")
            {
                var search = (from a in _db.DevNews
                              //where a.AdminCheck == true && a.Type == "Blog" && a.CreatedOn > todayDate
                              //orderby a.PublishedOn descending
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
                              }).Take(4);
                return View( search.ToList());
            }
            else
            {
                var search = (from a in _db.DevNews
                              //where a.AdminCheck == true && a.Region.Contains(reg) && a.Type == "Blog" && a.CreatedOn > todayDate
                              //orderby a.PublishedOn descending
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
                              }).Take(4);
                return View(search.ToList());
            }
        }
    }
}
