using Web.Db.Interfaces;

namespace Web.Models
{
    /// <summary>
    /// Information about interconnect cables
    /// </summary>
    public class Wire : IEquipmentEntity<WireManufacturer>
    {
        public int Id { get; set; }
        public string Data {  get; set; }
        public string? Description { get; set; }
        public WireManufacturer? Manufacturer { get; set; }
        public int? ManufacturerId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
