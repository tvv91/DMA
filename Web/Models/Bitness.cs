namespace Web.Models
{
    /// <summary>
    /// Information about audio bitness
    /// </summary>
    public class Bitness
    {
        public int Id { get; set; }
        public int Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
