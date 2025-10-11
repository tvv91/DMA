using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Sampling
    {
        public int Id { get; set; }
        [Required]
        public double Value { get; set; }
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}
