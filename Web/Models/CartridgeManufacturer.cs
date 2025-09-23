using Web.Db.Interfaces;

namespace Web.Models
{
    public class CartridgeManufacturer : IManufacturer
    {
        public int? Id { get; set; }
        public string Data { get; set; }
        public ICollection<Cartridge> Cartridges { get; set; }
    }
}
