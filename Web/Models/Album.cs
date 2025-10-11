using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Album
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;        
        public DateTime? AddedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}
