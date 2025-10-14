using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Implementation;
using Web.Interfaces;
using Web.Services;
using Web.SignalRHubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DMADbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnectionDev"], sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IDigitizationRepository, DigitizationRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPostCategoryRepository, PostCategoryRepository>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

builder.Services.AddSignalR();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
} 
else
{
    // TODO: Implement error page
    app.UseExceptionHandler("/Home/Error");
}

    app.UseStatusCodePages();
app.UseStaticFiles();
app.MapControllerRoute(name: "default", pattern: "{controller=Post}/{action=Index}");
// TODO: Separate hubs
app.MapHub<DefaultHub>("/defaulthub");
await SeedData.EnsurePopulated(app);
app.Run();
