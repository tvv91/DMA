using Web.Models;

namespace Web.ViewModels
{
    public class AlbumIndexViewModel
    {
        public IEnumerable<Album>? Albums { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}
