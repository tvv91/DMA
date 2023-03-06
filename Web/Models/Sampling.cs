namespace Web.Models
{
    /// <summary>
    /// Information about audio sampling
    /// </summary>
    public class Sampling : Base
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
