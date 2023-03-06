namespace Web.Models
{
    /// <summary>
    /// Information about album artist
    /// </summary>
    public class Artist : Base
    {
        /// <summary>
        /// Navigation property
        /// </summary>
        public Album Album { get; set; }
    }
}
