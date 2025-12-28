namespace Web.Request
{
    public class CreateUpdateDigitizationRequest
    {
        public int AlbumId { get; set; }
        public int DigitizationId { get; set; }
        public string Artist { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string? Source { get; set; }
        public string? Discogs { get; set; }
        public bool IsFirstPress { get; set; }
        public string? Country { get; set; }
        public string? Label { get; set; }
        public string? Storage { get; set; }
        public int? Year { get; set; }
        public int? Reissue { get; set; }
        public double? Size { get; set; }
        public string? VinylState { get; set; }
        public string? DigitalFormat { get; set; }
        public int? Bitness { get; set; }
        public double? Sampling { get; set; }
        public string? SourceFormat { get; set; }
        public string? Player { get; set; }
        public string? Cartridge { get; set; }
        public string? Amplifier { get; set; }
        public string? Adc { get; set; }
        public string? Wire { get; set; }
    }
}
