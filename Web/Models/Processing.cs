namespace Web.Models
{
    /// <summary>
    /// Information about processing
    /// </summary>
    public class Processing : Base
    {
        /// <summary>
        /// Foreign key
        /// </summary>
        public int TechicalInfoId { get; set; }

        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
