using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Implementation;
using Web.Models;
using Xunit;

namespace Tests.Unit
{
    public class PostRepositoryTests : IDisposable
    {
        private readonly DMADbContext _context;
        private readonly PostRepository _repository;

        public PostRepositoryTests()
        {
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _repository = new PostRepository(_context);
        }

        [Fact]
        public async Task AddAsync_AddsPost_AndReturnsIt()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();

            // Act
            var result = await _repository.AddAsync(post);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            var savedPost = await _context.Posts.FindAsync(result.Id);
            Assert.NotNull(savedPost);
            Assert.Equal(post.Title, savedPost.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPost_WhenExists()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(post.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(post.Id, result.Id);
            Assert.Equal(post.Title, result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_IncludesCategories()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            var category = Helpers.TestDataBuilder.CreateCategory();
            _context.Categories.Add(category);
            _context.Posts.Add(post);
            post.PostCategories.Add(new PostCategory
            {
                PostId = post.Id,
                CategoryId = category.Id,
                Category = category
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(post.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.PostCategories);
            Assert.Single(result.PostCategories);
        }

        [Fact]
        public async Task GetListAsync_ReturnsPagedResults()
        {
            // Arrange
            for (int i = 1; i <= 10; i++)
            {
                var post = Helpers.TestDataBuilder.CreatePost(i, $"Post {i}");
                post.CreatedDate = DateTime.UtcNow.AddDays(-i);
                _context.Posts.Add(post);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetListAsync(1, 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TotalItems);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(5, result.Items.Count);
        }

        [Fact]
        public async Task GetListAsync_OrdersByCreatedDateDescending()
        {
            // Arrange
            var post1 = Helpers.TestDataBuilder.CreatePost(1, "Post 1");
            post1.CreatedDate = DateTime.UtcNow.AddDays(-2);
            var post2 = Helpers.TestDataBuilder.CreatePost(2, "Post 2");
            post2.CreatedDate = DateTime.UtcNow.AddDays(-1);
            _context.Posts.AddRange(post1, post2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetListAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Post 2", result.Items[0].Title); // Newest first
            Assert.Equal("Post 1", result.Items[1].Title);
        }

        [Fact]
        public async Task GetFilteredListAsync_FiltersBySearchText()
        {
            // Arrange
            var post1 = new Post
            {
                Id = 1,
                Title = "Test Post",
                Description = "Test Description",
                Content = "Test Content",
                IsDraft = false,
                CreatedDate = DateTime.UtcNow
            };
            var post2 = new Post
            {
                Id = 2,
                Title = "Another Post",
                Description = "Another Description",
                Content = "Another Content",
                IsDraft = false,
                CreatedDate = DateTime.UtcNow
            };
            _context.Posts.AddRange(post1, post2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetFilteredListAsync(1, 10, "Test", null, null, false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Test Post", result.Items[0].Title);
        }

        [Fact]
        public async Task GetFilteredListAsync_FiltersByCategory()
        {
            // Arrange
            var category = Helpers.TestDataBuilder.CreateCategory(1, "Developing");
            _context.Categories.Add(category);
            
            var post1 = Helpers.TestDataBuilder.CreatePost(1, "Post 1");
            post1.PostCategories.Add(new PostCategory
            {
                PostId = 1,
                CategoryId = 1,
                Category = category
            });
            
            var post2 = Helpers.TestDataBuilder.CreatePost(2, "Post 2");
            _context.Posts.AddRange(post1, post2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetFilteredListAsync(1, 10, null, "Developing", null, false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Post 1", result.Items[0].Title);
        }

        [Fact]
        public async Task GetFilteredListAsync_FiltersByYear()
        {
            // Arrange
            var post1 = Helpers.TestDataBuilder.CreatePost(1, "Post 2024");
            post1.CreatedDate = new DateTime(2024, 1, 1);
            var post2 = Helpers.TestDataBuilder.CreatePost(2, "Post 2023");
            post2.CreatedDate = new DateTime(2023, 1, 1);
            _context.Posts.AddRange(post1, post2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetFilteredListAsync(1, 10, null, null, "2024", false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Post 2024", result.Items[0].Title);
        }

        [Fact]
        public async Task GetFilteredListAsync_FiltersByDraftStatus()
        {
            // Arrange
            var draftPost = Helpers.TestDataBuilder.CreatePost(1, "Draft", true);
            var publishedPost = Helpers.TestDataBuilder.CreatePost(2, "Published", false);
            _context.Posts.AddRange(draftPost, publishedPost);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetFilteredListAsync(1, 10, null, null, null, true);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.True(result.Items[0].IsDraft);
        }

        [Fact]
        public async Task DeleteAsync_DeletesPost_WhenExists()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(post.Id);

            // Assert
            Assert.True(result);
            var deletedPost = await _context.Posts.FindAsync(post.Id);
            Assert.Null(deletedPost);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_SavesChanges()
        {
            // Arrange
            var post = Helpers.TestDataBuilder.CreatePost();
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            
            post.Title = "Updated Title";
            _context.Entry(post).State = EntityState.Modified;

            // Act
            var result = await _repository.UpdateAsync(post);

            // Assert
            Assert.NotNull(result);
            var updatedPost = await _context.Posts.FindAsync(post.Id);
            Assert.NotNull(updatedPost);
            Assert.Equal("Updated Title", updatedPost.Title);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }
    }
}

