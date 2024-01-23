using Microsoft.EntityFrameworkCore;
using Web.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DMADbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnection"]);
});
builder.Services.AddScoped<IAlbumRepository, DbRepository>();
var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
SeedData.EnsurePopulated(app);
app.Run();
