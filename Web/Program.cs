using DMA.Application;
using DMA.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Db;
using Web.Interfaces;
using Web.Services;
using Web.SignalRHubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddSingleton(TimeProvider.System);
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

builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IReleaseService, ReleaseService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<IEntityFindOrCreateService, EntityFindOrCreateService>();
builder.Services.AddHostedService<TempImageCleanupService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
}).AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
var app = builder.Build();

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
