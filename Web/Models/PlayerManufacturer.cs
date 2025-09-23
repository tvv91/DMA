
using Web.Db.Interfaces;

namespace Web.Models
{
    public class PlayerManufacturer : IManufacturer
    {
        public int? Id { get; set; }
        public string Data { get; set; }
        public ICollection<Player> Players { get; set; }
    }
}
