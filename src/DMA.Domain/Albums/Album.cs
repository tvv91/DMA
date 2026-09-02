namespace DMA.Domain.Albums;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? AddedDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int GenreId { get; set; }
    public ReferenceData.Genre Genre { get; set; } = null!;
    public int ArtistId { get; set; }
    public ReferenceData.Artist Artist { get; set; } = null!;
    public ICollection<Release> Releases { get; set; } = [];
}
