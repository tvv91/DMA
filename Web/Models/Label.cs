namespace Web.Models
{
    /// <summary>
    /// Information about sound recording studio
    /// </summary>
    public class Label
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
