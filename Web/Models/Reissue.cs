namespace Web.Models
{
    /// <summary>
    /// Information about album reissue year
    /// </summary>
    public class Reissue
    {
        public int Id { get; set; }
        public int? Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
