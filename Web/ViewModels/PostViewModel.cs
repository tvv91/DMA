using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.ViewModels
{
    public class PostViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
