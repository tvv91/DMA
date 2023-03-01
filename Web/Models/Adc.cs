namespace Web.Models
{
    /// <summary>
    /// Information about ADC (analog to digital converter)
    /// </summary>
    public class Adc
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string? Description { get; set; }
        public AdcManufacturer AdcManufacturer { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
