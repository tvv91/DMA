using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db.Interfaces
{
    public interface IPostRepository
    {
        public IQueryable<Post> Posts { get; }
        public IQueryable<Category> Categories { get; }
        public IQueryable<PostCategory> PostCategories { get; }
        public Task<PostCategory> AddPostAsync(PostCategory postCategory);
        public Task<Category> GetOrCreateCategory(string title);
        public Task UpdatePostCategory(Post post, string categoryTitle);
        public Task UpdatePostAsync(Post post, string title, string description, string content, string categoryTitle);
    }
}
