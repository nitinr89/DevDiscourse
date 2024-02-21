using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                reg = reg == "Global Edition" ? "" : reg;
                DateTime thirtyDays = DateTime.Today.AddDays(-15);
                if (reg == "")
                {
                    //new 
                    var infocus = (from dn in _db.DevNews
                                   where dn.AdminCheck == true
                                         && dn.PublishedOn > thirtyDays
                                         && dn.Type == "Blog"
                                         && dn.SubType != "Interview"
                                   orderby dn.PublishedOn descending
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

                        var region = (from c in _db.Countries
                                      join r in _db.Regions on c.RegionId equals r.Id
                                      where c.Title == reg
                                      select new
                                      {
                                          r.Title
                                      }).FirstOrDefault();
                      string regionTitle = "Global Edition";
                   var result = region != null && region.Title != null ? regionTitle = region.Title : regionTitle = reg;
                    var infocus = (from dn in _db.DevNews
                                   where dn.AdminCheck
                                         && dn.PublishedOn > thirtyDays
                                         && dn.Type == "Blog"
                                        && dn.Region.Contains(result) && dn.SubType != "Interview"
                                   orderby dn.PublishedOn descending
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
