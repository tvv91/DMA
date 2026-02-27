using Web.Common;
using Web.Enums;
using Web.Models;

namespace Web.Interfaces
{
    public interface IAlbumRepository : IRepository<Album>
    {
        Task<PagedResult<Album>> SearchByTitleAsync(string title, int page, int pageSize);
        Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null);
        Task<PagedResult<Album>> GetAlbumsByEquipmentAsync(EntityType equipmentType, int equipmentId, int page, int pageSize);
        Task<Album?> FindByAlbumAndArtistAsync(string title, string artist);
    }
}
