using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            Context ctx = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<Context>();
            if (ctx.Database.GetPendingMigrations().Any())
            {
                ctx.Database.Migrate();
            }
        }
    }
}
