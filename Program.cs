using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Data;
using Devdiscourse.Models;
//using ImageResizer.AspNetCore.Helpers;
//using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.FileProviders;
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

app.UseAuthorization();
//app.UseStaticFiles();
//app.UseImageResizer();
//app.UseImageSharp();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//                    name: "AgroForestoryNews",
//                    pattern: "AgroForestoryNews",
//                    defaults: new { controller = "AgroForestoryNewsViewComponent", action = "InvokeAsync" });


//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=index}/{id?}");
//});

app.MapRazorPages();

app.Run();
