using Web.Db.Interfaces;

namespace Web.Models
{
    public class AmplifierManufacturer : IManufacturer
    {
        public int? Id { get; set; }
        public string Data { get; set; }
        public ICollection<Amplifier> Amplifiers  { get; set; }
    }
}
