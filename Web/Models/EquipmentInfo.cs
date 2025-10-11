namespace Web.Models
{
    public class EquipmentInfo
    {
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }

        public int? CartridgeId { get; set; }
        public Cartridge? Cartridge { get; set; }

        public int? AmplifierId { get; set; }
        public Amplifier? Amplifier { get; set; }

        public int? AdcId { get; set; }
        public Adc? Adc { get; set; }

        public int? WireId { get; set; }
        public Wire? Wire { get; set; }
    }
}