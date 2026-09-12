using MediatR;
using Web.Db;
using Web.ViewModels;
using Web.Features.Albums;

namespace Web.Features.Albums.GetById;

public sealed class GetByIdQueryHandler(Context context) : IRequestHandler<GetByIdQuery, AlbumDetailsViewModel>
{
    public async Task<AlbumDetailsViewModel> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var album = await AlbumFeatureHelpers.GetByIdAsync(context, request.Id);
        if (album is null) throw new KeyNotFoundException($"Album with id {request.Id} not found");
        return AlbumFeatureHelpers.ToDetails(album, await AlbumFeatureHelpers.GetReleasesAsync(context, album.Id));
    }
}
