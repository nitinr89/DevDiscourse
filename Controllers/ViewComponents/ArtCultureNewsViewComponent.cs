using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class ArtCultureNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ArtCultureNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string reg = "Global Edition")
        {
            try
            {
                ViewBag.Sector = "14";
                DateTime twoDays = DateTime.UtcNow.AddDays(-2);
                var resultList = await _db.RegionNewsRankings
                    .Where(dn => dn.DevNews.CreatedOn > twoDays &&
                       dn.DevNews.AdminCheck == true &&
                       dn.DevNews.Sector == "14" &&
                       dn.Region.Title == reg &&
                       dn.DevNews.IsSponsored != true
                  ).OrderByDescending(dn => dn.DevNews.CreatedOn).Take(30).Select(dn => new NewsViewModel
                  {
                      NewsId = dn.DevNews.NewsId,
                      Title = dn.DevNews.Title,
                      ImageUrl = dn.DevNews.ImageUrl,
                      CreatedOn = dn.DevNews.ModifiedOn,
                      Subtitle = dn.DevNews.SubTitle,
                      SubType = dn.DevNews.SubType,
                      Country = dn.DevNews.Country,
                      Sector = dn.DevNews.Sector,
                      Label = dn.DevNews.NewsLabels,
                      Ranking = dn.Ranking
                  }).ToListAsync();
                resultList = resultList.GroupBy(s => s.Title).Select(a => a.First()).OrderByDescending(a => a.Ranking).Take(6).ToList();

                var sponsoredNews = (from sn in _db.SponsoredNews
                                     join n in _db.DevNews on sn.NewsId equals n.Id
                                     where sn.IsActive == true && n.AdminCheck == true && sn.Sector == 14 && sn.EndTime > DateTime.UtcNow
                                     select new NewsViewModelIndex
                                     {
                                         Index = sn.Position,
                                         News = new NewsViewModel
                                         {
                                             NewsId = n.NewsId,
                                             Title = n.Title,
                                             ImageUrl = n.ImageUrl,
                                             CreatedOn = n.ModifiedOn,
                                             Subtitle = n.SubTitle,
                                             SubType = n.SubType,
                                             Country = n.Country,
                                             Sector = n.Sector,
                                             Label = n.NewsLabels,
                                             Ranking = 0
                                         }

                                     }).ToList();

                foreach (var item in sponsoredNews)
                {
                    resultList.RemoveAll(n => n.NewsId == item.News.NewsId);
                    resultList.Insert(item.Index, item.News);
                }
                return View(resultList);
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
    }
}
