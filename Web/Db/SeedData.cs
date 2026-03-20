using Microsoft.EntityFrameworkCore;

namespace Web.Db
{
    public class SeedData
    {
        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<Context>();
            await ctx.Database.MigrateAsync();
            if (!await ctx.Albums.AnyAsync())
            {
                await ctx.Albums.AddRangeAsync(new TestData().GetAlbums());
                
            }
            if (!await ctx.PostCategories.AnyAsync())
            {
                await ctx.PostCategories.AddRangeAsync(new TestData().GetPosts());
            }
            await ctx.SaveChangesAsync();
        }
    }
}
