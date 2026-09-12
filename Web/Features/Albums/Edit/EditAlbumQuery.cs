using MediatR;
using Web.ViewModels;

namespace Web.Features.Albums.Edit;

public sealed record EditAlbumQuery(int Id) : IRequest<AlbumCreateUpdateViewModel?>;
