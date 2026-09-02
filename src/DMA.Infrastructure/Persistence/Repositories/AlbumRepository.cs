using DMA.Domain.Albums;
using DMA.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DMA.Infrastructure.Persistence.Repositories;

public class AlbumRepository(DmaDbContext context) : IAlbumRepository
{
    private readonly DmaDbContext _context = context;

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _context.Albums.AsNoTracking().AnyAsync(cancellationToken);

    public IQueryable<Album> GetQueryable() => _context.Albums.AsQueryable();

    public Task<Album?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Album?> FindByTitleAndArtistAsync(string title, string artist, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(artist))
            return Task.FromResult<Album?>(null);

        var normalizedTitle = title.Trim();
        var normalizedArtist = artist.Trim();

        return _context.Albums
            .Include(a => a.Artist)
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.Artist != null &&
                a.Title == normalizedTitle &&
                a.Artist.Name == normalizedArtist, cancellationToken);
    }

    public Task<Album?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Albums.FindAsync([id], cancellationToken).AsTask();

    public void Add(Album album) => _context.Albums.Add(album);

    public void Remove(Album album) => _context.Albums.Remove(album);

    public async Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null, CancellationToken cancellationToken = default)
    {
        if (page < 1)
            page = 1;

        var query = _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(artistName))
            query = query.Where(a => a.Artist != null && a.Artist.Name.Contains(artistName));

        if (!string.IsNullOrWhiteSpace(genreName))
            query = query.Where(a => a.Genre != null && a.Genre.Name.Contains(genreName));

        if (!string.IsNullOrWhiteSpace(albumTitle))
            query = query.Where(a => a.Title.Contains(albumTitle));

        if (!string.IsNullOrWhiteSpace(yearValue))
        {
            if (int.TryParse(yearValue, out var yearInt))
            {
                query = query.Where(a => _context.Releases.Any(d => d.AlbumId == a.Id && d.Year != null && d.Year.Value == yearInt));
            }
            else
            {
                query = query.Where(a => _context.Releases.Any(d => d.AlbumId == a.Id && d.Year != null && d.Year.Value.ToString().Contains(yearValue)));
            }
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Album>(items, totalItems, page, pageSize);
    }
}
