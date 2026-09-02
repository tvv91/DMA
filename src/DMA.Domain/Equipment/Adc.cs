namespace DMA.Domain.Equipment;

public class Adc : IManufacturerEquipment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public int? ManufacturerId { get; set; }
}
