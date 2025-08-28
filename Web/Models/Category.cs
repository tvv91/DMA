namespace Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<PostCategory> PostCategories { get; } = [];
    }
}
