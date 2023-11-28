using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class AnalyticViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public AnalyticViewComponent(ApplicationDbContext _db)
        {
            this._db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(DateTime? bfd, DateTime? afd, int skip = 0, string reg = "Global Edition", string sec = "0", string country = "", string text = "")
        {
            var search = _db.DevNews.Where(a => a.AdminCheck == true).OrderByDescending(a => a.ModifiedOn).Select(a => new AnalyticView { Id = a.NewsId, Title = a.Title, Region = a.Region, Country = a.Country, Sector = a.Sector, CreatedOn = a.CreatedOn, ModifiedOn = a.ModifiedOn });
            if (sec != "0")
            {
                search = search.Where(a => a.Sector.StartsWith(sec + ",") || a.Sector.Contains("," + sec + ",") || a.Sector.EndsWith("," + sec) || a.Sector == sec);
            }
            if (reg != "Global Edition")
            {
                search = search.Where(a => a.Region.Contains(reg));
            }
            if (!String.IsNullOrEmpty(country))
            {
                search = search.Where(a => a.Country.Contains(country));
            }
            if (!String.IsNullOrEmpty(text))
            {
                search = search.Where(a => a.Title.ToUpper().Contains(text.ToUpper()));
            }
            if (bfd != null)
            {
                DateTime filterDate = DateTime.Parse(bfd.ToString());
                search = search.Where(a => a.CreatedOn < filterDate);
            }
            if (afd != null)
            {
                DateTime filterDate2 = DateTime.Parse(afd.ToString());
                search = search.Where(a => a.CreatedOn > filterDate2);
            }
            return View(search.Skip(skip).Take(10).ToList());
        }
    }
}
