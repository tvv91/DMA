namespace Web.Models
{
    /// <summary>
    /// Information about sound source
    /// </summary>
    public class Format : Base
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
