using Microsoft.EntityFrameworkCore;
using Moq;
using Web.Db;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Unit
{
    public class PostServiceTests : IDisposable
    {
        private readonly Mock<IPostRepository> _mockRepository;
        private readonly DMADbContext _context;
        private readonly PostService _service;

        public PostServiceTests()
        {
            _mockRepository = new Mock<IPostRepository>();
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _service = new PostService(_mockRepository.Object, _context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPost_WhenExists()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(post.Id, result.Id);
            Assert.Equal(post.Title, result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPostViewModelAsync_ReturnsViewModel_WhenPostExists()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            post.PostCategories.Add(new PostCategory
            {
                Category = Helpers.TestDataBuilder.CreateCategory()
            });
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

            // Act
            var result = await _service.GetPostViewModelAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(post.Id, result.Id);
            Assert.Equal(post.Title, result.Title);
            Assert.Equal(post.Description, result.Description);
            Assert.Equal(post.Content, result.Content);
            Assert.Equal(post.IsDraft, result.IsDraft);
        }

        [Fact]
        public async Task GetPostViewModelAsync_ThrowsKeyNotFoundException_WhenPostNotExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetPostViewModelAsync(1));
        }

        [Fact]
        public async Task CreatePostAsync_CreatesPublishedPost_WithCategory()
        {
            // Arrange
            var category = Helpers.TestDataBuilder.CreateCategory();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "New Post",
                Description = "Description",
                Content = "Content",
                Category = "Test Category"
            };

            var createdPost = new Post { Id = 1, Title = model.Title };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Post>()))
                .ReturnsAsync((Post p) => { p.Id = 1; return p; });

            // Act
            var result = await _service.CreatePostAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsDraft);
            Assert.Equal(model.Title, result.Title);
            _mockRepository.Verify(r => r.AddAsync(It.Is<Post>(p => 
                p.Title == model.Title && 
                !p.IsDraft && 
                p.PostCategories.Any())), Times.Once);
        }

        [Fact]
        public async Task CreateDraftPostAsync_CreatesDraftPost_WithCategory()
        {
            // Arrange
            var category = Helpers.TestDataBuilder.CreateCategory();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "Draft Post",
                Description = "Description",
                Content = "Content",
                Category = "Test Category"
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Post>()))
                .ReturnsAsync((Post p) => { p.Id = 1; return p; });

            // Act
            var result = await _service.CreateDraftPostAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsDraft);
            Assert.Equal(model.Title, result.Title);
            _mockRepository.Verify(r => r.AddAsync(It.Is<Post>(p => 
                p.Title == model.Title && 
                p.IsDraft && 
                p.PostCategories.Any())), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_UpdatesPost_WhenExists()
        {
            // Arrange
            var existingPost = Helpers.TestDataBuilder.CreatePost(1, "Old Title", false);
            var category = Helpers.TestDataBuilder.CreateCategory();
            existingPost.PostCategories.Add(new PostCategory
            {
                PostId = 1,
                CategoryId = 1,
                Category = category
            });
            _context.Posts.Add(existingPost);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var model = new PostViewModel
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Content = "Updated Content",
                Category = "New Category"
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Post>()))
                .ReturnsAsync((Post p) => p);

            // Act
            var result = await _service.UpdatePostAsync(1, model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Title, result.Title);
            Assert.Equal(model.Description, result.Description);
            Assert.NotNull(result.UpdatedDate);
        }

        [Fact]
        public async Task UpdatePostAsync_ThrowsKeyNotFoundException_WhenPostNotExists()
        {
            // Arrange
            var model = new PostViewModel
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Content = "Updated Content"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdatePostAsync(999, model));
        }

        [Fact]
        public async Task DeletePostAsync_ReturnsTrue_WhenPostExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeletePostAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletePostAsync_ReturnsFalse_WhenPostNotExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _service.DeletePostAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MapPostToViewModel_MapsCorrectly_WithCategory()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            var category = Helpers.TestDataBuilder.CreateCategory();
            post.PostCategories.Add(new PostCategory
            {
                PostId = 1,
                CategoryId = 1,
                Category = category
            });

            // Act
            var result = _service.MapPostToViewModel(post);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(post.Id, result.Id);
            Assert.Equal(post.Title, result.Title);
            Assert.Equal(post.Description, result.Description);
            Assert.Equal(post.Content, result.Content);
            Assert.Equal(post.IsDraft, result.IsDraft);
            Assert.Equal(category.Title, result.Category);
        }

        [Fact]
        public void MapPostToViewModel_MapsCorrectly_WithoutCategory()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();

            // Act
            var result = _service.MapPostToViewModel(post);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Category);
        }
    }
}

