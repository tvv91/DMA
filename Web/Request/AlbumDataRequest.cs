using System.ComponentModel.DataAnnotations;
using Web.Enums;

namespace Web.Request
{
    public class AlbumDataRequest
    {
        #region Main info
        [Required]
        public string Album { get; set; }
        [Required]
        public string Artist { get; set; }
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public int? Reissue { get; set; }
        public string? Country { get; set; }
        public string? Label { get; set; }
        public string? Source { get; set; }
        public double? Size { get; set; }
        public string? Storage { get; set; }
        public string? AlbumCover { get; set; }
        public string? Discogs { get; set; }

        #endregion

        #region Technical Info
        public string? Adc { get; set; }
        public string? AdcManufacturer { get; set; }
        public string? Amplifier { get; set; }
        public string? AmplifierManufacturer { get; set; }
        public int? Bitness { get; set; }
        public string? Cartridge { get; set; }
        public string? CartridgeManufacturer { get; set; }
        public string? DigitalFormat { get; set; }
        public string? Player { get; set; }
        public string? PlayerManufacturer { get; set; }
        public string? SourceFormat { get; set; }
        public double? Sampling { get; set; }
        public string? VinylState { get; set; }
        public string? Wire { get; set; }
        public string? WireManufacturer { get; set; }
        #endregion

        public ActionType Action { get; set; }
        public int AlbumId { get; set; }
    }
}
