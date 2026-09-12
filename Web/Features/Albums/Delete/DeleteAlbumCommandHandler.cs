using MediatR;
using Web.Db;
using Web.Enums;
using Web.Interfaces;

namespace Web.Features.Albums.Delete;

public sealed class DeleteAlbumCommandHandler(
    Context context,
    IImageService imageService) : IRequestHandler<DeleteAlbumCommand, bool>
{
    public async Task<bool> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await context.Albums.FindAsync([request.Id], cancellationToken);
        if (album is null)
            return false;

        context.Albums.Remove(album);
        await context.SaveChangesAsync(cancellationToken);
        await imageService.RemoveAsync(request.Id, EntityType.AlbumCover);
        return true;
    }
}
