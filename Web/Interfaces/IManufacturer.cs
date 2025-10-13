using Web.Models;

namespace Web.Interfaces
{
    // Otherwice we should create 5 separete repos for entities that contains Manufacturer
    public interface IManufacturer
    {
        int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        Manufacturer? Manufacturer { get; set; }
        int? ManufacturerId { get; set; }
    }
}
