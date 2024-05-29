using Devdiscourse.Data;
using Devdiscourse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Devdiscourse.Controllers.ViewComponents
{
    public class GetTagNewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public GetTagNewsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(long id, string tag, string sector)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tag)) return View(new List<LatestNewsView>());
                var tagList = tag.Split(',').Take(3).ToList();
                DateTime onemonth = DateTime.Today.AddDays(-30);
                string tag1 = tagList[0] ?? "&&&&&&&&&&&&";
                string tag2 = tagList[1] ?? "&&&&&&&&&&&&";
                string tag3 = tagList[2] ?? "&&&&&&&&&&&&";
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
                    var finalResult = result.Select(n => new LatestNewsView
                    {
                        Title = n.Title,
                        NewId = n.NewsId,
                        Label = n.NewsLabels
                    }).ToList();
                    return View(finalResult);
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
                    var finalResult = result.Select(n => new LatestNewsView
                    {
                        Title = n.Title,
                        NewId = n.NewsId,
                        Label = n.NewsLabels
                    }).ToList();
                    return View(finalResult);
                }
            }
            catch (Exception _)
            {
                return View(new List<LatestNewsView>());
            }
        }
    }
}
