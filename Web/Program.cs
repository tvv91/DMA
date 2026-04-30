using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Interfaces;
using Web.Services;
using Web.SignalRHubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDbContext<Context>(opts =>
{
    var connectionString = builder.Configuration["ConnectionStrings:DbConnectionDev"];
    opts.UseSqlServer(connectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});
builder.Services.AddScoped<IImageService, LocalStorageImageService>();
builder.Services.AddScoped<IResourceIconService, LocalResourceIconService>();

// Services
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IDigitizationService, DigitizationService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<IEntityFindOrCreateService, EntityFindOrCreateService>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
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
app.MapControllerRoute(name: "default", pattern: "{controller=Post}/{action=Index}");
app.MapHub<AlbumHub>("/albumhub");
app.MapHub<EquipmentHub>("/equipmenthub");
app.MapHub<PostHub>("/posthub");

await SeedData.EnsurePopulated(app);
await app.RunAsync();

