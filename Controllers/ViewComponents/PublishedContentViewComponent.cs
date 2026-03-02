using Devdiscourse.Data;
using Devdiscourse.Models.ContributorModels;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers.ViewComponents
{
    public class PublishedContentViewComponent : ViewComponent
    {
        private ApplicationDbContext db;
        public PublishedContentViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(string userId = "", string fl = "")
        {
            await Task.Yield();
            List<PublishView> ResultList = new List<PublishView>();
            if (!String.IsNullOrEmpty(userId))
            {
                ResultList = db.Earnings.Where(a => a.Creator == userId).Select(a => new PublishView { Id = a.Id, NewsId = a.NewsId, Title = a.Contents.Title, ViewCount = a.ViewCount, Amount = a.Amount, CreatedOn = a.CreatedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.Contents.IsVideo }).ToList();
            }
            else
            {
                ResultList = db.Earnings.Select(a => new PublishView { Id = a.Id, NewsId = a.NewsId, Title = a.Contents.Title, ViewCount = a.ViewCount, Amount = a.Amount, CreatedOn = a.CreatedOn, Creator = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, IsVideo = a.Contents.IsVideo }).ToList();
            }
            if (fl == "article")
            {
                ResultList = ResultList.Where(a => a.IsVideo == false).ToList();
            }
            else if (fl == "video")
            {
                ResultList = ResultList.Where(a => a.IsVideo == true).ToList();
            }
            return View(ResultList.OrderByDescending(a => a.CreatedOn));
        }
    }
}
