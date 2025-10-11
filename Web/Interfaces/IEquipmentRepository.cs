using Web.Common;
using Web.Enums;

namespace Web.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<IManufacturer?> GetByIdAsync(int id, EquipmentType type);
        Task<PagedResult<IManufacturer>> GetListAsync(int page, int pageSize, EquipmentType type);
        Task<IManufacturer> AddAsync(IManufacturer entity, EquipmentType type);
        Task<IManufacturer> UpdateAsync(IManufacturer entity, EquipmentType type);
        Task<bool> DeleteAsync(int id, EquipmentType type);
    }
}