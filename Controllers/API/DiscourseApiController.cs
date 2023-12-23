using Devdiscourse.Hubs;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Html2Amp;
using Html2Amp.Sanitization;
using Html2Amp.Sanitization.Implementation;
using Microsoft.AspNetCore.Identity;
using X.PagedList;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDiscourse.Controllers.API
{
    public class DiscourseApiController : Controller, IDisposable
    {
        private ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public DiscourseApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {

            this.db = db;
            this.userManager = userManager;
        }
        [HttpGet]
        [Route("api/Discourse/livediscourseUpdates/{parentId}/{page}")]
        public IActionResult livediscourseUpdates(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = db.Livediscourses.Where(a => a.ParentId == parentId && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(a => new { a.Id, a.Title, a.ImageUrl, a.ImageCaption, a.ImageCopyright, a.Country, a.CreatedOn, a.ViewCount, a.Region }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }
        [HttpGet]
        [Route("api/Discourse/SubLivediscourse/{parentId}/{page}")]
        public IActionResult SubLivediscourse(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = db.Livediscourses.Where(a => a.ParentId == parentId).OrderByDescending(o => o.CreatedOn).Select(a => new { a.Id, a.Title, a.ImageUrl, a.ImageCaption, a.ImageCopyright, a.Country, a.CreatedOn, a.ViewCount, a.Region }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }
        [Route("api/DiscourseTopic")]
        [HttpPost]
        public IActionResult DiscourseTopic(DiscourseTopic obj)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (ModelState.IsValid)
            {
                // Do something with the product (not shown).
                db.DiscourseTopics.Add(obj);
                obj.Creator = userManager.GetUserId(User);
                obj.CreatedOn = DateTime.UtcNow;
                obj.ModifiedOn = DateTime.UtcNow;
                db.SaveChanges();
                return Ok("Success");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        [Route("api/Discourse/GetSuggestedTopics")]
        public IActionResult GetSuggestedTopics()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var SuggestedTopics = db.DiscourseTopics.Where(a => a.IsPublished == false).OrderBy(o => o.CreatedOn).Select(a => new { a.Id, a.Title, Name = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, a.LikeCount, a.DislikeCount, ImageUrl = a.ApplicationUsers.ProfilePic, reacted = ReactType.None }).Take(4);
                return Ok(SuggestedTopics);
            }
            else
            {
                var user = userManager.GetUserId(User);
                var SuggestedTopics = db.DiscourseTopics.Where(a => a.IsPublished == false).OrderBy(o => o.CreatedOn).Select(a => new
                {
                    a.Id,
                    a.Title,
                    Name = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName,
                    a.LikeCount,
                    a.DislikeCount,
                    ImageUrl = a.ApplicationUsers.ProfilePic,
                    reacted = db.Reacts.FirstOrDefault(r => r.ItemId == a.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.Topic) == null ? ReactType.None : db.Reacts.FirstOrDefault(r => r.ItemId == a.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.Topic).ReactType
                }).Take(4);
                return Ok(SuggestedTopics);
            }

        }
        [HttpGet]
        [Route("api/Discourse/GetTrendingDiscourse/{take}/{id}")]
        public IActionResult GetTrendingDiscourse(int take, long id)
        {
            var SuggestedTopics = db.Livediscourses.Where(a => a.AdminCheck == true && a.ParentId == 0 && a.Id != id).OrderByDescending(o => o.ViewCount).Select(a => new { a.Id, a.Title, a.ImageUrl, a.Country }).Take(take);
            return Ok(SuggestedTopics);
        }
        [HttpGet]
        [Route("api/Discourse/GetInfocusDiscourse/{reg}")]
        public IActionResult GetInfocusDiscourse(string reg)
        {
            reg = reg == "Global Edition" ? "Universal Edition" : reg;
            var SuggestedTopics = (from m in db.LiveDiscourseInfocus
                                   where m.Edition == reg
                                   join s in db.Livediscourses on m.LivediscourseId equals s.Id
                                   where s.AdminCheck == true && s.ParentId == 0
                                   orderby m.SrNo
                                   select new { s.Id, s.Title, s.ImageUrl, s.Country }).Take(1);
            return Ok(SuggestedTopics);
        }
        [HttpGet]
        [Route("api/Discourse/GetLatestDiscourse/{take}/{id}")]
        public IActionResult GetLatestDiscourse(int take, long id)
        {
            var LatestDiscourse = db.Livediscourses.Where(a => a.AdminCheck == true && a.ParentId == 0 && a.Id != id).OrderByDescending(o => o.CreatedOn).Select(a => new { a.Id, a.Title, a.ImageUrl, a.Country }).Take(take);
            return Ok(LatestDiscourse);
        }
        [HttpGet]
        [Route("api/Discourse/GetFollowedDiscourse/{take}")]
        public IActionResult GetFollowedDiscourse(int take)
        {
            var user = userManager.GetUserId(User);
            var LatestDiscourse = db.FollowLivediscourses.Where(a => a.FollowBy == user).OrderByDescending(o => o.FollowOn).Select(a => new { a.Livediscourses.Id, a.Livediscourses.Title, a.Livediscourses.ImageUrl, a.Livediscourses.Country }).Take(take);
            return Ok(LatestDiscourse);
        }
        [HttpGet]
        [Route("api/Discourse/GetRecommendedDiscourse/{take}")]
        public IActionResult GetRecommendedDiscourse(int take)
        {
            var LatestDiscourse = (from m in db.Livediscourses
                                   where m.AdminCheck == true && m.ParentId != 0
                                   join p in db.Livediscourses on m.ParentId equals p.Id
                                   where p.AdminCheck == true
                                   orderby m.CreatedOn descending
                                   select new
                                   {
                                       m.Id,
                                       m.Title,
                                       m.ParentId,
                                       m.Country,
                                       m.ImageUrl,
                                       ParenTitle = p.Title,
                                       ParentImageUrl = p.ImageUrl
                                   }).Take(take);
            return Ok(LatestDiscourse);
        }
        [Route("api/FollowLiveDiscourse/{id}")]
        [HttpPost]
        public IActionResult FollowLiveDiscourse(long id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (ModelState.IsValid)
            {
                string returnText = "";
                string user = userManager.GetUserId(User);
                var search = db.FollowLivediscourses.FirstOrDefault(a => a.FollowBy == user && a.LivediscourseId == id);
                if (search != null)
                {
                    db.FollowLivediscourses.Remove(search);
                    db.SaveChanges();
                    returnText = "Follow";
                }
                else
                {
                    FollowLivediscourse follow = new FollowLivediscourse();
                    follow.FollowBy = user;
                    follow.LivediscourseId = id;
                    follow.FollowOn = DateTime.UtcNow;
                    db.FollowLivediscourses.Add(follow);
                    db.SaveChanges();
                    returnText = "Following";
                }
                var livediscourse = db.Livediscourses.Find(id);
                if (livediscourse != null)
                {
                    livediscourse.FollowCount = db.FollowLivediscourses.Count(a => a.LivediscourseId == id);
                    db.Entry(livediscourse).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Ok(returnText);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [Route("api/FollowDiscourseTag/{id}")]
        [HttpPost]
        public IActionResult FollowDiscourseTag(long id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (ModelState.IsValid)
            {

                string user = userManager.GetUserId(User);
                var search = db.FollowTags.FirstOrDefault(a => a.FollowBy == user && a.TagId == id);
                string returnText = "";
                if (search != null)
                {
                    db.FollowTags.Remove(search);
                    db.SaveChanges();
                    returnText = "Follow";
                }
                else
                {
                    FollowTag follow = new FollowTag();
                    follow.FollowBy = user;
                    follow.TagId = id;
                    follow.FollowOn = DateTime.UtcNow;
                    db.FollowTags.Add(follow);
                    db.SaveChanges();
                    returnText = "Unfollow";
                }
                return Ok(returnText);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        [Route("api/Discourse/GetChildTags/{title}/{parentId}/{page}")]
        public IActionResult GetChildTags(string title, long parentId, int page = 1)
        {
            title = (title == "All") ? "" : title;
            var skipCount = (page - 1) * 20;
            var LatestDiscourse = db.DiscourseTags.Where(a => a.Title.Contains(title) && a.ParentId == parentId).Select(a => new { a.Id, a.Title, Sector = a.DevSector.Title, a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Skip(skipCount).Take(20);
            return Ok(LatestDiscourse);
        }
        [HttpGet]
        [Route("/api/DevSector/GetSector")]
        public IActionResult GetSector()
        {
            var devSectors = db.DevSectors.Select(a => new { a.Id, a.Title }).OrderByDescending(o => o.Title).ToArray();
            return Ok(devSectors);
        }
        [HttpGet]
        [Route("api/Discourse/GetTags")]
        public IActionResult GetTags()
        {
            if (User.Identity.IsAuthenticated)
            {
                string user = userManager.GetUserId(User);
                var followedTag = db.FollowTags.Where(a => a.FollowBy == user).Select(a => a.DiscourseTags);
                var LatestDiscourse = db.DiscourseTags.Except(followedTag).Select(a => new { a.Id, a.Title, Sector = a.DevSector.Title, a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(15);
                return Ok(LatestDiscourse);
            }
            else
            {
                var LatestDiscourse = db.DiscourseTags.Select(a => new { a.Id, a.Title, Sector = a.DevSector.Title, a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(15);
                return Ok(LatestDiscourse);
            }
        }
        [Route("api/ReactLivediscourse/{id}/{itemType}/{reactType}")]
        [HttpPost]
        public IActionResult ReactLivediscourse(long id, ReactItemType itemType, ReactType reactType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (id != 0)
            {

                string user = userManager.GetUserId(User);
                var react = db.Reacts.FirstOrDefault(a => a.ReactBy == user && a.ItemId == id && a.ReactItemType == itemType);
                if (react != null)
                {
                    react.ReactType = reactType;
                    db.Entry(react).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    React newReact = new React();
                    newReact.ReactBy = user;
                    newReact.ItemId = id;
                    newReact.ReactItemType = itemType;
                    newReact.ReactType = reactType;
                    newReact.ReactOn = DateTime.UtcNow;
                    db.Reacts.Add(newReact);
                    db.SaveChanges();
                }
                var reacted = db.Reacts.Where(a => a.ItemId == id && a.ReactItemType == itemType).AsEnumerable();
                var likeCount = reacted.Count(r => r.ReactType == ReactType.Like);
                var dislikeCount = reacted.Count(r => r.ReactType == ReactType.Dislike);
                var endorseCount = reacted.Count(r => r.ReactType == ReactType.Endorse);
                var rejectCount = reacted.Count(r => r.ReactType == ReactType.Reject);
                if (itemType == ReactItemType.SubBlog)
                {
                    var subBlog = db.Livediscourses.Find(id);
                    subBlog.LikeCount = likeCount;
                    subBlog.DislikeCount = dislikeCount;
                    db.Entry(subBlog).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (itemType == ReactItemType.Topic)
                {
                    var topic = db.DiscourseTopics.Find(id);
                    topic.LikeCount = likeCount;
                    topic.DislikeCount = dislikeCount;
                    db.Entry(topic).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (itemType == ReactItemType.Comment)
                {
                    var comment = db.DiscourseComments.Find(id);
                    comment.LikeCount = likeCount;
                    comment.DislikeCount = dislikeCount;
                    comment.EndorseCount = endorseCount;
                    comment.RejectCount = rejectCount;
                    db.Entry(comment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Ok(new { rt = reactType, it = itemType, id = id, likeCount, dislikeCount, endorseCount, rejectCount });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        [Route("api/Discourse/GetDiscourseIndex/{id}/{page}")]
        public IActionResult GetDiscourseIndex(long id, int page)
        {
            var skipCount = (page - 1) * 20;
            var DiscourseIndexes = db.DiscourseIndexs.Where(a => a.LivediscourseId == id).OrderBy(o => o.CreatedOn).Select(a => new { a.Id, a.Title }).Skip(skipCount).Take(20);
            return Ok(DiscourseIndexes);
        }
        [HttpGet]
        [Route("api/Discourse/GetSubBlogs/{id}/{page}")]
        public IActionResult GetSubBlogs(long id, int page)
        {
            var skip = (page - 1) * 20;
            var search = (db.LiveBlogs.Where(a => a.ParentId == id).OrderByDescending(a => a.CreatedOn).Select(b => new
            {
                b.Id,
                b.Title,
                b.ImageUrl,
                b.Description,
                b.CreatedOn
            })).Skip(skip).Take(20);
            return Ok(search);
        }
        [HttpGet]
        [Route("api/Discourse/livediscourseIndexUpdates/{parentId}/{page}")]
        public IActionResult livediscourseIndexUpdates(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = (from m in db.DiscourseIndexs
                                    where m.LivediscourseId == parentId
                                    orderby m.CreatedOn
                                    select new
                                    {
                                        m.Title,
                                        m.Id,
                                        Livediscourse = db.Livediscourses.Where(s => s.LivediscourseIndex == m.Id && s.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new { s.Id, s.Title, s.CreatedOn }).Take(3)
                                    }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }
        [HttpGet]
        [Route("api/Discourse/livediscourseIndexUpdatesById/{id}/{skip}")]
        public IActionResult livediscourseIndexUpdatesById(long id, int skip = 3)
        {
            var discourseUpdates = db.Livediscourses.Where(s => s.LivediscourseIndex == id && s.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new { s.Id, s.Title, s.CreatedOn }).Skip(skip).Take(10);
            return Ok(discourseUpdates);
        }
        [HttpGet]
        [Route("api/Discourse/livediscourseFollower/{id}/{name}/{page}")]
        public IActionResult livediscourseFollower(long id, string name, int page = 1)
        {
            if (name == "All")
            {
                name = "";
            }
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = (from m in db.FollowLivediscourses
                                    where m.LivediscourseId == id && m.ApplicationUsers.FirstName.Contains(name)
                                    orderby m.FollowOn
                                    select new
                                    {
                                        m.FollowBy,
                                        m.Id,
                                        Name = m.ApplicationUsers.FirstName,
                                        m.IsModerator
                                    }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }
        /// <summary>
        /// Method to create/Update Moderator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMod"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Discourse/FollowerToModerator/{id}/{isMod}")]
        public IActionResult FollowerToModerator(long id, bool isMod)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            var followLivediscourses = db.FollowLivediscourses.Find(id);
            if (followLivediscourses != null)
            {
                followLivediscourses.IsModerator = isMod;
                db.Entry(followLivediscourses).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Ok("success");
        }
        [HttpGet]
        [Route("api/Discourse/GetInfocusLiveDiscourse/{reg?}")]
        public IActionResult GetInfocusLiveDiscourse(string reg)
        {
            reg = reg == "Global Edition" ? "" : reg;
            var InfocusLiveDiscourse = (from m in db.Livediscourses
                                        where m.Region.Contains(reg) && m.ParentId == 0 && m.AdminCheck == true
                                        orderby m.CreatedOn descending
                                        select new DiscourseViewModel
                                        {
                                            Id = m.Id,
                                            Title = m.Title,
                                            ImageUrl = m.ImageUrl,
                                            children = db.Livediscourses.Where(a => a.ParentId == m.Id).OrderByDescending(o => o.CreatedOn).Select(s => new DiscourseChildViewModel { Id = s.Id, Title = s.Title }).Take(2).ToList()
                                        }).Take(3);
            return Ok(InfocusLiveDiscourse);
        }
        [HttpGet]
        [Route("api/Discourse/GetModerators/{id}")]
        public IActionResult GetModerators(long id)
        {
            var moderators = db.FollowLivediscourses.Where(a => a.LivediscourseId == id && a.IsModerator == true).Select(s => new { Id = s.FollowBy, Name = s.ApplicationUsers.FirstName + " " + s.ApplicationUsers.LastName });
            return Ok(moderators);
        }
        [HttpGet]
        [Route("api/Discourse/GetDiscourseAmpIndex/{id}/{__amp_source_origin?}")]
        public IActionResult GetDiscourseAmpIndex(long id, string __amp_source_origin)
        {
            var returnData = db.DiscourseIndexs.Where(a => a.LivediscourseId == id).OrderByDescending(o => o.CreatedOn).Select(s => new { s.Id, s.Title }).ToList();
            return Ok(new { items = returnData, hasMorePages = returnData.Any() });
        }
        [HttpGet]
        [Route("api/Discourse/GetDiscourseIndexItems/{id}/{moreItemsPageIndex?}/{__amp_source_origin?}")]
        public IActionResult GetDiscourseIndexItems(long id, string __amp_source_origin, int? moreItemsPageIndex)
        {
            var search = db.Livediscourses.Where(a => a.LivediscourseIndex == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(b => new DiscourseItemAmpViewModel { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn }).Take(3).ToList();
            //if (!string.IsNullOrEmpty(__amp_source_origin))
            //{
            //    HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
            //}
            int pageSize = 10;
            int pageNumber = (moreItemsPageIndex ?? 1);
            var resultData = search.Select(b => new { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn.ToString(), Url = "/live-discourse" + "/" + b.GenerateSecondSlug().ToString() }).ToPagedList(pageNumber, pageSize);
            return Ok(new { items = resultData, hasMorePages = resultData.Any() });
        }
        [HttpGet]
        [Route("api/Discourse/GetDiscourseUpdates/{id}/{moreItemsPageIndex?}/{__amp_source_origin?}")]
        public IActionResult GetDiscourseUpdates(long id, string __amp_source_origin, int? moreItemsPageIndex)
        {
            int pageSize = 20;
            int pageNumber = (moreItemsPageIndex ?? 1);
            var search = db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new LiveblogViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl, Description = s.Description, CreatedOn = s.CreatedOn }).ToPagedList(pageNumber, pageSize).ToList();
            //var search = db.Livediscourse.Where(a => a.LivediscourseIndex == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(b => new DiscourseItemAmpViewModel { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn }).Take(3).ToList();
            //if (!string.IsNullOrEmpty(__amp_source_origin))
            //{
            //    HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
            //}

            var resultData = search.Select(b => new { Id = b.Id, Title = b.Title, ImageUrl = (b.ImageUrl ?? "").IndexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + b.ImageUrl : b.ImageUrl, Description = GetAmpHtml(b.Description), CreatedOn = b.CreatedOn.ToString("dd-MM-yyyy hh:mm:ss"), hasImage = b.ImageUrl == null ? false : true });
            return Ok(new { items = resultData, hasMorePages = search.Any() });
        }
        public string GetAmpHtml(string Description)
        {
            string ampHtml = "";
            try
            {
                var converter = new HtmlToAmpConverter();
                converter.WithSanitizers(
                    new HashSet<ISanitizer>
                    {
                    new InstagramSanitizer(),
                    new TwitterSanitizer(),
                    new AudioSanitizer(),
                    new HrefJavaScriptSanitizer(),
                    new ImageSanitizer(),
                    new JavaScriptRelatedAttributeSanitizer(),
                    new StyleAttributeSanitizer(),
                    new ScriptElementSanitizer(),
                    new TargetAttributeSanitizer(),
                    new XmlAttributeSanitizer(),
                    new YouTubeVideoSanitizer(),
                    new AmpIFrameSanitizer()
                    });
                ampHtml = converter.ConvertFromHtml(Description).AmpHtml;
            }
            catch
            {

            }
            return ampHtml;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
        }
    }
}
