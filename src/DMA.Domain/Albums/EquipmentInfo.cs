namespace DMA.Domain.Albums;

public class EquipmentInfo
{
    public int Id { get; set; }
    public int? PlayerId { get; set; }
    public Equipment.Player? Player { get; set; }
    public int? CartridgeId { get; set; }
    public Equipment.Cartridge? Cartridge { get; set; }
    public int? AmplifierId { get; set; }
    public Equipment.Amplifier? Amplifier { get; set; }
    public int? AdcId { get; set; }
    public Equipment.Adc? Adc { get; set; }
    public int? WireId { get; set; }
    public Equipment.Wire? Wire { get; set; }
}
