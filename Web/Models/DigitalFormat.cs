using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class DigitalFormat
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}
