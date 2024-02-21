using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class LatestNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public LatestNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            await Task.Yield();
            DateTime twoDays = DateTime.Today.AddDays(-2);
            if (reg == "Global Edition")
            {
                var result = (from dn in _db.DevNews
                              where dn.AdminCheck == true && dn.CreatedOn > twoDays && dn.Type != "Blog"
                              select new NewsViewModel
                              {
                                  NewsId = dn.NewsId,
                                  Title = dn.Title,
                                  ImageUrl = dn.ImageUrl,
                                  CreatedOn = dn.ModifiedOn,
                                  Subtitle = dn.SubTitle,
                                  Label = dn.NewsLabels,
                              }).OrderByDescending(a => a.CreatedOn).AsNoTracking()
                     .Take(5);
                return View(result.ToList());               
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

                if (region != null && region.Title != null) regionTitle = region.Title;
                var result
                    = (from dn in _db.DevNews
                       where dn.Type != "Blog" &&  dn.CreatedOn > twoDays && dn.AdminCheck == true && dn.Region.Contains(regionTitle)
                       select new NewsViewModel
                       {
                           NewsId = dn.NewsId,
                           Title = dn.Title,
                           ImageUrl = dn.ImageUrl,
                           CreatedOn = dn.ModifiedOn,
                           Subtitle = dn.SubTitle,
                           Label = dn.NewsLabels,
                       }).OrderByDescending(a => a.CreatedOn).AsNoTracking()
                     .Take(5);
                return View(result.ToList());
            }
        }
    }
}
