namespace Web.Models
{
    public class FormatInfo
    {
        public int Id { get; set; }
        public double? Size { get; set; }
        public int? BitnessId { get; set; }
        public Bitness? Bitness { get; set; }
        public int? SamplingId { get; set; }
        public Sampling? Sampling { get; set; }
        public int? DigitalFormatId { get; set; }
        public DigitalFormat? DigitalFormat { get; set; }
        public int? SourceFormatId { get; set; }
        public SourceFormat? SourceFormat { get; set; }
        public int? VinylStateId { get; set; }
        public VinylState? VinylState { get; set; }
    }
}