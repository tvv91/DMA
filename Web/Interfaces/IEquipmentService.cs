using Web.Common;
using Web.Enums;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IEquipmentService
    {
        Task<IManufacturer?> GetByIdAsync(int id, EntityType type);
        Task<PagedResult<IManufacturer>> GetListAsync(int page, int pageSize, EntityType type);
        Task<IManufacturer?> GetManufacturerByNameAsync(string name, EntityType type);
        Task<IManufacturer> CreateEquipmentAsync(EquipmentViewModel request);
        Task<IManufacturer> UpdateEquipmentAsync(EquipmentViewModel request);
        Task<bool> DeleteEquipmentAsync(int id, EntityType type);
        EquipmentViewModel MapEquipmentToViewModel(IManufacturer equipment, EntityType type, string? imageUrl = null);
        Task<IManufacturer> MapViewModelToEquipmentAsync(EquipmentViewModel request);
    }
}


