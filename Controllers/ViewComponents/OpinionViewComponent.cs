using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
			reg = reg == "Global Edition" ? "" : reg;
			DateTime thirtyDays = DateTime.Today.AddDays(-150);
			if (reg == "")
			{
				var search = (from a in _db.DevNews
							  where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.SubType != "Interview"
							  orderby a.PublishedOn descending
							  select new NewsViewModel
							  {
								  Title = a.Title,
								  CreatedOn = a.PublishedOn,
								  ImageUrl = a.ImageUrl,
								  SubType = a.Themes,
								  Subtitle = a.Description,
								  Country = a.Author,
								  NewsId = a.NewsId,
								  Label = a.NewsLabels
							  }).Take(10);
				//.AsNoTracking();
				return View(search.ToList());
			}
			else
			{
				var search = (from a in _db.DevNews
							  where a.AdminCheck == true && a.PublishedOn > thirtyDays && a.Type == "Blog" && a.Region.Contains(reg) && a.SubType != "Interview"
							  orderby a.PublishedOn descending
							  select new NewsViewModel
							  {
								  Title = a.Title,
								  CreatedOn = a.PublishedOn,
								  ImageUrl = a.ImageUrl,
								  SubType = a.Themes,
								  Subtitle = a.Description,
								  Country = a.Author,
								  NewsId = a.NewsId,
								  Label = a.NewsLabels
							  }).Take(10);
				//.AsNoTracking();
				return View(search.ToList());
			}
		}
	}
}
