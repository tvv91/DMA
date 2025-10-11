using Web.Interfaces;

namespace Web.Models
{
    public class Cartridge : IManufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public int? ManufacturerId { get; set; }
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}
