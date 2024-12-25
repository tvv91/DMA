namespace Web.Models
{
    /// <summary>
    /// Information about interconnect cables
    /// </summary>
    public class Wire
    {
        public int Id { get; set; }
        public string Data {  get; set; }
        public string? Description { get; set; }
        public WireManufacturer? Manufacturer { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
