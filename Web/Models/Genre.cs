namespace Web.Models
{
    /// <summary>
    /// Information about album genre
    /// </summary>
    public class Genre : Base
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
