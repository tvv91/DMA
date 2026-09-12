using MediatR;
using Web.ViewModels;

namespace Web.Features.Albums.GetById;

public sealed record GetByIdQuery(int Id) : IRequest<AlbumDetailsViewModel>;
