using Web.Models;

namespace Web.Interfaces
{
    public interface IEquipment
    {
        int Id { get; set; }
        string Name { get; set; }
        string? Description { get; set; }
        Manufacturer? Manufacturer { get; set; }
        int? ManufacturerId { get; set; }
    }
}
