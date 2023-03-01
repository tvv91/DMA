namespace Web.Models
{
    /// <summary>
    /// Information about country
    /// </summary>
    public class Country
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
