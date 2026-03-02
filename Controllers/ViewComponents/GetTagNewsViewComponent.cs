using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class GetTagNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        public GetTagNewsViewComponent(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }
        public async Task<IViewComponentResult> InvokeAsync(long id, string tag, string sector)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tag)) return View(new List<LatestNewsView>());
                var tagList = tag.Split(',').Take(3).ToList();
                string tag1 = tagList[0] ?? "&&&&&&&&&&&&";
                string tag2 = tagList.Count > 1 ? tagList[1] ?? "&&&&&&&&&&&&" : "&&&&&&&&&&&&";
                string tag3 = tagList.Count > 2 ? tagList[2] ?? "&&&&&&&&&&&&" : "&&&&&&&&&&&&";

                // Cache key based on article id + tags + sector
                string cacheKey = $"vc:tagnews:{id}:{tag1}:{tag2}:{tag3}:{sector}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached != null)
                {
                    var cachedResult = JsonSerializer.Deserialize<List<LatestNewsView>>(cached);
                    if (cachedResult != null) return View(cachedResult);
                }

                DateTime onemonth = DateTime.Today.AddDays(-30);
                List<LatestNewsView> finalResult;

                if (!string.IsNullOrEmpty(sector))
                {
                    string sqlQuery = @"SELECT TOP (5) *
                                            FROM DevNews
                                            WHERE CreatedOn > @onemonth 
                                            AND NewsId != @id 
                                            AND AdminCheck = 1 
                                            AND Sector = @sec
                                            AND Title IS NOT NULL 
                                            AND (Title LIKE '%' + @tag1 + '%' 
                                                OR Title LIKE '%' + @tag2 + '%' 
                                                OR Title LIKE '%' + @tag3 + '%')
                                            ORDER BY CreatedOn DESC";
                    var result = await _db.DevNews
                                            .FromSqlRaw(sqlQuery, new SqlParameter("@onemonth", onemonth),
                                                                  new SqlParameter("@id", id),
                                                                  new SqlParameter("@sec", sector),
                                                                  new SqlParameter("@tag1", tag1),
                                                                  new SqlParameter("@tag2", tag2),
                                                                  new SqlParameter("@tag3", tag3)).ToListAsync();
                    finalResult = result.Select(n => new LatestNewsView
                    {
                        Title = n.Title,
                        NewId = n.NewsId,
                        Label = n.NewsLabels
                    }).ToList();
                }
                else
                {
                    string sqlQuery = @"SELECT TOP (5) *
                                            FROM DevNews
                                            WHERE CreatedOn > @onemonth 
                                            AND NewsId != @id 
                                            AND AdminCheck = 1 
                                            AND Title IS NOT NULL 
                                            AND (Title LIKE '%' + @tag1 + '%' 
                                                OR Title LIKE '%' + @tag2 + '%' 
                                                OR Title LIKE '%' + @tag3 + '%')
                                            ORDER BY CreatedOn DESC";
                    var result = await _db.DevNews
                                             .FromSqlRaw(sqlQuery, new SqlParameter("@onemonth", onemonth),
                                                                   new SqlParameter("@id", id),
                                                                   new SqlParameter("@tag1", tag1),
                                                                   new SqlParameter("@tag2", tag2),
                                                                   new SqlParameter("@tag3", tag3)).ToListAsync();
                    finalResult = result.Select(n => new LatestNewsView
                    {
                        Title = n.Title,
                        NewId = n.NewsId,
                        Label = n.NewsLabels
                    }).ToList();
                }

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(finalResult), _cacheOptions);
                return View(finalResult);
            }
            catch (Exception _)
            {
                return View(new List<LatestNewsView>());
            }
        }
    }
}
