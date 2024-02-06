using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Db.Implementation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DMADbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnection"]);
});
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<ITechInfoRepository, TechnicalInfoRepository>();
var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
SeedData.EnsurePopulated(app);
app.Run();
