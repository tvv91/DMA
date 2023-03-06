namespace Web.Models
{
    /// <summary>
    /// Music album representation
    /// </summary>
    public class Album : Base
    {
        #region Main album data        
        /// <summary>
        /// Album artist
        /// </summary>
        public Artist Artist { get; set; }
        
        /// <summary>
        /// Album genre
        /// </summary>
        public Genre Genre { get; set; }

        /// <summary>
        /// Album year
        /// </summary>
        public Year Year { get; set; }

        /// <summary>
        /// Album reissue year
        /// </summary>
        public Year? Reissue { get; set; }

        /// <summary>
        /// Album country
        /// </summary>
        public Country? Country { get; set; }

        /// <summary>
        /// Sound recording studio
        /// </summary>
        public Label? Label { get; set; }
        #endregion

        /// <summary>
        /// Technical information about digitizing (playback device, quality, cartrige etc.)
        /// </summary>
        #region Technical info
        public TechnicalInfo? TechnicalInfo { get; set; }
        #endregion
    }
}
