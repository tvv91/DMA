using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class SourceFormat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
