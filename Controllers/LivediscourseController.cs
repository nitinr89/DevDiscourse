using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Models;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Devdiscourse.Data;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace Devdiscourse.Controllers
{
    public class LivediscourseController : Controller, IDisposable
    {
        private ApplicationDbContext _db;
        public LivediscourseController(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        // GET: Livediscourse
        [Authorize(Roles = "SuperAdmin,Admin,Upfront")]
        // GET: DevNews
        public ActionResult Index(DateTime? bfd, DateTime? afd, int? page = 1, string region = "", string label = "0", string sector = "0", string category = "0", string country = "", string source = "", string text = "", string uid = "", bool editorPick = false)
        {
            ViewBag.sector = sector ?? "";
            ViewBag.region = region ?? "";
            ViewBag.country = country ?? "";
            ViewBag.text = text ?? "";
            ViewBag.afDate = afd;
            ViewBag.bfDate = bfd;
            DateTime fifteenDay = DateTime.Today.AddDays(-115);
            var livediscourse = _db.Livediscourses.Where(a => a.Title.Contains(text) && a.Region.Contains(region) && a.Country.Contains(country) && a.ParentId == 0).AsEnumerable();

            if (sector != "0")
            {
                livediscourse = livediscourse.Where(a => a.Sector.Contains("," + sector + ",") || a.Sector.StartsWith("," + sector) || a.Sector.EndsWith(sector + ",") || a.Sector.Equals(sector));
            }
            if (bfd != null)
            {
                DateTime filterDate = DateTime.Parse(bfd.ToString());
                livediscourse = livediscourse.Where(a => a.CreatedOn < filterDate);
            }
            if (afd != null)
            {
                DateTime filterDate2 = DateTime.Parse(afd.ToString());
                livediscourse = livediscourse.Where(a => a.CreatedOn > filterDate2);
            }
            return View(livediscourse.OrderByDescending(a => a.CreatedOn).ToPagedList((page ?? 1), 10));
        }
    }
}
