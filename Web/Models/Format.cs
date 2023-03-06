namespace Web.Models
{
    /// <summary>
    /// Information about sound source
    /// </summary>
    public class Format
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int TechicalInfoId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
