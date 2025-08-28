using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Db.Implementation;
using Web.Db.Interfaces;
using Web.Services;
using Web.SignalRHubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DMADbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnectionDev"]);
});
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<ITechInfoRepository, TechnicalnfoRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddSignalR();
var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseStatusCodePages();
app.UseStaticFiles();
app.MapControllerRoute(name: "default", pattern: "{controller=Post}/{action=Index}");
app.MapHub<DefaultHub>("/defaulthub");
await SeedData.EnsurePopulated(app);
app.Run();
