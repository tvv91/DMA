using Microsoft.EntityFrameworkCore;

namespace Web.Db
{
    /// <summary>
    /// Initial data
    /// </summary>
    public class SeedData
    {
        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            var ctx = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DMADbContext>();
            if (ctx.Database.GetPendingMigrations().Any())
            {
                ctx.Database.Migrate();
            }
            if (!ctx.Albums.Any())
            {
                await ctx.Albums.AddRangeAsync(TestData.GetAlbums());
            }
            await ctx.SaveChangesAsync();
        }
    }
}
