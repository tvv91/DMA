namespace DMA.Domain.ReferenceData;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Albums.Release> Releases { get; set; } = [];
}
