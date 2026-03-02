using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Devdiscourse.Data;
using Devdiscourse.Models;
using Microsoft.Extensions.FileProviders;
using Devdiscourse.Helper;
using Devdiscourse.Hubs;
using DNTCaptcha.Core;
using Devdiscourse.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SitemapService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IpAddressHelper>();
builder.Services.Configure<PasswordHasherOptions>(o =>
{
    o.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://devdiscourse.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Cache());
    options.AddPolicy("MyOutputCachePolicy", MyOutputCachePolicy.Instance);
});
var redisConnectionString =
    builder.Configuration.GetConnectionString("RedisOutputCache") ??
    builder.Configuration["Redis:OutputCacheConnectionString"];
if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisOutputCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "devdiscourse-outputcache";
    });
    // IDistributedCache backed by Redis — used by ViewComponents & controllers
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "devdiscourse-cache:";
    });
}
else
{
    // Fallback: in-memory distributed cache when Redis is not configured
    builder.Services.AddDistributedMemoryCache();
}
builder.Services.AddSingleton<IViewTrackingQueue, ViewTrackingQueue>();
builder.Services.AddHostedService(sp => (ViewTrackingQueue)sp.GetRequiredService<IViewTrackingQueue>());

builder.Services.AddDNTCaptcha(option =>
{
    option.UseCookieStorageProvider().ShowThousandsSeparators(false);
    option.WithEncryptionKey("thisisasecretkey@12345");
});

IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
builder.Services.AddSingleton<IFileProvider>(physicalProvider);

var app = builder.Build();
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
builder.Configuration.SetBasePath(env.ContentRootPath)
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
      .AddEnvironmentVariables();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseOutputCache();
app.UseMiddleware<SeoFriendlySecondRoute>();
RouteConfig.ConfigureRoutes(app);
app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();
