namespace Web.Models
{
    /// <summary>
    /// Information about storage where album placed / saved
    /// </summary>
    public class Storage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Release> Releases { get; set; } = [];
    }
}
