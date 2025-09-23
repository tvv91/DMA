using Web.Db.Interfaces;

namespace Web.Models
{
    /// <summary>
    /// Information about playback device cartridge / headshell
    /// </summary>
    public class Cartridge : IEquipmentEntity<CartridgeManufacturer>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string? Description { get; set; }
        public CartridgeManufacturer? Manufacturer { get; set; }
        public int? ManufacturerId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
