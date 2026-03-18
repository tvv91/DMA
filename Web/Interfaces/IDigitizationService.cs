using Web.Common;
using Web.Enums;
using Web.Models;

namespace Web.Interfaces
{
    public interface IDigitizationService
    {
        Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId);
        Task<Digitization?> GetByIdAsync(int id);
        Task<PagedResult<Album>> GetAlbumsDigitizedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize);
        Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source);
        Task<Digitization> AddAsync(Digitization digitization);
        Task<Digitization> UpdateAsync(Digitization digitization);
        Task<bool> DeleteAsync(int id);
    }
}

