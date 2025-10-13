namespace Web.Models
{
    public class Digitization
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Source { get; set; }
        public string? Discogs { get; set; }
        public bool IsFirstPress { get; set; }
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
        public int? LabelId { get; set; }
        public Label? Label { get; set; }
        public int? ReissueId { get; set; }
        public Reissue? Reissue { get; set; }
        public int? YearId { get; set; }
        public Year? Year { get; set; }
        public int? StorageId { get; set; }
        public Storage? Storage { get; set; }
        public FormatInfo? Format { get; set; }
        public EquipmentInfo? Equipment { get; set; }
        public double? Size { get; set; }
    }
}