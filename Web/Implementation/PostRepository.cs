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
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
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
            return await _context.Posts
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            post.UpdatedDate = DateTime.Now;

            await _context.Posts
                .Where(p => p.Id == post.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Title, post.Title)
                    .SetProperty(p => p.Description, post.Description)
                    .SetProperty(p => p.Content, post.Content)
                    .SetProperty(p => p.IsDraft, post.IsDraft)
                    .SetProperty(p => p.CreatedDate, post.CreatedDate)
                    .SetProperty(p => p.UpdatedDate, post.UpdatedDate)
                );

            return post;
        }


        /*
        public async Task<Category> GetOrCreateCategory(string title)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Title == title);
            if (category == null)
            {
                category = new Category { Title = title };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            return category;
        }

        public async Task<PostCategory> AddPostAsync(PostCategory postCategory)
        {
            var result = await _context.PostCategories.AddAsync(postCategory);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task UpdatePostCategory(Post post, string categoryTitle)
        {
            var postCategory = await _context.PostCategories.Include(x => x.Category).FirstOrDefaultAsync(x => x.Post.Id == post.Id);
            if (postCategory?.Category.Title != categoryTitle)
            {
                if (postCategory != null)
                    _context.PostCategories.Remove(postCategory);

                var category = await GetOrCreateCategory(categoryTitle);
                _context.PostCategories.Add(new PostCategory {  Post = post, Category = category });
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePostAsync(Post post, string title, string description, string content, string categoryTitle)
        {
            post.Title = title;
            post.Description = description;
            post.Content = content;
            post.UpdatedDate = DateTime.UtcNow;

            var postCategory = await _context.PostCategories.Include(x => x.Category).FirstOrDefaultAsync(x => x.Post.Id == post.Id);

            if (postCategory?.Category.Title != categoryTitle)
            {
                if (postCategory != null)
                    _context.PostCategories.Remove(postCategory);

                var category = await GetOrCreateCategory(categoryTitle);
                _context.PostCategories.Add(new PostCategory { Post = post, Category = category });
            }

            await _context.SaveChangesAsync();
        }
        */
    }
}
