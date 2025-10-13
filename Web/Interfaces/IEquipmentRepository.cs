using Web.Common;
using Web.Enums;

namespace Web.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<IManufacturer?> GetByIdAsync(int id, EntityType type);
        Task<PagedResult<IManufacturer>> GetListAsync(int page, int pageSize, EntityType type);
        Task<IManufacturer> AddAsync(IManufacturer entity, EntityType type);
        Task<IManufacturer> UpdateAsync(IManufacturer entity, EntityType type);
        Task<bool> DeleteAsync(int id, EntityType type);
    }
}