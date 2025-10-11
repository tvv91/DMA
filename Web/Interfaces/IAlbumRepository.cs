using Web.Common;
using Web.Models;

namespace Web.Interfaces
{
    public interface IAlbumRepository : IRepository<Album>
    {
        Task<PagedResult<Album>> SearchByTitleAsync(string title, int page, int pageSize);
        Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize);
        Task<Album?> FindByTitleAndArtistAsync(string title, string artist);
    }
}
