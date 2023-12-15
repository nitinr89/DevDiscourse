using Devdiscourse.Data;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Devdiscourse.Models;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class LiveBlogsViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public LiveBlogsViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            await Task.Yield();
            if (User.Identity.IsAuthenticated)
            {
                string user = userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);
                var search = _db.Livediscourses
                    //Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn)
                    .Select(s => new LiveblogViewModel
                    {
                        Title = s.Title,
                        Description = s.Description,
                        Id = s.Id,
                        CreatedOn = s.CreatedOn,
                        PublishedOn = s.PublishedOn,
                        ImageUrl = s.ImageUrl,
                        ImageCopyright = s.ImageCopyright,
                        ModifiedOn = s.ModifiedOn,
                        DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
                        CommentCount = s.CommentCount,
                        LikeCount = s.LikeCount,
                        DislikeCount = s.DislikeCount,
                        Tags = s.Tags,
                        ParentStoryLink = s.ParentStoryLink,
                        Reacted = _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog) == null ? ReactType.None : _db.Reacts.FirstOrDefault(r => r.ItemId == s.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.SubBlog).ReactType
                    }).Take(10).ToList();
                return View(search);
            }
            else
            {
                var search = _db.Livediscourses
                    //Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(a => a.CreatedOn).        
                    .Select(s => new LiveblogViewModel
                    {
                        Title = s.Title,
                        Description = s.Description,
                        Id = s.Id,
                        CreatedOn = s.CreatedOn,
                        ImageUrl = s.ImageUrl,
                        PublishedOn = s.PublishedOn,
                        ImageCopyright = s.ImageCopyright,
                        ModifiedOn = s.ModifiedOn,
                        DiscourseComments = _db.DiscourseComments.Where(a => a.ItemId == s.Id && a.ParentId == 0 && a.IsHidden == false).Take(1).ToList(),
                        CommentCount = s.CommentCount,
                        LikeCount = s.LikeCount,
                        DislikeCount = s.DislikeCount,
                        Tags = s.Tags,
                        ParentStoryLink = s.ParentStoryLink,
                        Reacted = ReactType.None
                    }).Take(10).ToList();
                return View(search);

            }
        }
    }
}
