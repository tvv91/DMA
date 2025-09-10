using Microsoft.EntityFrameworkCore;
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

        public async Task<Category> GetOrCreateCategory(string title)
        {
            var category = await _context.Category.FirstOrDefaultAsync(x => x.Title == title);
            if (category == null)
            {
                category = new Category { Title = title };
                _context.Category.Add(category);
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
    }
}
