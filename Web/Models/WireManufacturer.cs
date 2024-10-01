namespace Web.Models
{
    public class WireManufacturer
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Wire> Wires { get; set; }
    }
}
