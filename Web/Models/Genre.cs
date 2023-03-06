namespace Web.Models
{
    /// <summary>
    /// Information about album genre
    /// </summary>
    public class Genre : Base
    {
        /// <summary>
        /// Navigation property
        /// </summary>
        public Album Album { get; set; }
    }
}
