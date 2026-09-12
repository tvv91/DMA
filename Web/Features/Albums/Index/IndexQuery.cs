using MediatR;
using Web.ViewModels;

namespace Web.Features.Albums.Index;

public sealed record IndexQuery(
    int Page,
    int PageSize,
    string? ArtistName,
    string? GenreName,
    string? YearValue,
    string? AlbumTitle) : IRequest<AlbumIndexViewModel>;
