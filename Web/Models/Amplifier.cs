using Web.Db.Interfaces;

namespace Web.Models
{
    /// <summary>
    /// Information about amplifier
    /// </summary>
    public class Amplifier : IEquipmentEntity<AmplifierManufacturer>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string? Description { get; set; }
        public AmplifierManufacturer? Manufacturer { get; set; }
        public int? ManufacturerId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
