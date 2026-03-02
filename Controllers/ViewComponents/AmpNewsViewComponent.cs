using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class AmpNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public AmpNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition", int skip = 0, int take = 0)
        {
            await Task.Yield();
            ViewBag.skipCount = skip;
            DateTime threemonths = DateTime.Today.AddDays(-10);
            var infocusData = _db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
            if (reg == "Global Edition")
            {
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
