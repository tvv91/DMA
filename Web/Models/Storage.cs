namespace Web.Models
{
    /// <summary>
    /// Information about storage where album placed / saved
    /// </summary>
    public class Storage
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
