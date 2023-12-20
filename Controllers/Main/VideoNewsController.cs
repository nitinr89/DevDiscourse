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
using Devdiscourse.Models.VideoNewsModels;
using X.PagedList;

namespace Devdiscourse.Controllers.Main
{
    public class VideoNewsController : Controller
    {
        private readonly ApplicationDbContext db;
        public VideoNewsController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index(Guid? region, int? sector = 0, int? page = 1, string label = "0", string country = "", string source = "", string text = "", bool editorPick = false)
        {
            ViewBag.label = label;
            ViewBag.sector = sector;
            ViewBag.region = region;
            ViewBag.country = country;
            ViewBag.text = text;
            ViewBag.source = source;
            ViewBag.editorPick = editorPick;
            DateTime twoMonth = DateTime.Today.AddDays(-60);
            IQueryable<VideoNews> videoNews;
            if (string.IsNullOrEmpty(text))
            {
                videoNews = db.VideoNews.Where(a => a.CreatedOn > twoMonth);
            }
            else
            {
                videoNews = db.VideoNews.Where(a => a.Title.Contains(text) && a.CreatedOn > twoMonth);
            }
            if (label != "0")
            {
                videoNews = videoNews.Where(a => a.Label == label);
            }
            if (sector != 0)
            {
                videoNews = videoNews.Where(a => a.VideoNewsSectors.Any(s => s.SectorId == sector));
            }
            if (region != new Guid() && region != null)
            {
                videoNews = videoNews.Where(a => a.VideoNewsRegions.Any(r => r.EditionId == region));
            }
            if (!string.IsNullOrEmpty(country))
            {
                videoNews = videoNews.Where(a => a.Country.Contains(country));
            }
            if (!string.IsNullOrEmpty(source))
            {
                videoNews = videoNews.Where(a => a.Source.Contains(source));
            }
            return View(videoNews.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }

        public PartialViewResult GetVideo(long id)
        {
            var videoNews = db.VideoNews.Find(id);
            return PartialView("_getVideo", videoNews);
        }
    }
}
