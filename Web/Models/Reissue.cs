using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Reissue
    {
        public int Id { get; set; }
        public int YearValue { get; set; }
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}