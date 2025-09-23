using Web.Db.Interfaces;

namespace Web.Models
{
    public class WireManufacturer : IManufacturer
    {
        public int? Id { get; set; }
        public string Data { get; set; }
        public ICollection<Wire> Wires { get; set; }
    }
}
