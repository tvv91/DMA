using Microsoft.EntityFrameworkCore;
using Web.Db;

namespace Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static DMADbContext CreateInMemoryContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new DMADbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static void Cleanup(DMADbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}


