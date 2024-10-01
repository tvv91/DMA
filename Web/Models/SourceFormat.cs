namespace Web.Models
{
    /// <summary>
    /// Information about sound source
    /// </summary>
    public class SourceFormat
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
