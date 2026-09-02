namespace DMA.Domain.ReferenceData;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Albums.Album> Albums { get; set; } = [];
}
