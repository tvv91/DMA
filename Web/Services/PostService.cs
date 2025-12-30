using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly DMADbContext _context;

        public PostService(IPostRepository postRepository, DMADbContext context)
        {
            _postRepository = postRepository;
            _context = context;
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _postRepository.GetByIdAsync(id);
        }

        public async Task<PostViewModel> GetPostViewModelAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
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

            return await _postRepository.AddAsync(post);
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

            return await _postRepository.AddAsync(post);
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
            return await _postRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            return await _postRepository.DeleteAsync(id);
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
                Category = post.PostCategories.FirstOrDefault()?.Category?.Title
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

