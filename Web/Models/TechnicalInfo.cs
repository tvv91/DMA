namespace Web.Models
{
    /// <summary>
    /// Information about album digitizing
    /// </summary>
    public class TechnicalInfo
    {
        public int Id { get; set; }

        #region Foreign keys

        public int? AlbumId { get; set; }
        public int? AmplifierId { get; set; }
        public int? BitnessId { get; set; }
        public int? CartrigeId { get; set; }
        public int? CodecId { get; set; }
        public int? DeviceId { get; set; }
        public int? FormatId { get; set; }
        public int? ProcessingId { get; set; }
        public int? AdcId { get; set; }
        public int? SamplingId { get; set; }
        public int? StateId { get; set; }

        #endregion

        #region Navigation properties
        public Album Album { get; set; }
        public Adc? Adc { get; set; }
        public Amplifier? Amplifier { get; set; }
        public Bitness? Bitness { get; set; }
        public Cartrige? Cartrige { get; set; }
        public Codec? Codec { get; set; }
        public Device? Device { get; set; }
        public Format? Format { get; set; }
        public Processing? Processing { get; set; }
        public Sampling? Sampling { get; set; }
        public State? State { get; set; }
        #endregion
    }
}
