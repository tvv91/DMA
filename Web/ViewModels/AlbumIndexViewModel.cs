using Web.Models;

namespace Web.ViewModels
{
    public class AlbumIndexViewModel
    {
        public IEnumerable<Album>? Albums { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasAnyAlbumsInDb { get; set; }
        
        // Current filter values (text inputs)
        public string? ArtistName { get; set; }
        public string? GenreName { get; set; }
        public string? YearValue { get; set; }
        public string? AlbumTitle { get; set; }
    }
}
