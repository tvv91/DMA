using DMA.Domain.Common;

namespace DMA.Domain.Equipment;

public interface IEquipmentRepository
{
    Task<IManufacturerEquipment?> GetByIdAsync(int id, EntityType type, CancellationToken cancellationToken = default);
    IQueryable<IManufacturerEquipment> GetQueryable(EntityType type);
    Task<IManufacturerEquipment?> GetByNameAsync(string name, EntityType type, CancellationToken cancellationToken = default);
    void Add(IManufacturerEquipment equipment);
    void Update(IManufacturerEquipment equipment);
    void Remove(IManufacturerEquipment equipment);
    Task<Manufacturer?> FindManufacturerByNameAsync(string name, CancellationToken cancellationToken = default);
    void AddManufacturer(Manufacturer manufacturer);
}
