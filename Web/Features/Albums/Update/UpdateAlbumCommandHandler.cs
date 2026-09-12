using MediatR;
using Web.Db;
using Web.Enums;
using Web.Features.Albums;
using Web.Interfaces;
using Web.SignalRHubs;

namespace Web.Features.Albums.Update;

public sealed class UpdateAlbumCommandHandler(
    Context context,
    TimeProvider timeProvider,
    IImageService imageService) : IRequestHandler<UpdateAlbumCommand, int>
{
    public async Task<int> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var model = request.Request;
        if (model.AlbumId <= 0)
            throw new InvalidDataException("AlbumId is invalid");
        var album = await AlbumFeatureHelpers.GetByIdAsync(context, model.AlbumId);
        if (album is null)
            throw new KeyNotFoundException($"Album {model.AlbumId} not found");

        album.Title = model.Title;
        album.UpdateDate = timeProvider.GetUtcNow().UtcDateTime;
        if (!string.IsNullOrWhiteSpace(model.Genre))
            album.GenreId = (await AlbumFeatureHelpers.FindOrCreateGenreAsync(context, model.Genre)).Id;
        if (!string.IsNullOrWhiteSpace(model.Artist))
            album.ArtistId = (await AlbumFeatureHelpers.FindOrCreateArtistAsync(context, model.Artist)).Id;
        await context.SaveChangesAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(model.AlbumCover))
            await imageService.RemoveAsync(album.Id, EntityType.AlbumCover);
        else if (model.AlbumCover != album.Id.ToString())
            await imageService.SaveAsync(album.Id, model.AlbumCover, EntityType.AlbumCover);

        AlbumHub.InvalidateAlbumCache(album.Id);
        return album.Id;
    }
}
