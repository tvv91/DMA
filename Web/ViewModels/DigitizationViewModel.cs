namespace Web.ViewModels
{
    public class DigitizationViewModel
    {
        public int DigitizationId { get; set; }
        public string? VinylState { get; set; }
        public string? DigitalFormat { get; set; }
        public int? Bitness { get; set; }
        public double? Sampling { get; set; }
        public string? SourceFormat { get; set; }

        public string? Player { get; set; }
        public string? PlayerManufacturer { get; set; }
        public string? Cartridge { get; set; }
        public string? CartridgeManufacturer { get; set; }
        public string? Amplifier { get; set; }
        public string? AmplifierManufacturer { get; set; }
        public string? Adc { get; set; }
        public string? AdcManufacturer { get; set; }
        public string? Wire { get; set; }
        public string? WireManufacturer { get; set; }
    }
}