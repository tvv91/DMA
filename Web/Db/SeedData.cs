using Microsoft.EntityFrameworkCore;

namespace Web.Db
{
    /// <summary>
    /// Initial data (just for test purposes)
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
                await ctx.Albums.AddRangeAsync(new TestData().GetAlbums());
            }

            if (!ctx.PostCategories.Any())
            {
                await ctx.PostCategories.AddRangeAsync(new TestData().GetPosts());
            }
            
            await ctx.SaveChangesAsync();
        }
    }
}
