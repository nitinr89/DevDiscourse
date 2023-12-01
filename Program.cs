using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Data;
using Devdiscourse.Models;
//using ImageResizer.AspNetCore.Helpers;
//using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.FileProviders;
using Devdiscourse.Utility;
using Devdiscourse.Helper;
using System.Web.Mvc;
using System.Web.Http;
using Devdiscourse.Models.BasicModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Rewrite;
//using SixLabors.ImageSharp.Web.DependencyInjection;
//using SixLabors.ImageSharp.Web.Providers.Azure;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();//To detect mobile device request from browser
                                          //builder.Services.AddImageSharp();
                                          //builder.Services.AddImageSharp();
                                          // Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://example.com") // Add the allowed origin(s)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
builder.Services.AddSingleton<IFileProvider>(physicalProvider);
var app = builder.Build();
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
builder.Configuration.SetBasePath(env.ContentRootPath)
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
      .AddEnvironmentVariables();
//builder.Services.AddSingleton<IFileProvider>(_ => new PhysicalFileProvider(env.WebRootPath ?? env.ContentRootPath));
//builder.Services.AddImageResizer();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
              name: "DefaulApi",
              pattern: "api/controller/{id?}");

app.UseAuthorization();

//app.UseStaticFiles();
//app.UseImageResizer();
//app.UseImageSharp();

// Enable CORS
app.UseCors("AllowSpecificOrigin");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers().RequireCors("AllowSpecificOrigin"); // Add this line to enable CORS for controllers
//});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.MapControllerRoute(
              name: "NewsSector",
              pattern: "news/{sector}",
              defaults: new { controller = "Search", action = "Index", sector = UrlParameter.Optional }
            );


app.MapRazorPages();

app.Run();
