using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class EntityFindOrCreateServiceTestFactory : IDisposable
{
    private readonly TestMediatorContext _mediatorContext;

    public Context Context { get; }
    public EntityFindOrCreateService Service { get; }

    public EntityFindOrCreateServiceTestFactory()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        _mediatorContext = MediatorTestHelper.Create(Context);
        Service = new EntityFindOrCreateService(_mediatorContext);
    }

    public void Dispose()
    {
        _mediatorContext.Dispose();
        Context.Dispose();
    }
}
