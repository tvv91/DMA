namespace Web.Models
{
    /// <summary>
    /// Information about album digitizing
    /// </summary>
    public class TechnicalInfo
    {
        public int Id { get; set; }

        #region Foreign keys

        public int AlbumId { get; set; }
        public int? AmplifierId { get; set; }
        public int? BitnessId { get; set; }
        public int? CartridgeId { get; set; }
        public int? DigitalFormatId { get; set; }
        public int? PlayerId { get; set; }
        public int? SourceFormatId { get; set; }
        public int? AdcId { get; set; }
        public int? SamplingId { get; set; }
        public int? VinylStateId { get; set; }
        public int? WireId { get; set; }

        #endregion

        #region Navigation properties
        public Album Album { get; set; }
        public Adc? Adc { get; set; }
        public Amplifier? Amplifier { get; set; }
        public Bitness? Bitness { get; set; }
        public Cartridge? Cartridge { get; set; }
        public DigitalFormat? DigitalFormat { get; set; }
        public Player? Player { get; set; }
        public SourceFormat? SourceFormat { get; set; }
        public Sampling? Sampling { get; set; }
        public VinylState? VinylState { get; set; }
        public Wire? Wire { get; set; }
        #endregion
    }
}
