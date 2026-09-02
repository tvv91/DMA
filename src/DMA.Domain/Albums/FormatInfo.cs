namespace DMA.Domain.Albums;

public class FormatInfo
{
    public int Id { get; set; }
    public int? BitnessId { get; set; }
    public ReferenceData.Bitness? Bitness { get; set; }
    public int? SamplingId { get; set; }
    public ReferenceData.Sampling? Sampling { get; set; }
    public int? DigitalFormatId { get; set; }
    public ReferenceData.DigitalFormat? DigitalFormat { get; set; }
    public int? SourceFormatId { get; set; }
    public ReferenceData.SourceFormat? SourceFormat { get; set; }
    public int? VinylStateId { get; set; }
    public ReferenceData.VinylState? VinylState { get; set; }
}
