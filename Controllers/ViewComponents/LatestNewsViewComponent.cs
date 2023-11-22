using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
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
                //var result = _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > twoDays && a.AdminCheck == true)
                //    .Select(a => new NewsViewModel
                //    {
                //        Title = a.Title,
                //        CreatedOn = a.ModifiedOn,
                //        ImageUrl = a.ImageUrl,
                //        Subtitle = a.Description,
                //        NewsId = a.NewsId,
                //        Label = a.NewsLabels
                //    }).OrderByDescending(a => a.CreatedOn).AsNoTracking().Take(4);
                //return View(result.ToList());

                var infocus = (from dn in _db.DevNews
                               select new LatestNewsView
                               {
                                   Id = dn.Id,
                                   NewId = dn.NewsId,
                                   Title = dn.Title,
                                   ImageUrl = dn.ImageUrl,
                                   CreatedOn = dn.ModifiedOn,
                                   Type = dn.Type,
                                   SubType = dn.SubType,
                                   Country = dn.Country,
                                   Label = dn.NewsLabels,
                                   Ranking = 0
                               }).OrderByDescending(a => a.CreatedOn)
                     .Take(65)
                     .ToList();
                return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());
            }
            else
            {
                //var result = _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > twoDays && a.AdminCheck == true && a.Region.Contains(reg))
                //    .Select(a => new NewsViewModel
                //    {
                //        Title = a.Title,
                //        CreatedOn = a.ModifiedOn,
                //        ImageUrl = a.ImageUrl,
                //        Subtitle = a.Description,
                //        NewsId = a.NewsId,
                //        Label = a.NewsLabels
                //    }).OrderByDescending(a => a.CreatedOn).AsNoTracking().Take(4);
                //return View(result.ToList());

                var infocus = (from dn in _db.DevNews
                               select new LatestNewsView
                               {
                                   Id = dn.Id,
                                   NewId = dn.NewsId,
                                   Title = dn.Title,
                                   ImageUrl = dn.ImageUrl,
                                   CreatedOn = dn.ModifiedOn,
                                   Type = dn.Type,
                                   SubType = dn.SubType,
                                   Country = dn.Country,
                                   Label = dn.NewsLabels,
                                   Ranking = 0
                               }).OrderByDescending(a => a.CreatedOn)
                     .Take(65)
                     .ToList();
                return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Ranking).Take(6).ToList());
            }
        }
    }
}
