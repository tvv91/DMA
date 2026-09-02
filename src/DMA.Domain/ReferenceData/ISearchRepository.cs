using DMA.Domain.Common;

namespace DMA.Domain.ReferenceData;

public interface ISearchRepository
{
    Task<List<AutocompleteItem>> SearchAsync(EntityType entityType, string value, CancellationToken cancellationToken = default);
}
