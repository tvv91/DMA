namespace DMA.Domain.Equipment;

public interface IManufacturerEquipment
{
    int Id { get; set; }
    string Name { get; set; }
    string? Description { get; set; }
    Manufacturer? Manufacturer { get; set; }
    int? ManufacturerId { get; set; }
}
