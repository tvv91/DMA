namespace Web.Models
{
    /// <summary>
    /// Information about audio sampling
    /// </summary>
    public class Sampling
    {
        public int Id { get; set; }
        public double Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
