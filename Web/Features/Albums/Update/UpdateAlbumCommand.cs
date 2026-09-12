using MediatR;
using Web.ViewModels;

namespace Web.Features.Albums.Update;

public sealed record UpdateAlbumCommand(AlbumCreateUpdateViewModel Request) : IRequest<int>;
