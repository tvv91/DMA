namespace Web.Models
{
    /// <summary>
    /// Information about ADC (analog to digital converter)
    /// </summary>
    public class Adc : Base
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
