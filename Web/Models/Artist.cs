namespace Web.Models
{
    /// <summary>
    /// Information about album artist
    /// </summary>
    public class Artist
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
