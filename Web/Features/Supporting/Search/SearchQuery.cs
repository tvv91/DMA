using MediatR;
using Web.Enums;

namespace Web.Features.Supporting.Search;

public sealed record SearchQuery(EntityType EntityType, string Value) : IRequest<List<AutocompleteResponse>>;
