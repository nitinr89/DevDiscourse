using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class SectorViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        public SectorViewComponent(ApplicationDbContext _db, IDistributedCache cache)
        {
            this._db = _db;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sector, string reg = "", string filter = "")
        {
            try
            {
                ViewBag.reg = reg;
                ViewBag.filter = filter;
                if (!string.IsNullOrEmpty(sector))
                {
                    List<int> idList = new();
                    var stringidList = sector.Split(',');
                    foreach (var id in stringidList)
                    {
                        bool result = int.TryParse(id, out int number);
                        if (result) idList.Add(number);
                    }

                    // Cache all sectors (they rarely change)
                    const string cacheKey = "vc:sectors:all";
                    List<ItemView>? allSectors = null;
                    var cached = await _cache.GetStringAsync(cacheKey);
                    if (cached != null)
                    {
                        allSectors = JsonSerializer.Deserialize<List<ItemView>>(cached);
                    }
                    if (allSectors == null)
                    {
                        var devSectors = await _db.DevSectors
                                            .Where(m => m.Id != 8 && m.Id != 16)
                                            .ToListAsync();
                        allSectors = devSectors.Select(m => new ItemView
                        {
                            Id = m.Id,
                            Title = m.Title ?? "",
                        }).ToList();
                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(allSectors), _cacheOptions);
                    }

                    var finteredSector = allSectors.Where(m => idList.Contains(m.Id)).ToList();
                    if (filter == "Single")
                    {
                        finteredSector = finteredSector.Take(1).ToList();
                    }
                    return View(finteredSector.OrderBy(a => a.Title));
                }
                return View();
            }
            catch (Exception _)
            {
                return View(new List<ItemView> { new() { Id = 0, Title = "" } });
            }
        }
    }
}
