namespace Web.Models
{
    /// <summary>
    /// Information about codec
    /// </summary>
    public class Codec
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int TechicalInfoId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
