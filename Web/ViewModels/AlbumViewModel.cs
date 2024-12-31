using Web.Models;

namespace Web.ViewModels
{
    public class AlbumViewModel
    {
        public IEnumerable<Album> Albums { get; set; }
        public int AlbumCount { get; set; }
        public int CurrentPage { get; set; }
    }
}
