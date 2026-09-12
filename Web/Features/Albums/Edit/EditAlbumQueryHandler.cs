using MediatR;
using Web.Db;
using Web.Enums;
using Web.Features.Albums;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Features.Albums.Edit;

public sealed class EditAlbumQueryHandler(Context context, IImageService imageService) : IRequestHandler<EditAlbumQuery, AlbumCreateUpdateViewModel?>
{
    public async Task<AlbumCreateUpdateViewModel?> Handle(EditAlbumQuery request, CancellationToken cancellationToken)
    {
        var album = await AlbumFeatureHelpers.GetByIdAsync(context, request.Id);
        if (album is null) return null;
        var coverUrl = await imageService.GetUrlAsync(album.Id, EntityType.AlbumCover);
        return new AlbumCreateUpdateViewModel
        {
            AlbumId = album.Id,
            Title = album.Title,
            Artist = album.Artist?.Name ?? string.Empty,
            Genre = album.Genre?.Name ?? string.Empty,
            AlbumCover = coverUrl.Contains("nocover") ? null : album.Id.ToString(),
            Action = ActionType.Update,
            Releases = await AlbumFeatureHelpers.GetReleasesAsync(context, album.Id)
        };
    }
}
