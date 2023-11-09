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
            ViewBag.filter = filter;
			DateTime todayDate = DateTime.Today.AddDays(1).AddTicks(-1);
			DateTime weekend = todayDate.AddDays(-2).AddTicks(1);
			if (reg == "Global Edition")
			{
				var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.Id != id && a.IsSponsored == false).OrderByDescending(a => a.ViewCount).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Take(4)
					//.AsNoTracking()
					.ToList();
				return View(resultList);
			}
			else
			{
				var resultList = _db.DevNews.Where(a => a.AdminCheck == true && a.Sector != "14" && a.CreatedOn < todayDate && a.CreatedOn > weekend && a.Id != id && a.Region.Contains(reg) && a.IsSponsored == false).OrderByDescending(a => a.ViewCount).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.Sector, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels }).Take(4)
					//.AsNoTracking()
					.ToList();
				return View(resultList);
			}
		}
	}
}
