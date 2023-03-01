namespace Web.Models
{
    /// <summary>
    /// Information about codec / digitized audio format
    /// </summary>
    public class DigitalFormat
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
