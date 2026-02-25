using Web.Common;
using Web.Models;

namespace Web.Interfaces
{
    public interface IAlbumRepository : IRepository<Album>
    {
        Task<PagedResult<Album>> SearchByTitleAsync(string title, int page, int pageSize);
        Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null);
        Task<Album?> FindByAlbumAndArtistAsync(string title, string artist);
    }
}
