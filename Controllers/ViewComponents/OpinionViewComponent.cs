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
            try { 
            reg = reg == "Global Edition" ? "" : reg;
            DateTime thirtyDays = DateTime.Today.AddDays(-120);
                //var thirtyDays = DateTime.Now.AddDays(-30);
                if (reg == "")
                {


                    //old
                    // var infocus = (from dn in _db.DevNews
                    //                where dn.AdminCheck == true && dn.PublishedOn > thirtyDays && dn.Type == "Blog" && dn.SubType != "Interview"
                    //                select new NewsViewModel
                    //                {
                    //                    // Id = dn.Id,
                    //                    NewsId = dn.NewsId,
                    //                    Title = dn.Title,
                    //                    ImageUrl = dn.ImageUrl,
                    //                    CreatedOn = dn.ModifiedOn,
                    //                    Subtitle = dn.SubTitle,
                    //                    SubType = dn.Themes,
                    //                    Country = dn.Country,
                    //                    Label = dn.NewsLabels,
                    //                    // Ranking = 0
                    //                })
                    //.Take(10);
                    //.ToList();
                    // return View(infocus.ToList());


                    //new 
                    var infocus = (from dn in _db.DevNews
                                   where dn.AdminCheck
                                         && dn.PublishedOn > thirtyDays
                                         && dn.Type == "Blog"
                                         && dn.SubType != "Interview"
                                   //  orderby dn.PublishedOn descending
                                   select new NewsViewModel
                                   {
                                       NewsId = dn.NewsId,
                                       Title = dn.Title,
                                       ImageUrl = dn.ImageUrl,
                                       CreatedOn = dn.ModifiedOn,
                                       Subtitle = dn.SubType,
                                       SubType = dn.Themes,
                                       Country = dn.Country,
                                       Label = dn.NewsLabels,
                                       Ranking = 0
                                   })
                                  .Take(10)
                                  .ToList();
                    return View(infocus.ToList());

                }
                else
                {

                    //this was wroking 
                    //var infocus = (from dn in _db.DevNews
                    //               where dn.AdminCheck == true && dn.PublishedOn > thirtyDays && dn.Type == "Blog" && dn.Region.Contains(reg) && dn.SubType != "Interview"
                    //               			  orderby dn.PublishedOn descending
                    //               select new NewsViewModel
                    //               {
                    //                   // Id = dn.Id,
                    //                     NewsId = dn.NewsId,
                    //                   Title = dn.Title,
                    //                   ImageUrl = dn.ImageUrl,
                    //                   CreatedOn = dn.ModifiedOn,
                    //                   Subtitle=dn.SubType,
                    //                   SubType = dn.Themes,
                    //                   Country = dn.Country,
                    //                   Label = dn.NewsLabels,
                    //                   Ranking = 0
                    //               })
                    //.Take(10);
                    ////.ToList();
                    //return View(infocus.ToList());



                    //new query
                    var infocus = (from dn in _db.DevNews
                                   where dn.AdminCheck
                                         && dn.PublishedOn > thirtyDays
                                         && dn.Type == "Blog"
                                         && dn.SubType != "Interview"
                                   // orderby dn.PublishedOn descending
                                   select new NewsViewModel
                                   {
                                       NewsId = dn.NewsId,
                                       Title = dn.Title,
                                       ImageUrl = dn.ImageUrl,
                                       CreatedOn = dn.ModifiedOn,
                                       Subtitle = dn.SubType,
                                       SubType = dn.Themes,
                                       Country = dn.Country,
                                       Label = dn.NewsLabels,
                                       Ranking = 0
                                   })
                                  .Take(10)
                                  .ToList();
                    return View(infocus.ToList());
                }
                

            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
