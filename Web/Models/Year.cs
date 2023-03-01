namespace Web.Models
{
    /// <summary>
    /// Information about album year
    /// </summary>
    public class Year
    {
        public int Id { get; set; }
        public int Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
