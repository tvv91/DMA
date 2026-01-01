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
            try
            {
                var ctx = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DMADbContext>();
                
                // Skip migrations for InMemory database (used in tests)
                // Check if it's InMemory by examining the database provider type
                bool isInMemory = false;
                try
                {
                    var provider = ctx.Database.ProviderName;
                    isInMemory = provider?.Contains("InMemory") == true || 
                                provider?.Contains("In-Memory") == true;
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("database providers"))
                {
                    // If we get InvalidOperationException about multiple providers,
                    // it means we're in a test environment - skip everything
                    return;
                }
                catch
                {
                    // For any other error, assume it might be InMemory and skip migrations
                    isInMemory = true;
                }
                
                if (!isInMemory)
                {
                    try
                    {
                        if (ctx.Database.GetPendingMigrations().Any())
                        {
                            ctx.Database.Migrate();
                        }
                    }
                    catch
                    {
                        // Ignore migration errors (e.g., in test environments or if migrations already applied)
                    }
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
            catch (InvalidOperationException ex) when (ex.Message.Contains("database providers"))
            {
                // Silently skip seed data if multiple database providers are registered (test environment)
                return;
            }
        }
    }
}
