
namespace Web.Models
{
    public class PlayerManufacturer
    {
        public int? Id { get; set; }
        public string Data { get; set; }
        public ICollection<Player> Players { get; set; }
    }
}
