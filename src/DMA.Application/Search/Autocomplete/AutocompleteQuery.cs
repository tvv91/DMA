using DMA.Application.Search;
using DMA.Domain.Common;
using MediatR;

namespace DMA.Application.Search.Autocomplete;

public sealed record AutocompleteQuery(EntityType EntityType, string Value) : IRequest<List<AutocompleteDto>>;
