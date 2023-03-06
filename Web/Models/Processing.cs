namespace Web.Models
{
    /// <summary>
    /// Information about processing (declicking, etc.)
    /// </summary>
    public class Processing
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int TechicalInfoId { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
