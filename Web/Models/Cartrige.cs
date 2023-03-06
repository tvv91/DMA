namespace Web.Models
{
    /// <summary>
    /// Information about playback device cartrige / headshell
    /// </summary>
    public class Cartrige : Base
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
