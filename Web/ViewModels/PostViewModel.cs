using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class PostViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool IsDraft { get; set; }
    }
}
