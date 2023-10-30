using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
	public class NewsViewComponent : ViewComponent
	{
		private ApplicationDbContext _db;
		public NewsViewComponent(ApplicationDbContext _db)
		{
			this._db = _db;
		}
		public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition", int skip = 0, int take = 0)
		{
			ViewBag.skipCount = skip;
			DateTime tenDays = DateTime.Today.AddHours(-10);
			//var infocusData = _db.Infocus.Where(a => a.Edition == reg).Select(a => a.NewsId);
			var resultdata = _db.DevNews.Take(10).ToList();
			if (reg == "Global Edition")
			{
				var result = _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Sector != null && a.NewsLabels != null)
					//.AsNoTracking()
					.OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels })
					//.AsNoTracking()
					.Skip(skip).Take(take);
				return View(result.ToList());
			}
			else
			{
				var result = _db.DevNews.Where(a => a.Type != "Blog" && a.CreatedOn > tenDays && a.AdminCheck == true && a.Region.Contains(reg) && a.Sector != null && a.NewsLabels != null)
					//.AsNoTracking()
					.OrderByDescending(a => a.ModifiedOn).Select(a => new LatestNewsView { Id = a.Id, Title = a.Title, CreatedOn = a.ModifiedOn, ImageUrl = a.ImageUrl, Sector = a.SubTitle, Country = a.Country, NewId = a.NewsId, Type = a.Type, SubType = a.SubType, Label = a.NewsLabels })
					//.AsNoTracking()
					.Skip(skip).Take(take);
				return View(result.ToList());
			}
		}
	}
}
