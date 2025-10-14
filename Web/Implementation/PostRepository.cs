using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class PostRepository : IPostRepository
    {
        private readonly DMADbContext _context;
        
        public PostRepository(DMADbContext context) => _context = context;

        public async Task<Post> AddAsync(Post post)
        {
            post.CreatedDate = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affected = await _context.Posts
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync();

            return affected > 0;
        }

        public async Task<PagedResult<Post>> GetListAsync(int page, int pageSize)
        {
            var query = _context.Posts
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
            .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, p => p.Id);
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts.Include(p => p.PostCategories).ThenInclude(pc => pc.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            var existing = await _context.Posts
               .Include(p => p.PostCategories)
               .FirstOrDefaultAsync(p => p.Id == post.Id);

            if (existing == null)
                throw new KeyNotFoundException($"Post with Id {post.Id} not found.");

            existing.Title = post.Title;
            existing.Description = post.Description;
            existing.Content = post.Content;
            existing.IsDraft = post.IsDraft;
            existing.UpdatedDate = DateTime.UtcNow;

            if (post.PostCategories?.Any() == true)
            {
                existing.PostCategories.Clear();
                foreach (var pc in post.PostCategories)
                {
                    existing.PostCategories.Add(new PostCategory
                    {
                        CategoryId = pc.CategoryId,
                        PostId = existing.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<PagedResult<Post>> GetFilteredListAsync(
            int page,
            int pageSize,
            string? searchText,
            string? category,
            string? year,
            bool onlyDrafts)
        {
            var query = _context.Posts.Include(p => p.PostCategories).ThenInclude(pc => pc.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchText) ||
                    p.Description.Contains(searchText) ||
                    p.Content.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p =>
                    p.PostCategories.Any(pc => pc.Category.Title == category));
            }

            if (!string.IsNullOrWhiteSpace(year) && int.TryParse(year, out var yearValue))
            {
                query = query.Where(p =>
                    p.CreatedDate.HasValue && p.CreatedDate.Value.Year == yearValue);
            }

            if (onlyDrafts)
            {
                query = query.Where(p => p.IsDraft);
            }

            return await query.ToPagedResultAsync(page, pageSize, p => p.Id);
        }
    }
}
