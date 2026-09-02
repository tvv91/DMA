using DMA.Domain.Common;

namespace DMA.Infrastructure.Persistence.Repositories;

public class UnitOfWork(DmaDbContext context) : IUnitOfWork
{
    private readonly DmaDbContext _context = context;

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
