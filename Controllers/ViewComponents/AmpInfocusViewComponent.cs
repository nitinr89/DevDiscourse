using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class AmpInfocusViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public AmpInfocusViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg="Global Edition")
        {
            await Task.Yield();
            var search = (from m in _db.Infocus
                          where m.Edition == reg && (m.ItemType == "News" || m.ItemType == "Blog")
                          join s in _db.DevNews.Where(s => s.AdminCheck == true) on m.NewsId equals s.NewsId
                          orderby m.SrNo
                          select new LatestNewsView
                          {
                              Id = s.Id,
                              NewId = s.NewsId,
                              Title = s.Title,
                              ImageUrl = s.ImageUrl,
                              CreatedOn = s.ModifiedOn,
                              Type = s.Type,
                              SubType = s.SubType,
                              Country = s.Country,
                              Label = s.NewsLabels,
                              SrNo = m.SrNo
                          }).AsNoTracking().Take(10);

            return View( search.ToList());
        }
    }
}
