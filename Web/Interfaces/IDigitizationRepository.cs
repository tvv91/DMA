using Web.Common;
using Web.Enums;
using Web.Models;

namespace Web.Interfaces
{
    public interface IDigitizationRepository : IRepository<Digitization>
    {
        Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId);
        Task<IEnumerable<Digitization>> GetDigitizationsByEquipmentAsync(EntityType equipmentType, int equipmentId);
        Task<PagedResult<Album>> GetAlbumsDigitizedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize);
        Task<bool> ExistsByAlbumIdAsync(int albumId);
        Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source);
    }
}
