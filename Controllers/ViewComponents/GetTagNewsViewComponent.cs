using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;


namespace Devdiscourse.Controllers.ViewComponents
{
    public class GetTagNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public GetTagNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(long id, string tag, string sector)
        {
            await Task.Yield();
            try
            {
                var tagList = tag.Split(',').Reverse().Skip(3).Take(3).ToList();
                DateTime threemonths = DateTime.Today.AddDays(-15);
                if (!string.IsNullOrEmpty(sector))
                {
                    var result = (from a in _db.DevNews
                                  from s in tagList
                                  where a.CreatedOn > threemonths && a.NewsId != id && a.AdminCheck == true
                                  && (a.Title.Contains(s)) && a.Sector == sector
                                  select new LatestNewsView
                                  {
                                      Title = a.Title,
                                      NewId = a.NewsId,
                                      Label = a.NewsLabels
                                  }).OrderByDescending(a => a.CreatedOn).Distinct().Take(5);
                    return View(result);
                }
                else
                {
                    var result = _db.DevNews
                        .Where(a =>
                            a.CreatedOn > threemonths
                            && a.NewsId != id
                            && a.AdminCheck)
                        .AsEnumerable() // Bring data into memory
                        .Where(a => tagList.Any(s => a.Title != null && a.Title.Contains(s)))
                        .OrderByDescending(a => a.CreatedOn)
                        .Select(a => new LatestNewsView
                        {
                            Title = a.Title,
                            NewId = a.NewsId,
                            Label = a.NewsLabels
                        })
                        .Distinct()
                        .Take(5)
                        .ToList();

                    return View(result);
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
