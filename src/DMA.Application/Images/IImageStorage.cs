using DMA.Domain.Common;

namespace DMA.Application.Images;

public interface IImageStorage
{
    Task<string> GetUrlAsync(int id, EntityType entity, CancellationToken cancellationToken = default);
    Task SaveAsync(int id, string filename, EntityType entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(int id, EntityType entity, CancellationToken cancellationToken = default);
}
