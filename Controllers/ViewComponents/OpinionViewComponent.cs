using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class OpinionViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public OpinionViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            reg = reg == "Global Edition" ? "" : reg;
            DateTime thirtyDays = DateTime.Today.AddDays(-120);
            if (reg == "")
            {
                //var search = (from a in _db.DevNews
                //			  where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.SubType != "Interview"
                //			  orderby a.PublishedOn descending
                //			  select new NewsViewModel
                //			  {
                //				  Title = a.Title,
                //				  CreatedOn = a.PublishedOn,
                //				  ImageUrl = a.ImageUrl,
                //				  SubType = a.Themes,
                //				  Subtitle = a.Description,
                //				  Country = a.Author,
                //				  NewsId = a.NewsId,
                //				  Label = a.NewsLabels

                //			  }).Take(10);
                ////.AsNoTracking();
                //return View(search.ToList());
                var infocus = (from dn in _db.DevNews
                               select new NewsViewModel
                               {
                                   // Id = dn.Id,
                                   //  NewId = dn.NewsId,
                                   Title = dn.Title,
                                   ImageUrl = dn.ImageUrl,
                                   CreatedOn = dn.ModifiedOn,
                                   // Type = dn.Type,
                                   SubType = dn.SubType,
                                   Country = dn.Country,
                                   Label = dn.NewsLabels,
                                   Ranking = 0
                               })
               .Take(10);
                //.ToList();
                return View(infocus.ToList());
            }
            else
            {
                //var search = (from a in _db.DevNews
                //			  where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.Region.Contains(reg) && a.SubType != "Interview"
                //			  orderby a.PublishedOn descending
                //			  select new NewsViewModel
                //			  {
                //				  Title = a.Title,
                //				  CreatedOn = a.PublishedOn,
                //				  ImageUrl = a.ImageUrl,
                //				  SubType = a.Themes,
                //				  Subtitle = a.Description,
                //				  Country = a.Author,
                //				  NewsId = a.NewsId,
                //				  Label = a.NewsLabels

                //			  }).Take(10);
                ////.AsNoTracking();
                //return View(search.ToList());

                var infocus = (from dn in _db.DevNews
                               select new NewsViewModel
                               {
                                   // Id = dn.Id,
                                   //  NewId = dn.NewsId,
                                   Title = dn.Title,
                                   ImageUrl = dn.ImageUrl,
                                   CreatedOn = dn.ModifiedOn,
                                   // Type = dn.Type,
                                   SubType = dn.SubType,
                                   Country = dn.Country,
                                   Label = dn.NewsLabels,
                                   Ranking = 0
                               })
                .Take(10);
                //.ToList();
                return View(infocus.ToList());


            }
        }
    }
}
