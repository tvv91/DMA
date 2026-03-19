namespace Web.Models
{
    public class Label
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}
