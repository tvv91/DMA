using MediatR;

namespace Web.Features.Albums.Delete;

public sealed record DeleteAlbumCommand(int Id) : IRequest<bool>;
