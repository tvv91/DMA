namespace DMA.Domain.Albums;

public class Release
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;
    public DateTime? AddedDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? Source { get; set; }
    public string? Discogs { get; set; }
    public bool? IsFirstPress { get; set; }
    public int? CountryId { get; set; }
    public ReferenceData.Country? Country { get; set; }
    public int? LabelId { get; set; }
    public ReferenceData.Label? Label { get; set; }
    public int? ReissueId { get; set; }
    public ReferenceData.Reissue? Reissue { get; set; }
    public int? YearId { get; set; }
    public ReferenceData.Year? Year { get; set; }
    public int? StorageId { get; set; }
    public ReferenceData.Storage? Storage { get; set; }
    public int? FormatInfoId { get; set; }
    public FormatInfo? FormatInfo { get; set; }
    public int? EquipmentInfoId { get; set; }
    public EquipmentInfo? EquipmentInfo { get; set; }
    public double? Size { get; set; }
}
