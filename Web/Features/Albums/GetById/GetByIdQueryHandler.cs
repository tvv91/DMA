using MediatR;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Features.Albums.GetById;

public sealed class GetByIdQueryHandler(IAlbumService albumService) : IRequestHandler<GetByIdQuery, AlbumDetailsViewModel>
{
    public Task<AlbumDetailsViewModel> Handle(GetByIdQuery request, CancellationToken cancellationToken) =>
        albumService.GetAlbumDetailsAsync(request.Id);
}
