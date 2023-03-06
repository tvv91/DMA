namespace Web.Models
{
    /// <summary>
    /// Information about album year
    /// </summary>
    public class Year : Base
    {
        /// <summary>
        /// Navigation property
        /// </summary>
        public Album Album { get; set; }
    }
}
