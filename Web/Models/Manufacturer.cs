using Web.Enums;

namespace Web.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public EntityType Type { get; set; }
        public ICollection<Adc>? Adcs { get; set; }
        public ICollection<Amplifier>? Amplifiers { get; set; }
        public ICollection<Cartridge>? Cartridges { get; set; }
        public ICollection<Player>? Players { get; set; }
        public ICollection<Wire>? Wires { get; set; }
    }
}
