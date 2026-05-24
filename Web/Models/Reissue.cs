using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Reissue
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public ICollection<Release> Releases { get; set; } = [];
    }
}