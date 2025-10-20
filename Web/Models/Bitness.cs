using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Bitness
    {
        public int Id { get; set; }
        [Required]
        public int Value { get; set; }
    }
}
