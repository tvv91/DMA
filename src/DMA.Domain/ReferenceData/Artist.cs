namespace DMA.Domain.ReferenceData;

public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Albums.Album> Albums { get; set; } = [];
}
