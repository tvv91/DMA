using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.Db;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Integration
{
    public class PostServiceIntegrationTests : IDisposable
    {
        private readonly DMADbContext _context;
        private readonly PostService _service;

        public PostServiceIntegrationTests()
        {
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            var repository = new Web.Implementation.PostRepository(_context);
            _service = new PostService(repository, _context);
        }

        [Fact]
        public async Task CreatePostAsync_CreatesPost_WithCategory()
        {
            // Arrange
            var category = Helpers.TestDataBuilder.CreateCategory(1, "Developing");
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "New Post",
                Description = "Description",
                Content = "Content",
                Category = "Developing"
            };

            // Act
            var result = await _service.CreatePostAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.False(result.IsDraft);
            Assert.Equal(model.Title, result.Title);
            
            var savedPost = await _context.Posts
                .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == result.Id);
            
            Assert.NotNull(savedPost);
            Assert.Single(savedPost.PostCategories);
            Assert.Equal("Developing", savedPost.PostCategories.First().Category.Title);
        }

        [Fact]
        public async Task CreateDraftPostAsync_CreatesDraft_WithCategory()
        {
            // Arrange
            var category = Helpers.TestDataBuilder.CreateCategory(1, "Releases");
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "Draft Post",
                Description = "Description",
                Content = "Content",
                Category = "Releases"
            };

            // Act
            var result = await _service.CreateDraftPostAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsDraft);
            Assert.Equal(model.Title, result.Title);
        }

        [Fact]
        public async Task UpdatePostAsync_UpdatesPost_AndCategory()
        {
            // Arrange
            var existingCategory = Helpers.TestDataBuilder.CreateCategory(1, "Old Category");
            var newCategory = Helpers.TestDataBuilder.CreateCategory(2, "New Category");
            _context.Categories.AddRange(existingCategory, newCategory);

            var post = Helpers.TestDataBuilder.CreatePost(1, "Old Title");
            post.PostCategories.Add(new PostCategory
            {
                PostId = 1,
                CategoryId = 1,
                Category = existingCategory
            });
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Content = "Updated Content",
                Category = "New Category"
            };

            // Act
            var result = await _service.UpdatePostAsync(1, model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.NotNull(result.UpdatedDate);

            var updatedPost = await _context.Posts
                .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == 1);

            Assert.NotNull(updatedPost);
            Assert.Single(updatedPost.PostCategories);
            Assert.Equal("New Category", updatedPost.PostCategories.First().Category.Title);
        }

        [Fact]
        public async Task UpdatePostAsync_CreatesNewCategory_WhenNotExists()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost(1, "Post");
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "Updated Title",
                Description = "Description",
                Content = "Content",
                Category = "New Category Name"
            };

            // Act
            var result = await _service.UpdatePostAsync(1, model);

            // Assert
            Assert.NotNull(result);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Title == "New Category Name");
            Assert.NotNull(category);
        }

        [Fact]
        public async Task DeletePostAsync_DeletesPost_AndReturnsTrue()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost(1, "To Delete");
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeletePostAsync(1);

            // Assert
            Assert.True(result);
            var deletedPost = await _context.Posts.FindAsync(1);
            Assert.Null(deletedPost);
        }

        [Fact]
        public async Task GetPostViewModelAsync_ReturnsCorrectViewModel()
        {
            // Arrange
            var category = Helpers.TestDataBuilder.CreateCategory(1, "Test Category");
            _context.Categories.Add(category);
            
            var post = Helpers.TestDataBuilder.CreatePost(1, "Test Post", false);
            post.PostCategories.Add(new PostCategory
            {
                PostId = 1,
                CategoryId = 1,
                Category = category
            });
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPostViewModelAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Post", result.Title);
            Assert.Equal("Test Category", result.Category);
            Assert.False(result.IsDraft);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }
    }
}


