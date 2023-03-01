namespace Web.Models
{
    /// <summary>
    /// Information about vinyl state
    /// </summary>
    public class VinylState
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
