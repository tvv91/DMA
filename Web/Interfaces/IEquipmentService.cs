using Web.Enums;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IEquipmentService
    {
        Task<IManufacturer?> GetByIdAsync(int id, EntityType type);
        Task<IManufacturer> CreateEquipmentAsync(EquipmentViewModel request);
        Task<IManufacturer> UpdateEquipmentAsync(EquipmentViewModel request);
        Task<bool> DeleteEquipmentAsync(int id, EntityType type);
        EquipmentViewModel MapEquipmentToViewModel(IManufacturer equipment, EntityType type, string? imageUrl = null);
        IManufacturer MapViewModelToEquipment(EquipmentViewModel request);
    }
}

