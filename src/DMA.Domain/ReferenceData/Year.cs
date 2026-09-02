namespace DMA.Domain.ReferenceData;

public class Year
{
    public int Id { get; set; }
    public int Value { get; set; }
    public ICollection<Albums.Release> Releases { get; set; } = [];
}
