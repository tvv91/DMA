using MediatR;
using Web.ViewModels;

namespace Web.Features.Albums.Create;

public sealed record CreateAlbumCommand(AlbumCreateUpdateViewModel Request) : IRequest<int>;
