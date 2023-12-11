using System.Data;
using System.Net;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devdiscourse.Data;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;

namespace DevDiscourse.Controllers.Main
{
    public class InfocusController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public InfocusController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        // GET: Infocus
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Index(string ed = "", string type = "")
        {
            var search = from m in db.Infocus
                         //join s in db.DevNews on m.NewsId equals s.NewsId
                         select new InfocusView
                         {
                             //Title = s.Title,
                             Title = "News Title",
                             Type = m.ItemType,
                             Id = m.Id,
                             SrNo = m.SrNo,
                             Creator = m.ApplicationUsers.FirstName,
                             Edition = m.Edition,
                             NewsId = m.NewsId
                         };
            if (!string.IsNullOrEmpty(ed))
            {
                search = search.Where(a => a.Edition.Contains(ed));
            }
            if (!string.IsNullOrEmpty(type))
            {
                search = search.Where(a => a.Type.Contains(type));
            }
            ViewBag.ed = ed;
            ViewBag.type = type;
            return View(search.OrderBy(a => a.SrNo).ToList());
        }

        // GET: Infocus/Details/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Infocus? infocus = db.Infocus.Find(id);
            if (infocus == null)
            {
                return NotFound();
            }
            return View(infocus);
        }

        // GET: Infocus/Create
        // ft : filter to go back
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Create(long id, string ft = "")
        {
            ViewBag.ft = ft;
            var newsData = db.DevNews.Where(a => a.NewsId == id).FirstOrDefault();
            ViewBag.newsTitle = newsData?.Title;
            if (!string.IsNullOrEmpty(newsData?.Region))
            {
                ViewBag.editionList = newsData.Region.Split(',').ToList();
            }
            else
            {
                ViewBag.editionList = "";
            }
            ViewBag.edition = newsData?.Region;
            TempData["NewsId"] = id;
            TempData["ft"] = ft;
            return View();
        }

        // POST: Infocus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,SrNo,Edition")] Infocus infocus)
        {
            string _type = TempData["ft"].ToString();
            long _itemId = long.Parse(TempData["NewsId"].ToString());
            var newsData = await db.DevNews.Where(a => a.NewsId == _itemId).FirstOrDefaultAsync();
            switch (_type)
            {
                case "evt":
                    infocus.ItemType = "Event";
                    break;
                case "nws":
                    infocus.ItemType = "News";
                    break;
                default:
                    infocus.ItemType = "Blog";
                    break;
            }
            // Send Notification
            string description = Regex.Replace(newsData.Description, @"<[^>]+>|&nbsp;", "").Trim();
            if (description.Length > 150)
            {
                description = description.Substring(0, 150) + "...";
            }
            if (ModelState.IsValid)
            {
                List<string> editionList = new List<string>();
                var region = infocus.Edition.Split(',').ToList();
                foreach (var edition in region)
                {
                    var newobj = await db.Infocus.FirstOrDefaultAsync(a => a.SrNo == infocus.SrNo && a.Edition == edition && a.ItemType == infocus.ItemType);
                    if (newobj != null)
                    {
                        newobj.NewsId = _itemId;
                        newobj.SrNo = infocus.SrNo;
                        newobj.ItemType = infocus.ItemType;
                        newobj.Edition = edition;
                        newobj.Creator = userManager.GetUserId(User);
                        newobj.CreatedOn = DateTime.UtcNow;
                        db.Entry(newobj).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        Infocus newsobj = new Infocus
                        {
                            NewsId = _itemId,
                            SrNo = infocus.SrNo,
                            Creator = userManager.GetUserId(User),
                            ItemType = infocus.ItemType,
                            Edition = edition,
                            CreatedOn = DateTime.UtcNow
                        };
                        db.Infocus.Add(newsobj);
                        await db.SaveChangesAsync();
                    }
                }
                if (TempData["ft"].ToString() == "evt")
                {
                    return RedirectToAction("Index", "Events");
                }
                else if (TempData["ft"].ToString() == "nws")
                {
                    return RedirectToAction("Index", "DevNews");
                }
                else if (TempData["ft"].ToString() == "authnws")
                {
                    return RedirectToAction("NewsList", "DevNews");
                }
                else
                {
                    return RedirectToAction("Blog", "DevNews");
                }
            }
            TempData["NewsId"] = _itemId;
            TempData["ft"] = _type;
            ViewBag.ft = _type;
            // return
            ViewBag.newsTitle = newsData.Title;
            ViewBag.edition = newsData.Region;
            ViewBag.editionList = newsData.Region.Split(',').ToList();
            return View(infocus);
        }
        public async Task<bool> IsAlreadyInInfocus(long id, string itemtype, string edition)
        {
            var search = await db.Infocus.CountAsync(a => a.ItemType == itemtype && a.Edition == edition && a.NewsId == id);
            return search > 0 ? true : false; ;
        }
        public async Task<string> UpdateOldInfocusNewsOnAdd(string edition, int order)
        {
            var infocsData = await db.Infocus.Where(a => a.Edition == edition && a.SrNo >= order).ToListAsync();
            foreach (var item in infocsData)
            {
                if (item.SrNo >= 6)
                {
                    db.Infocus.Remove(item);
                }
                else
                {
                    item.SrNo = item.SrNo + 1;
                    db.Entry(item).State = EntityState.Modified;
                }
            }
            await db.SaveChangesAsync();
            return "Ok";
        }
        public async Task<string> AddToRelatedEditionInfocus(long ItemId, string ItemType)
        {
            var search = await db.DevNews.FirstOrDefaultAsync(a => a.NewsId == ItemId);
            var region = search.Region.Split(',').ToList();
            foreach (var edition in region)
            {
                if (edition != "Global Edition")
                {
                    string result = await UpdateOldInfocusNewsOnAdd(edition, 1);
                    Infocus newobj = new Infocus
                    {
                        NewsId = ItemId,
                        SrNo = 1,
                        ItemType = ItemType,
                        Edition = edition,
                        CreatedOn = DateTime.UtcNow
                    };
                    db.Infocus.Add(newobj);
                    await db.SaveChangesAsync();
                }
            }
            return "Ok";
        }
        public async Task SendMessage(string title, string desc, string newsId, string ImageUrl)
        {
            try
            {
                var applicationID = "AAAAS31kOEA:APA91bGZqeQdyNHF4kXD1VzfimttW6BiJywDm-x74Fesf_6cRKSR2_Jm7aFTrzszZpn6weqFkbV5sq53X8Qkpinm5TdEKpepkwzjOaKP37DKoi95OB1YSFpCQ7WeoekFZufR-klVb6BV";
                var senderId = "324226267200";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = "/topics/news",
                    notification = new
                    {
                        title = title,
                        body = desc,
                        icon = newsId
                    },
                    data = new
                    {
                        newsid = newsId,
                        description = desc,
                        newsImage = ImageUrl
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            await Response.WriteAsync(sResponseFromServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Response.WriteAsync(ex.Message);
            }
        }
        // GET: Infocus/Edit/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Infocus? infocus = db.Infocus.Find(id);
            TempData["OldSr"] = infocus?.SrNo;
            TempData["OldEdition"] = infocus?.Edition;
            ViewBag.newsTitle = db.DevNews.Where(a => a.NewsId == infocus.NewsId).FirstOrDefault()?.Title;
            if (infocus == null)
            {
                return NotFound();
            }
            return View(infocus);
        }

        // POST: Infocus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,NewsId,SrNo,Edition,ItemType,CreatedOn")] Infocus infocus)
        {
            string OldEdition = TempData["OldEdition"].ToString();
            long OldSr = long.Parse(TempData["OldSr"].ToString());
            if (ModelState.IsValid)
            {
                //var oldInfocus = db.Infocus.Find(infocus.Id);
                if (OldSr < infocus.SrNo)
                {
                    var infocusGreater = db.Infocus.Where(a => a.Edition == infocus.Edition && a.SrNo > OldSr && a.SrNo <= infocus.SrNo);
                    foreach (var itemGreater in infocusGreater.ToList())
                    {
                        itemGreater.SrNo = itemGreater.SrNo - 1;
                        db.Entry(itemGreater).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                else if (OldSr > infocus.SrNo)
                {
                    var infocusLess = db.Infocus.Where(a => a.Edition == infocus.Edition && a.SrNo < OldSr && a.SrNo >= infocus.SrNo);
                    foreach (var itemLess in infocusLess.ToList())
                    {
                        itemLess.SrNo = itemLess.SrNo + 1;
                        db.Entry(itemLess).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                db.Entry(infocus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData["OldSr"] = infocus.SrNo;
            TempData["OldEdition"] = infocus.Edition;
            ViewBag.newsTitle = db.DevNews.Where(a => a.NewsId == infocus.NewsId).FirstOrDefault()?.Title;
            return View(infocus);
        }

        // GET: Infocus/Delete/5
        [Authorize(Roles = "SuperAdmin,Admin,Author,Upfront")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Infocus? infocus = db.Infocus.Find(id);
            if (infocus == null)
            {
                return NotFound();
            }
            return View(infocus);
        }

        // POST: Infocus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Infocus? infocus = db.Infocus.Find(id);
            if (infocus == null)
            {
                return NotFound();
            }
            var infocsData = db.Infocus.Where(a => a.Edition == infocus.Edition && a.SrNo >= infocus.SrNo && a.ItemType == infocus.ItemType).ToList();
            foreach (var item in infocsData)
            {
                item.SrNo = item.SrNo - 1;
                db.Entry(item).State = EntityState.Modified;
            }
            db.Infocus.Remove(infocus);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<JsonResult> CreateInfocus(string type, string editions, long newsId)
        {
            string result = "";
            var regions = editions.Split(',').ToList();
            foreach (var edition in regions)
            {
                var newobj = await db.Infocus.FirstOrDefaultAsync(a => a.NewsId == newsId && a.Edition == edition && a.ItemType == type);
                if (newobj == null)
                {
                    var dataForIncement = db.Infocus.Where(a => a.Edition == edition && a.ItemType == type).ToList();
                    if (dataForIncement.Any())
                    {
                        foreach (var item in dataForIncement)
                        {
                            item.SrNo = item.SrNo + 1;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        await db.SaveChangesAsync();
                    }
                    Infocus obj = new Infocus()
                    {
                        NewsId = newsId,
                        SrNo = 1,
                        ItemType = type,
                        Edition = edition,
                        Creator = userManager.GetUserId(User),
                        CreatedOn = DateTime.UtcNow,
                    };
                    db.Infocus.Add(obj);
                    await db.SaveChangesAsync();
                    var infocusData = db.Infocus.Where(a => a.SrNo > 10);
                    if (infocusData.Any())
                    {
                        db.Infocus.RemoveRange(infocusData);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    result = result != "" ? result + "," + edition : edition;
                }
            }
            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
