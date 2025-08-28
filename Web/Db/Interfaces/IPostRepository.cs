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
    }
}
