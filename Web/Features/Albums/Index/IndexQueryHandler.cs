using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Persistence;
using Web.Extensions;
using Web.ViewModels;
using Web.Features.Albums;

namespace Web.Features.Albums.Index;

public sealed class IndexQueryHandler(Context context) : IRequestHandler<IndexQuery, AlbumIndexViewModel>
{
    public async Task<AlbumIndexViewModel> Handle(IndexQuery request, CancellationToken cancellationToken)
    {
        var query = context.Albums.Include(a => a.Artist).Include(a => a.Genre).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.ArtistName)) query = query.Where(a => a.Artist != null && a.Artist.Name.Contains(request.ArtistName));
        if (!string.IsNullOrWhiteSpace(request.GenreName)) query = query.Where(a => a.Genre != null && a.Genre.Name.Contains(request.GenreName));
        if (!string.IsNullOrWhiteSpace(request.AlbumTitle)) query = query.Where(a => a.Title.Contains(request.AlbumTitle));
        if (!string.IsNullOrWhiteSpace(request.YearValue))
        {
            query = int.TryParse(request.YearValue, out var year)
                ? query.Where(a => context.Releases.Any(r => r.AlbumId == a.Id && r.Year != null && r.Year.Value == year))
                : query.Where(a => context.Releases.Any(r => r.AlbumId == a.Id && r.Year != null && r.Year.Value.ToString().Contains(request.YearValue)));
        }
        var result = await query.ToPagedResultAsync(request.Page, request.PageSize, a => a.Id);

        return new AlbumIndexViewModel
        {
            CurrentPage = request.Page,
            PageCount = result.TotalPages,
            Albums = result.Items,
            PageSize = request.PageSize,
            HasAnyAlbumsInDb = await context.Albums.AsNoTracking().AnyAsync(),
            ArtistName = request.ArtistName,
            GenreName = request.GenreName,
            YearValue = request.YearValue,
            AlbumTitle = request.AlbumTitle
        };
    }
}

