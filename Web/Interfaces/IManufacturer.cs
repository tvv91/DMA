using Web.Models;

namespace Web.Interfaces
{
    // Otherwice we should create 5 separete repos for entities that contains Manufacturer
    public interface IManufacturer
    {
        int Id { get; set; }
        Manufacturer? Manufacturer { get; set; }
        int? ManufacturerId { get; set; }
    }
}
