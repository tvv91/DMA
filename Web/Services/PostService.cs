using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class PostService(DMADbContext context) : IPostService
    {
        private readonly DMADbContext _context = context;

        public async Task<PagedResult<Post>> GetListAsync(int page, int pageSize)
        {
            var query = _context.Posts
                .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
                .AsNoTracking()
                .AsQueryable();

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue)
                .ThenByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Post>(items, totalItems, page, pageSize);
        }

        public async Task<PagedResult<Post>> GetFilteredListAsync(
            int page,
            int pageSize,
            string? searchText,
            string? category,
            string? year,
            bool onlyDrafts)
        {
            var query = _context.Posts
                .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchText) ||
                    p.Description.Contains(searchText) ||
                    p.Content.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.PostCategories.Any(pc => pc.Category.Title == category));
            }

            if (!string.IsNullOrWhiteSpace(year) && int.TryParse(year, out var yearValue))
            {
                query = query.Where(p => p.CreatedDate.HasValue && p.CreatedDate.Value.Year == yearValue);
            }

            if (onlyDrafts)
            {
                query = query.Where(p => p.IsDraft);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue)
                .ThenByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Post>(items, totalItems, page, pageSize);
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PostViewModel> GetPostViewModelAsync(int id)
        {
            var post = await GetByIdAsync(id);
            if (post == null)
                throw new KeyNotFoundException($"Post with id {id} not found");

            return MapPostToViewModel(post);
        }

        public async Task<Post> CreatePostAsync(PostViewModel model)
        {
            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                CreatedDate = DateTime.UtcNow,
                IsDraft = false
            };

            if (!string.IsNullOrWhiteSpace(model.Category))
            {
                var category = await FindOrCreateCategoryAsync(model.Category);
                post.PostCategories.Add(new PostCategory
                {
                    Category = category
                });
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> CreateDraftPostAsync(PostViewModel model)
        {
            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                CreatedDate = DateTime.UtcNow,
                IsDraft = true
            };

            if (!string.IsNullOrWhiteSpace(model.Category) && model.Category != "Category")
            {
                var category = await FindOrCreateCategoryAsync(model.Category);
                post.PostCategories.Add(new PostCategory
                {
                    Category = category
                });
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdatePostAsync(int postId, PostViewModel model)
        {
            // Business logic: Load existing post with tracking for updates
            var existing = await _context.Posts
                .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == postId);
            
            if (existing == null)
                throw new KeyNotFoundException($"Post with Id {postId} not found.");

            // Business logic: Update properties
            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Content = model.Content;
            existing.UpdatedDate = DateTime.UtcNow;

            // Business logic: Update category if changed
            var currentCategory = existing.PostCategories.FirstOrDefault()?.Category?.Title;
            var newCategory = model.Category?.Trim();

            // Only update category if it's different and not empty/placeholder
            if (newCategory != currentCategory && !string.IsNullOrWhiteSpace(newCategory) && newCategory != "Category")
            {
                existing.PostCategories.Clear();
                var category = await FindOrCreateCategoryAsync(newCategory);
                existing.PostCategories.Add(new PostCategory
                {
                    Category = category
                });
            }

            // Repository only saves changes
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public PostViewModel MapPostToViewModel(Post post)
        {
            return new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                CreatedDate = post.CreatedDate,
                UpdatedTime = post.UpdatedDate,
                Category = post.PostCategories.FirstOrDefault()?.Category?.Title,
                IsDraft = post.IsDraft
            };
        }

        private async Task<Category> FindOrCreateCategoryAsync(string categoryTitle)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Title.ToLower() == categoryTitle.ToLower());

            if (category == null)
            {
                category = new Category { Title = categoryTitle };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            return category;
        }
    }
}

