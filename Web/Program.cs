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
    var connectionString = builder.Configuration["ConnectionStrings:DbConnectionDev"];
    // Use InMemory database for testing (when connection string is empty or null)
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        opts.UseInMemoryDatabase("TestDb_Integration");
    }
    else
    {
        opts.UseSqlServer(connectionString, sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });
    }
});
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IDigitizationRepository, DigitizationRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

// Services
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IDigitizationService, DigitizationService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<EntityFindOrCreateService>();

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
    // TODO: Implement error page
    app.UseExceptionHandler("/Home/Error");
}

    app.UseStatusCodePages();
app.UseStaticFiles();
app.MapControllerRoute(name: "default", pattern: "{controller=Post}/{action=Index}");
app.MapHub<AlbumHub>("/albumhub");
app.MapHub<EquipmentHub>("/equipmenthub");
app.MapHub<PostHub>("/posthub");

// Skip seed data in testing environment or when connection string is empty (InMemory)
var connectionString = builder.Configuration["ConnectionStrings:DbConnectionDev"];
if (!string.IsNullOrWhiteSpace(connectionString) && 
    !app.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    await SeedData.EnsurePopulated(app);
}

app.Run();

