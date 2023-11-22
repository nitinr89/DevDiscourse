
using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class InfocusLiveDiscourseViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public InfocusLiveDiscourseViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string reg)
        {
            await Task.Yield();
            reg = reg == "Global Edition" ? "Universal Edition" : reg;
            var InfocusLiveDiscourse = (from m in _db.LiveDiscourseInfocus
                                        where m.Edition == reg
                                        join s in _db.Livediscourses on m.LivediscourseId equals s.Id
                                        where s.AdminCheck == true && s.ParentId == 0
                                        orderby m.SrNo
                                        select new DiscourseViewModel
                                        {
                                            Id = s.Id,
                                            Title = s.Title,
                                            ImageUrl = s.ImageUrl,
                                            SrNo = m.SrNo,
                                            children = _db.Livediscourses.Where(a => a.ParentId == s.Id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new DiscourseChildViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl }).Take(2).ToList()
                                        }).Take(3);
            return View(InfocusLiveDiscourse);
            //var infocus = (from dn in _db.DevNews
            //               select new DiscourseViewModel
            //               {
            //                   Id = Convert.ToInt64(dn.Id),
            //                   //NewId = dn.NewsId,
            //                   Title = dn.Title,
            //                   ImageUrl = dn.ImageUrl,
            //                   //CreatedOn = dn.ModifiedOn,
            //                   //Type = dn.Type,
            //                   //SubType = dn.SubType,
            //                   //Country = dn.Country,
            //                   //Label = dn.NewsLabels,
            //                  // Ranking = 0
            //               }).OrderByDescending(a => a.Title)
            //    .Take(65)
            //    .ToList();
            //return View(infocus.GroupBy(s => s.Title).Select(a => a.FirstOrDefault()).OrderByDescending(o => o.Title).Take(6).ToList());
        }
    }
}
