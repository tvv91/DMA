using System.Collections;
using Web.Models;

namespace Web.ViewModels
{
    public class AlbumDetailsViewModel
    {
        public int AlbumId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string? AlbumCoverUrl { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public IEnumerable<Digitization>? Digitizations { get; set; }
        
    }
}
