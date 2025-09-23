using Web.Db.Interfaces;

namespace Web.Models
{
    /// <summary>
    /// Information about playback device
    /// </summary>
    public class Player : IEquipmentEntity<PlayerManufacturer>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string? Description { get; set; }
        public PlayerManufacturer? Manufacturer { get; set; }
        public int? ManufacturerId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
