using MediatR;
using Web.Enums;
using Web.Interfaces;

namespace Web.Features.Albums.Delete;

public sealed class DeleteAlbumCommandHandler(
    IAlbumService albumService,
    IImageService imageService) : IRequestHandler<DeleteAlbumCommand, bool>
{
    public async Task<bool> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        if (!await albumService.DeleteAlbumAsync(request.Id))
            return false;

        await imageService.RemoveAsync(request.Id, EntityType.AlbumCover);
        return true;
    }
}
