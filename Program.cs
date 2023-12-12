using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Data;
using Devdiscourse.Models;
//using ImageResizer.AspNetCore.Helpers;
//using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.FileProviders;
using Devdiscourse.Helper;
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

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<SeoFriendlySecondRoute>();
// Call the custom route configuration method
app.UseEndpoints(endpoints =>
{
    RouteConfig.ConfigureRoutes(endpoints);
});

//app.UseStaticFiles();
//app.UseImageResizer();
//app.UseImageSharp();

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.MapRazorPages();

app.Run();
