namespace Web.Models
{
    /// <summary>
    /// Information about album reissue year
    /// </summary>
    public class Reissue : Base
    {
        /// <summary>
        /// Foreign key
        /// </summary>
        public int AlbumId { get; set; }

        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<Album> Albums { get; set; }
    }
}
