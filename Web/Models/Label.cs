namespace Web.Models
{
    public class Label : Base
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
