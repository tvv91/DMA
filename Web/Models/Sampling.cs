namespace Web.Models
{
    /// <summary>
    /// Information about audio sampling
    /// </summary>
    public class Sampling
    {
        public int Id { get; set; }
        public int Data { get; set; }
        public int TechicalInfoId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
