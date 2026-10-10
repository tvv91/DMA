using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Serilog;
using Web.Infrastructure.Persistence;
using Web.Infrastructure.Icons;
using Web.Infrastructure.Storage;
using Web.Models;
using Web.Infrastructure.Cleanup;
using Web.Infrastructure.Security;
using Web.Hubs;
using Web.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAdminAuthorization, AdminAuthorization>();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDbContext<Context>(opts =>
{
    var connectionString = builder.Configuration["ConnectionStrings:DbConnectionDev"];
    opts.UseSqlServer(connectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<Context>()
.AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/accessdenied";
});
builder.Services.AddScoped<IImageService, LocalStorageImageService>();
builder.Services.AddScoped<IResourceIconService, LocalResourceIconService>();
builder.Services.AddSingleton<HtmlContentSanitizer>();

builder.Services.AddHostedService<TempImageCleanupService>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
}).AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ru"), new CultureInfo("uk") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders =
    [
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    ]
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStatusCodePages();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Post}/{action=Index}");
app.MapHub<AlbumHub>("/albumhub");
app.MapHub<EquipmentHub>("/equipmenthub");
app.MapHub<PostHub>("/posthub");

await SeedData.EnsurePopulated(app);
await app.RunAsync();

