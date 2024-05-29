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
            DateTime twoDays = DateTime.Today.AddDays(-2);
            if (reg == "Global Edition")
            {
                var result = await (from dn in _db.DevNews
                                    where dn.CreatedOn > twoDays && dn.AdminCheck == true && dn.Type != "Blog"
                                    select new NewsViewModel
                                    {
                                        NewsId = dn.NewsId,
                                        Title = dn.Title,
                                        ImageUrl = dn.ImageUrl,
                                        CreatedOn = dn.ModifiedOn,
                                        Subtitle = dn.SubTitle,
                                        Label = dn.NewsLabels,
                                    }).OrderByDescending(a => a.CreatedOn).Take(5).ToListAsync();
                return View(result);
            }
            else
            {
                var result = await (from dn in _db.DevNews
                                    where dn.CreatedOn > twoDays && dn.AdminCheck == true && dn.Type != "Blog"
                                     && dn.Region != null && dn.Region.ToLower().Contains(reg.ToLower())
                                    select new NewsViewModel
                                    {
                                        NewsId = dn.NewsId,
                                        Title = dn.Title,
                                        ImageUrl = dn.ImageUrl,
                                        CreatedOn = dn.ModifiedOn,
                                        Subtitle = dn.SubTitle,
                                        Label = dn.NewsLabels,
                                    }).OrderByDescending(a => a.CreatedOn).Take(5).ToListAsync();
                return View(result);
            }
        }
    }
}
