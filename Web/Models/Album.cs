namespace Web.Models
{
    /// <summary>
    /// Music album representation
    /// </summary>
    public class Album
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Album size, Gb
        /// </summary>
        public double? Size { get; set; }

        /// <summary>
        /// Source from where album was downloaded / buyed
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Link to discogs
        /// </summary>
        public string? Discogs { get; set; }

        /// <summary>
        /// Date when album was added to db
        /// </summary>
        public DateTime AddedDate { get; set; }

        #region Foreign keys

        public int ArtistId { get; set; }
        public int? GenreId { get; set; }
        public int? YearId { get; set; }
        public int? ReissueId { get; set; }
        public int? CountryId { get; set; }
        public int? LabelId { get; set; }
        public int? StorageId { get; set; }

        #endregion

        #region Navigation properties
        public Artist Artist { get; set; }
        public Genre? Genre { get; set; }
        public Year? Year { get; set; }
        public Reissue? Reissue { get; set; }
        public Country? Country { get; set; }
        public Label? Label { get; set; }
        public Storage? Storage {  get; set; }
        public TechnicalInfo? TechnicalInfo { get; set; }
        #endregion
    }
}
