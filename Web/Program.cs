using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Db.Implementation;
using Web.Services;
using Web.SignalRHubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DMADbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnection"]);
});
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<ITechInfoRepository, TechnicalInfoRepository>();
builder.Services.AddScoped<ICoverImageService, CoverImageService>();
builder.Services.AddSignalR();
var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapHub<DefaultHub>("/defaulthub");
SeedData.EnsurePopulated(app);
app.Run();
