using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers
{
    public class SectorNewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SectorNewsController(ApplicationDbContext db)
        {
            _db = db;
        }
     
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
