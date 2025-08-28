namespace Web.Models
{
    public class PostCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Post Post { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
