using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Web.Db;
using Web.Models;
using Xunit;
using Tests.Helpers;

namespace Tests.Integration
{
    public class PostControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PostControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<int> CreateTestPostAsync(string title = "Test Post")
        {
            // Create test data using the same database name as the factory uses
            var dbName = "TestDb_Integration";
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            
            using var testContext = new DMADbContext(options);
            var post = new Post
            {
                Title = title,
                Description = "Test Description",
                Content = "Test Content",
                IsDraft = false,
                CreatedDate = DateTime.UtcNow
            };
            testContext.Posts.Add(post);
            await testContext.SaveChangesAsync();
            
            return post.Id;
        }

        [Fact]
        public async Task Index_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/post");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task New_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/post/new");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode_WhenPostExists()
        {
            // Arrange
            var postId = await CreateTestPostAsync();

            // Act
            var response = await _client.GetAsync($"/post/{postId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPostNotExists()
        {
            // Act
            var response = await _client.GetAsync("/post/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsSuccessStatusCode_WhenPostExists()
        {
            // Arrange
            var postId = await CreateTestPostAsync();

            // Act
            var response = await _client.GetAsync($"/post/edit?id={postId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenPostNotExists()
        {
            // Act
            var response = await _client.GetAsync("/post/edit?id=999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsRedirect_WhenPostExists()
        {
            // Arrange
            var postId = await CreateTestPostAsync();

            // Act
            var response = await _client.DeleteAsync($"/post/delete?id={postId}");

            // Assert
            // Delete action returns RedirectToAction which is a redirect status (302, 301, etc.)
            Assert.True(response.IsSuccessStatusCode || 
                       response.StatusCode == HttpStatusCode.Redirect || 
                       response.StatusCode == HttpStatusCode.MovedPermanently ||
                       response.StatusCode == HttpStatusCode.Found);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.DeleteAsync("/post/delete?id=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPostNotExists()
        {
            // Act
            var response = await _client.DeleteAsync("/post/delete?id=999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public void Dispose()
        {
            // InMemory databases are automatically cleaned up, no need to call EnsureDeleted
            _client.Dispose();
        }
    }
}

