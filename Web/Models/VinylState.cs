using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class VinylState
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;    
    }
}
