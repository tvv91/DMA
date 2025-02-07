namespace Web.Models
{
    /// <summary>
    /// Information about playback device cartrige / headshell
    /// </summary>
    public class Cartrige
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string? Description { get; set; }
        public CartrigeManufacturer? Manufacturer { get; set; }
        public int? ManufacturerId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
