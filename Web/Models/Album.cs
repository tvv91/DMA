namespace Web.Models
{
    /// <summary>
    /// Music album representation
    /// </summary>
    public class Album
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int Size { get; set; }
        public DateTime AddedDate { get; set; }

        #region Foreign keys
        public int ArtistId { get; set; }
        public int GenreId { get; set; }
        public int YearId { get; set; }
        public int? ReissueId { get; set; }
        public int? CountryId { get; set; }

        #endregion

        #region Navigation properties
        public Artist Artist { get; set; }
        public Genre Genre { get; set; }
        public Year Year { get; set; }
        public Reissue? Reissue { get; set; }
        public Country? Country { get; set; }
        public Label? Label { get; set; }
        public TechnicalInfo? TechnicalInfo { get; set; }
        #endregion
    }
}
