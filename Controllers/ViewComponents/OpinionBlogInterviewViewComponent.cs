using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
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
            await Task.Yield();
            DateTime todayDate = DateTime.Today.AddDays(-30);
            reg = reg == "Global Edition" ? "" : reg;
            if (reg == "")
            {
                var search = (from a in _db.DevNews
                              where a.AdminCheck == true && a.Type == "Blog" && a.CreatedOn > todayDate
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
                              }).Take(4);
                return View( search.ToList());
            }
            else
            {

                var region = (from c in _db.Countries
                              join r in _db.Regions on c.RegionId equals r.Id
                              where c.Title == reg
                              select new
                              {
                                  r.Title
                              }).FirstOrDefault();
                string regionTitle = "Global Edition";
                var result = region != null && region.Title != null ? regionTitle = region.Title : regionTitle = reg;

                var search = (from a in _db.DevNews
                              where a.AdminCheck == true &&
                              a.Region.Contains(result) && //region=India not working
                              a.Type == "Blog" && a.CreatedOn > todayDate
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
                              }).Take(4);
                return View(search.ToList());
            }
        }
    }
}
