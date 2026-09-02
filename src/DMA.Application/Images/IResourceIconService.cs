using DMA.Domain.Common;

namespace DMA.Application.Images;

public interface IResourceIconService
{
    Task<string> GetIconUrlAsync(int id, EntityType entity, CancellationToken cancellationToken = default);
}
