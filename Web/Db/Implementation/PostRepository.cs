using Web.Db.Interfaces;
using Web.Models;

namespace Web.Db.Implementation
{
    public class PostRepository : IPostRepository
    {
        private readonly DMADbContext _context;
        public PostRepository(DMADbContext context) => _context = context;

        public IQueryable<Post> Posts => _context.Posts;
        public IQueryable<Category> Categories => _context.Category;
        public IQueryable<PostCategory> PostCategories => _context.PostCategories;

        public async Task<PostCategory> AddPostAsync(PostCategory postCategory)
        {
            var result = await _context.PostCategories.AddAsync(postCategory);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}
