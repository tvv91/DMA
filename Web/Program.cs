using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Db.Implementation;
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
builder.Services.AddSignalR();
var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapHub<DefaultHub>("/defaulthub");
await SeedData.EnsurePopulated(app);
app.Run();
