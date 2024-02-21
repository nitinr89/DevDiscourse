using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class TrendsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public TrendsViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? id, string reg = "Global Edition", string filter = "")
        {
            await Task.Yield();
            try { 
            ViewBag.filter = filter;
            DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
            //DateTime todayDate = DateTime.Today.AddDays(-50).AddTicks(-10);
            DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
                //DateTime weekend = todayDate.AddDays(-20).AddTicks(1);
                if (reg == "Global Edition")
                {              
                    var resultList = _db.DevNews
      .Where(dn => dn.AdminCheck == true &&
                   dn.CreatedOn < todayDate && dn.CreatedOn > weekend && dn.CreatedOn < todayDate && dn.Id != id &&
                   dn.Sector == "14")
      .OrderByDescending(dn => dn.CreatedOn)
      .Take(65)
      .Select(dn => new LatestNewsView
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
      })
      .ToList();

                    var groupedResult = resultList
                        .GroupBy(s => s.Title)
                        .Select(group => group.OrderByDescending(a => a.Ranking).FirstOrDefault())
                        .OrderByDescending(o => o.Ranking)
                        .Take(5)
                        .ToList();

                    return View(groupedResult);
                }
                else
                {
                    var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.CreatedOn < todayDate && a.CreatedOn > weekend &&
                    a.Id != id 
                    //&& a.Region.Contains(reg)
                    && a.IsSponsored == false).OrderByDescending(a => a.ViewCount)
                        .Select(a => new LatestNewsView
                        {
                            Id = a.Id,
                            Title = a.Title,
                            CreatedOn = a.ModifiedOn,
                            ImageUrl = a.ImageUrl,
                            Sector = a.Sector,
                            Country = a.Country,
                            NewId = a.NewsId,
                            Type = a.Type,
                            SubType = a.SubType,
                            Label = a.NewsLabels
                        }).Take(4)
                        .ToList();
                    return View(resultList);
                }
            }catch(Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
