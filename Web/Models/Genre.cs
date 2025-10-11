using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Album> Albums { get; set; } = [];
    }
}
