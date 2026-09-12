using MediatR;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Features.Albums.Edit;

public sealed class EditAlbumQueryHandler(IAlbumService albumService) : IRequestHandler<EditAlbumQuery, AlbumCreateUpdateViewModel?>
{
    public async Task<AlbumCreateUpdateViewModel?> Handle(EditAlbumQuery request, CancellationToken cancellationToken)
    {
        var album = await albumService.GetByIdAsync(request.Id);
        return album is null ? null : await albumService.MapAlbumToCreateUpdateVMAsync(album);
    }
}
