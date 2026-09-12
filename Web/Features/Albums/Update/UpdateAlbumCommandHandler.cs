using MediatR;
using Web.Enums;
using Web.Interfaces;
using Web.SignalRHubs;

namespace Web.Features.Albums.Update;

public sealed class UpdateAlbumCommandHandler(
    IAlbumService albumService,
    IImageService imageService) : IRequestHandler<UpdateAlbumCommand, int>
{
    public async Task<int> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var model = request.Request;
        var album = await albumService.UpdateAlbumAsync(model.AlbumId, model.Title, model.Artist, model.Genre);

        if (string.IsNullOrWhiteSpace(model.AlbumCover))
            await imageService.RemoveAsync(album.Id, EntityType.AlbumCover);
        else if (model.AlbumCover != album.Id.ToString())
            await imageService.SaveAsync(album.Id, model.AlbumCover, EntityType.AlbumCover);

        AlbumHub.InvalidateAlbumCache(album.Id);
        return album.Id;
    }
}
