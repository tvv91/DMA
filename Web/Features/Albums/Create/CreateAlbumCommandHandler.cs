using MediatR;
using Web.Enums;
using Web.Db;
using Web.Features.Albums;
using Web.Interfaces;

namespace Web.Features.Albums.Create;

public sealed class CreateAlbumCommandHandler(
    Context context,
    TimeProvider timeProvider,
    IImageService imageService) : IRequestHandler<CreateAlbumCommand, int>
{
    public async Task<int> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await AlbumFeatureHelpers.CreateOrFindAsync(context, timeProvider,
            request.Request.Title,
            request.Request.Artist,
            request.Request.Genre);

        if (request.Request.AlbumCover is not null)
            await imageService.SaveAsync(album.Id, request.Request.AlbumCover, EntityType.AlbumCover);

        return album.Id;
    }
}
