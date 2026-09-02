using DMA.Domain.Common;

namespace DMA.Domain.Albums;

public interface IAlbumRepository
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null, CancellationToken cancellationToken = default);
    IQueryable<Album> GetQueryable();
    Task<Album?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Album?> FindByTitleAndArtistAsync(string title, string artist, CancellationToken cancellationToken = default);
    Task<Album?> FindAsync(int id, CancellationToken cancellationToken = default);
    void Add(Album album);
    void Remove(Album album);
}
