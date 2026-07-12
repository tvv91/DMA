using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class EntityFindOrCreateServiceTestFactory : IDisposable
{
    public Context Context { get; }
    public EntityFindOrCreateService Service { get; }

    public EntityFindOrCreateServiceTestFactory()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        Service = new EntityFindOrCreateService(Context);
    }

    public void Dispose() => Context.Dispose();
}
