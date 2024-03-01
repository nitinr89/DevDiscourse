using Devdiscourse.Data;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class LivediscourseampViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public LivediscourseampViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            if (User.Identity.IsAuthenticated)
            {
                string user = userManager.GetUserId(User as ClaimsPrincipal);
                var search = db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
                {
                    Title = s.Title,
                    Description = s.Description,
                    PublishedOn = s.PublishedOn,
                    Id = s.Id,
                    CreatedOn = s.CreatedOn,
                    ImageUrl = s.ImageUrl,
                    ImageCopyright = s.ImageCopyright,
                    ModifiedOn = s.ModifiedOn,
                    DiscourseComments = db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
                    CommentCount = s.CommentCount,
                    LikeCount = s.LikeCount,
                    DislikeCount = s.DislikeCount,
                    Tags = s.Tags,
                    Reacted = db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog) == null ? ReactType.None : db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog).ReactType
                }).ToList();
                return View(search);
            }
            else
            {
                var search = db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).Select(s => new LiveblogViewModel
                {
                    Title = s.Title,
                    Description = s.Description,
                    Id = s.Id,
                    CreatedOn = s.CreatedOn,
                    PublishedOn = s.PublishedOn,
                    ImageUrl = s.ImageUrl,
                    ImageCopyright = s.ImageCopyright,
                    ModifiedOn = s.ModifiedOn,
                    DiscourseComments = db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
                    CommentCount = s.CommentCount,
                    LikeCount = s.LikeCount,
                    DislikeCount = s.DislikeCount,
                    Tags = s.Tags,
                    Reacted = ReactType.None
                }).ToList();
                return View(search);
            }
        }
    }
}
