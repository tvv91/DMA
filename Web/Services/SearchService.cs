using DMA.Application.Search.Autocomplete;
using MediatR;
using Web.Interfaces;
using Web.Response;

namespace Web.Services;

public class SearchService(IMediator mediator) : ISearchService
{
    public async Task<List<AutocompleteResponse>> SearchAsync(EntityType entityType, string value)
    {
        var results = await mediator.Send(new AutocompleteQuery(entityType, value));
        return results.ConvertAll(dto => new AutocompleteResponse { Label = dto.Label, Value = dto.Value });
    }
}
