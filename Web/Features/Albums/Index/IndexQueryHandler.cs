using MediatR;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Features.Albums.Index;

public sealed class IndexQueryHandler(IAlbumService albumService) : IRequestHandler<IndexQuery, AlbumIndexViewModel>
{
    public async Task<AlbumIndexViewModel> Handle(IndexQuery request, CancellationToken cancellationToken)
    {
        var result = await albumService.GetIndexListAsync(
            request.Page,
            request.PageSize,
            request.ArtistName,
            request.GenreName,
            request.YearValue,
            request.AlbumTitle);

        return new AlbumIndexViewModel
        {
            CurrentPage = request.Page,
            PageCount = result.TotalPages,
            Albums = result.Items,
            PageSize = request.PageSize,
            HasAnyAlbumsInDb = await albumService.HasAnyAlbumsAsync(),
            ArtistName = request.ArtistName,
            GenreName = request.GenreName,
            YearValue = request.YearValue,
            AlbumTitle = request.AlbumTitle
        };
    }
}
