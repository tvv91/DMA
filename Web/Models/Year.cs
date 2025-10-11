using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Year
    {
        public int Id { get; set; }
        [Required]
        public int YearValue { get; set; }
        public ICollection<Album> Albums { get; set; } = [];
    }
}
