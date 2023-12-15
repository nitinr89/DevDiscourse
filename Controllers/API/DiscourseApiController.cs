
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Hubs;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Html2Amp;
using X.PagedList;
using Html2Amp.Sanitization;
using Html2Amp.Sanitization.Implementation;

namespace Devdiscourse.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscourseApiController : ControllerBase
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> userManager;
        public DiscourseApiController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {

            this._db = _db;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("LivediscoursesUpdates/{parentId}/{page}")]
        public IActionResult LivediscoursesUpdates(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = _db.Livediscourses.Where(a => a.ParentId == parentId && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(a => new { a.Id, a.Title, a.ImageUrl, a.ImageCaption, a.ImageCopyright, a.Country, a.CreatedOn, a.ViewCount, a.Region }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }
        [HttpGet]
        [Route("SubLivediscourses/{parentId}/{page}")]
        public IActionResult SubLivediscourses(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = _db.Livediscourses.Where(a => a.ParentId == parentId).OrderByDescending(o => o.CreatedOn).Select(a => new { a.Id, a.Title, a.ImageUrl, a.ImageCaption, a.ImageCopyright, a.Country, a.CreatedOn, a.ViewCount, a.Region }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }

        [HttpPost]
        [Route("DiscourseTopic/{Title}/{Sector}")]
        public IActionResult DiscourseTopic(string Title, string Sector)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (ModelState.IsValid)
            {
                // Do something with the product (not shown).
                //_db.discoursetopics.add(obj);
                //obj.creator = usermanager.getuserid(user);
                //obj.createdon = datetime.utcnow;
                //obj.modifiedon = datetime.utcnow;
                _db.SaveChanges();
                return Ok("Success");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        [Route("GetSuggestedTopics")]
        public IActionResult GetSuggestedTopics()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var SuggestedTopics = _db.DiscourseTopics
                    //.Where(a => a.IsPublished == false).OrderBy(o => o.CreatedOn)
                    .Select(a => new { a.Id, a.Title, Name = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, a.LikeCount, a.DislikeCount, ImageUrl = a.ApplicationUsers.ProfilePic,reacted = ReactType.None }).Take(4);
                return Ok(SuggestedTopics);
            }
            else
            {
                var user = userManager.GetUserId(User);
                var SuggestedTopics = _db.DiscourseTopics
                    //.Where(a => a.IsPublished == false).OrderBy(o => o.CreatedOn)
                    .Select(a => new { a.Id, a.Title, Name = a.ApplicationUsers.FirstName + " " + a.ApplicationUsers.LastName, a.LikeCount, a.DislikeCount, ImageUrl = a.ApplicationUsers.ProfilePic,
                    reacted = _db.Reacts.FirstOrDefault(r=> r.ItemId == a.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.Topic) == null ? ReactType.None : _db.Reacts.FirstOrDefault(r => r.ItemId == a.Id && r.ReactBy == user && r.ReactItemType == ReactItemType.Topic).ReactType }).Take(4);
                return Ok(SuggestedTopics);
            }
            
        }
        [HttpGet]
        [Route("GetTrendingDiscourse/{take}/{id}")]
        public IActionResult GetTrendingDiscourse(int take,long id)
        {
            var SuggestedTopics = _db.Livediscourses.Where(a => a.AdminCheck == true && a.ParentId == 0 && a.Id != id).OrderByDescending(o => o.ViewCount).Select(a => new { a.Id, a.Title, a.ImageUrl,a.Country }).Take(take);
            return Ok(SuggestedTopics);
        }
        [HttpGet]
        [Route("GetInfocusDiscourse/{reg}")]
        public IActionResult GetInfocusDiscourse(string reg)
        {
            reg = reg == "Global Edition" ? "Universal Edition" : reg;
            var SuggestedTopics = (from m in _db.LiveDiscourseInfocus
                                  // where m.Edition == reg
                                        join s in _db.Livediscourses on m.LivediscourseId equals s.Id
                                       // where s.AdminCheck == true && s.ParentId == 0
                                        orderby m.SrNo
                                        select new { s.Id, s.Title, s.ImageUrl, s.Country }).Take(1);
            return Ok(SuggestedTopics);
        }

        [Route("GetSector")]
        public IActionResult GetSector()
        {
            var search = _db.DevSectors
                //.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.Title)
                .Select(s => new { s.Id, s.Slug, s.SrNo, s.Title });
            return Ok(search);
        }
        [HttpGet]
        [Route("GetLatestDiscourse/{take}/{id}")]
        public IActionResult GetLatestDiscourse(int take, long id)
        {
            var LatestDiscourse = _db.Livediscourses.Where(a => a.AdminCheck == true && a.ParentId == 0 && a.Id != id).OrderByDescending(o => o.CreatedOn).Select(a => new { a.Id, a.Title, a.ImageUrl, a.Country }).Take(take);
            return Ok(LatestDiscourse);
        }


        [HttpGet]
        [Route("livediscourseIndexUpdates/{parentId}/{page}")]
        public IActionResult livediscourseIndexUpdates(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = (from m in _db.DiscourseIndexs
                                    where m.LivediscourseId == parentId
                                    orderby m.CreatedOn
                                    select new
                                    {
                                        m.Title,
                                        m.Id,
                                        Livediscourse = _db.Livediscourses.Where(s => s.LivediscourseIndex == m.Id && s.AdminCheck == true).OrderByDescending(o => o.CreatedOn)
                                        .Select(s => new { s.Id, s.Title, s.CreatedOn }).Take(3).ToList()
                                    }).Skip(skipItem).Take(20);
            return Ok(discourseUpdates);
        }

        [HttpGet]
        [Route("livediscourseIndexUpdatesById/{id}/{skip}")]
        public IActionResult livediscourseIndexUpdatesById(long id, int skip = 3)
        {
            var discourseUpdates = _db.Livediscourses.Where(s => s.LivediscourseIndex == id && s.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new { s.Id, s.Title, s.CreatedOn }).Skip(skip).Take(10);
            return Ok(discourseUpdates);
        }

        [HttpGet]
        [Route("GetFollowedDiscourse/{take}")]
        public IActionResult GetFollowedDiscourse(int take)
        {
            var user = userManager.GetUserId(User);
            var LatestDiscourse = _db.FollowLivediscourses.Where(a => a.FollowBy== user).OrderByDescending(o => o.FollowOn).Select(a => new { a.Livediscourses.Id, a.Livediscourses.Title, a.Livediscourses.ImageUrl, a.Livediscourses.Country }).Take(take);
            return Ok(LatestDiscourse);
        }
        [HttpGet]
        [Route("GetRecommendedDiscourse/{take}")]
        public IActionResult GetRecommendedDiscourse(int take)
        {
            var LatestDiscourse = (from m in _db.Livediscourses
                                  //where m.AdminCheck == true && m.ParentId != 0
                                  //join p in _db.Livediscourses on m.ParentId equals p.Id
                                  //where p.AdminCheck == true
                                 //orderby m.CreatedOn descending
                                  select new
                                  {
                                      m.Id,
                                      m.Title,
                                      m.ParentId,
                                      m.Country,
                                      m.ImageUrl,
                                      //ParenTitle = p.Title,
                                      //ParentImageUrl = p.ImageUrl
                                      ParenTitle = m.Title,
                                       ParentImageUrl = m.ImageUrl
                                  }).Take(take);
              return Ok(LatestDiscourse);
        }
        [Route("FollowLivediscourses/{id}")]
        [HttpPost]
        public IActionResult FollowLivediscourses(long id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (ModelState.IsValid)
            {
                string returnText = "";
                string user = userManager.GetUserId(User);
                var search = _db.FollowLivediscourses.FirstOrDefault(a => a.FollowBy == user && a.LivediscourseId == id);
                if (search != null)
                {
                    _db.FollowLivediscourses.Remove(search);
                    _db.SaveChanges();
                    returnText = "Follow";
                }
                else
                {
                    FollowLivediscourse follow = new FollowLivediscourse();
                    follow.FollowBy = user;
                    follow.LivediscourseId = id;
                    follow.FollowOn = DateTime.UtcNow;
                    _db.FollowLivediscourses.Add(follow);
                    _db.SaveChanges();
                    returnText = "Following";
                }
                var Livediscourses = _db.Livediscourses.Find(id);
                if (Livediscourses != null)
                {
                    Livediscourses.FollowCount = _db.FollowLivediscourses.Count(a => a.LivediscourseId == id);
                    _db.Entry(Livediscourses).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                return Ok(returnText);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [Route("FollowDiscourseTag/{id}")]
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
                var search = _db.FollowTags.FirstOrDefault(a => a.FollowBy == user && a.TagId == id);
                string returnText = "";
                if (search != null)
                {
                    _db.FollowTags.Remove(search);
                    _db.SaveChanges();
                    returnText = "Follow";
                }
                else
                {
                    FollowTag follow = new FollowTag();
                    follow.FollowBy = user;
                    follow.TagId = id;
                    follow.FollowOn = DateTime.UtcNow;
                    _db.FollowTags.Add(follow);
                    _db.SaveChanges();
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
        [Route("GetChildTags/{title}/{parentId}/{page}")]
        public IActionResult GetChildTags(string title,long parentId,int page=1)
        {
            title = (title == "All") ? "" : title;
            var skipCount = (page - 1) * 20;
            var LatestDiscourse = _db.DiscourseTags.Where(a => a.Title.Contains(title) && a.ParentId == parentId).Select(a => new { a.Id, a.Title, Sector = a.DevSector.Title,a.CreatedOn }).OrderByDescending(o=>o.CreatedOn).Skip(skipCount).Take(20);
            return Ok(LatestDiscourse);
        }
        [HttpGet]
        [Route("Discourse/GetTags")]
        public IActionResult GetTags()
        {
            if (User.Identity.IsAuthenticated)
            {
                string user = userManager.GetUserId(User);
                var followedTag = _db.FollowTags.Where(a => a.FollowBy == user).Select(a => a.DiscourseTags);
                var LatestDiscourse = _db.DiscourseTags.Except(followedTag).Select(a => new { a.Id, a.Title, Sector = a.DevSector.Title, a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(15);
                return Ok(LatestDiscourse);
            }
            else
            {
                var LatestDiscourse = _db.DiscourseTags.Select(a => new { a.Id, a.Title, Sector = a.DevSector.Title, a.CreatedOn }).OrderByDescending(o => o.CreatedOn).Take(15);
                return Ok(LatestDiscourse);
            }
        }
        [Route("ReactLivediscourses/{id}/{itemType}/{reactType}")]
        [HttpPost]
        public IActionResult ReactLivediscourses(long id,ReactItemType itemType,ReactType reactType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            if (id!=0)
            {

                string user = userManager.GetUserId(User);
                var react = _db.Reacts.FirstOrDefault(a => a.ReactBy == user && a.ItemId == id && a.ReactItemType == itemType);
                if (react != null)
                {
                    react.ReactType = reactType;
                    _db.Entry(react).State = EntityState.Modified;
                    _db.SaveChanges();                    
                }
                else
                {
                    React newReact = new React();
                    newReact.ReactBy = user;
                    newReact.ItemId = id;
                    newReact.ReactItemType = itemType;
                    newReact.ReactType = reactType;
                    newReact.ReactOn = DateTime.UtcNow;
                    _db.Reacts.Add(newReact);
                    _db.SaveChanges();                   
                }
                var reacted = _db.Reacts.Where(a => a.ItemId == id && a.ReactItemType == itemType).AsEnumerable();
                var likeCount = reacted.Count(r => r.ReactType == ReactType.Like);
                var dislikeCount = reacted.Count(r => r.ReactType == ReactType.Dislike);
                var endorseCount = reacted.Count(r => r.ReactType == ReactType.Endorse);
                var rejectCount = reacted.Count(r => r.ReactType == ReactType.Reject);
                if (itemType == ReactItemType.SubBlog)
                {
                    var subBlog = _db.Livediscourses.Find(id);
                    subBlog.LikeCount = likeCount;
                    subBlog.DislikeCount = dislikeCount;
                    _db.Entry(subBlog).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else if (itemType == ReactItemType.Topic)
                {
                    var topic = _db.DiscourseTopics.Find(id);
                    topic.LikeCount = likeCount;
                    topic.DislikeCount = dislikeCount;
                    _db.Entry(topic).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else if (itemType == ReactItemType.Comment)
                {
                    var comment = _db.DiscourseComments.Find(id);
                    comment.LikeCount = likeCount;
                    comment.DislikeCount = dislikeCount;
                    comment.EndorseCount = endorseCount;
                    comment.RejectCount = rejectCount;
                    _db.Entry(comment).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                return Ok(new { rt = reactType,it= itemType, id = id, likeCount, dislikeCount, endorseCount, rejectCount });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        [Route("Discourse/GetDiscourseIndex/{id}/{page}")]
        public IActionResult GetDiscourseIndex(long id,int page)
        {
            var skipCount = (page - 1) * 20;
            var DiscourseIndexes = _db.DiscourseIndexs.Where(a => a.LivediscourseId == id).OrderBy(o => o.CreatedOn).Select(a => new { a.Id, a.Title }).Skip(skipCount).Take(20);
            return Ok(DiscourseIndexes);
        }
        [HttpGet]
        [Route("Discourse/GetSubBlogs/{id}/{page}")]
        public IActionResult GetSubBlogs(long id, int page)
        {
            var skip = (page - 1) * 20;
            var search = (_db.LiveBlogs.Where(a => a.ParentId == id).OrderByDescending(a => a.CreatedOn).Select(b=>new {
                b.Id,
                b.Title,
                b.ImageUrl,
                b.Description,
                b.CreatedOn
            })).Skip(skip).Take(20);
            return Ok(search); 
        }
        [HttpGet]
        [Route("Discourse/LivediscoursesIndexUpdates/{parentId}/{page}")]
        public IActionResult LivediscoursesIndexUpdates(long parentId, int page = 1)
        {
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = (from m in _db.DiscourseIndexs
                                    //where m.LivediscourseId == parentId
                                    //orderby m.CreatedOn
                                    select new
                                    {
                                        m.Title,
                                        m.Id,
                                        Livediscourses = _db.Livediscourses
                                        //.Where(s => s.LivediscourseIndex == m.Id && s.AdminCheck == true).OrderByDescending(o=>o.CreatedOn)
                                        .Select(s => new { s.Id, s.Title, s.CreatedOn }).Take(3)
                                    }).Skip(skipItem).Take(20);
           return Ok(discourseUpdates);
        }
        [HttpGet]
        [Route("Discourse/LivediscoursesIndexUpdatesById/{id}/{skip}")]
        public IActionResult LivediscoursesIndexUpdatesById(long id, int skip = 3)
        {
            var discourseUpdates = _db.Livediscourses.Where(s => s.LivediscourseIndex == id && s.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new { s.Id, s.Title, s.CreatedOn }).Skip(skip).Take(10);
            return Ok(discourseUpdates);
        }
        [HttpGet]
        [Route("Discourse/LivediscoursesFollower/{id}/{name}/{page}")]
        public IActionResult LivediscoursesFollower(long id,string name, int page = 1)
        {
            if (name == "All")
            {
                name = "";
            }
            var skipItem = ((page - 1) * 20);
            var discourseUpdates = (from m in _db.FollowLivediscourses
                                    where m.LivediscourseId == id && m.ApplicationUsers.FirstName.Contains(name)
                                    orderby m.FollowOn
                                    select new
                                    {
                                        m.FollowBy,
                                        m.Id,
                                        Name= m.ApplicationUsers.FirstName,
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
        [Route("Discourse/FollowerToModerator/{id}/{isMod}")]
        public IActionResult FollowerToModerator(long id, bool isMod)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("You are not Authorized.");
            }
            var FollowLivediscourses = _db.FollowLivediscourses.Find(id);
            if (FollowLivediscourses != null)
            {
                FollowLivediscourses.IsModerator = isMod;
                _db.Entry(FollowLivediscourses).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return Ok("success");
        }
        [HttpGet]
        [Route("Discourse/GetInfocusLivediscourses/{reg?}")]
        public IActionResult GetInfocusLivediscourses(string reg)
        {
            reg = reg =="Global Edition" ?"": reg;
            var InfocusLivediscourses = (from m in _db.Livediscourses
                                       where m.Region.Contains(reg) && m.ParentId == 0 && m.AdminCheck == true
                                       orderby m.CreatedOn descending
                                       select new DiscourseViewModel
                                       {
                                           Id = m.Id,
                                           Title = m.Title,
                                           ImageUrl = m.ImageUrl,
                                           children = _db.Livediscourses.Where(a=> a.ParentId == m.Id).OrderByDescending(o=>o.CreatedOn).Select(s=>new DiscourseChildViewModel { Id = s.Id, Title = s.Title}).Take(2).ToList()
                                       }).Take(3);
            return Ok(InfocusLivediscourses);
        }
        [HttpGet]
        [Route("GetModerators/{id}")]
        public IActionResult GetModerators(long id)
        {
            var moderators = _db.FollowLivediscourses.Where(a => a.LivediscourseId == id && a.IsModerator == true).Select(s=> new { Id = s.FollowBy, Name =   s.ApplicationUsers.FirstName+" "+ s.ApplicationUsers.LastName });
            return Ok(moderators);
        }
        [HttpGet]
        [Route("Discourse/GetDiscourseAmpIndex/{id}/{__amp_source_origin?}")]
        public IActionResult GetDiscourseAmpIndex(long id,string __amp_source_origin)
        {
            var returnData = _db.DiscourseIndexs.Where(a => a.LivediscourseId == id).OrderByDescending(o => o.CreatedOn).Select(s=> new { s.Id,s.Title}).ToList();
            return Ok(new { items = returnData, hasMorePages = returnData.Any() });
        }
        [HttpGet]
        [Route("Discourse/GetDiscourseIndexItems/{id}/{moreItemsPageIndex?}/{__amp_source_origin?}")]
        public IActionResult GetDiscourseIndexItems(long id, string __amp_source_origin, int? moreItemsPageIndex)
        {
            var search = _db.Livediscourses.Where(a => a.LivediscourseIndex == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(b => new DiscourseItemAmpViewModel { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn }).Take(3).ToList();
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
        [Route("Discourse/GetDiscourseUpdates/{id}/{moreItemsPageIndex?}/{__amp_source_origin?}")]
        public IActionResult GetDiscourseUpdates(long id, string __amp_source_origin, int? moreItemsPageIndex)
        {
            int pageSize = 20;
            int pageNumber = (moreItemsPageIndex ?? 1);
            var search = _db.Livediscourses.Where(a => a.ParentId == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(s => new LiveblogViewModel { Id = s.Id, Title = s.Title, ImageUrl = s.ImageUrl, Description = s.Description, CreatedOn = s.CreatedOn }).ToPagedList(pageNumber, pageSize).ToList();
            //var search = _db.Livediscourses.Where(a => a.LivediscoursesIndex == id && a.AdminCheck == true).OrderByDescending(o => o.CreatedOn).Select(b => new DiscourseItemAmpViewModel { Id = b.Id, Title = b.Title, CreatedOn = b.CreatedOn }).Take(3).ToList();
            //if (!string.IsNullOrEmpty(__amp_source_origin))
            //{
            //    HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", __amp_source_origin);
            //}
            
            var resultData = search.Select(b => new { Id = b.Id, Title = b.Title,ImageUrl = (b.ImageUrl??"").IndexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + b.ImageUrl : b.ImageUrl, Description = GetAmpHtml(b.Description), CreatedOn = b.CreatedOn.ToString("dd-MM-yyyy hh:mm:ss"),hasImage = b.ImageUrl == null ? false : true });
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
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _db != null)
        //    {
        //        _db.Dispose();
        //        _db = null;
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
