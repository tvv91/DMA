using DMA.Application.Search;
using DMA.Domain.ReferenceData;
using MediatR;

namespace DMA.Application.Search.Autocomplete;

public sealed class AutocompleteQueryHandler(ISearchRepository searchRepository)
    : IRequestHandler<AutocompleteQuery, List<AutocompleteDto>>
{
    public async Task<List<AutocompleteDto>> Handle(AutocompleteQuery request, CancellationToken cancellationToken)
    {
        var results = await searchRepository.SearchAsync(request.EntityType, request.Value, cancellationToken);
        return results.ConvertAll(item => new AutocompleteDto
        {
            Label = item.Label,
            Value = item.Value
        });
    }
}
