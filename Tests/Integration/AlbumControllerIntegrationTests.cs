using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Web.Db;
using Web.Models;
using Xunit;
using Tests.Helpers;

namespace Tests.Integration
{
    public class AlbumControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AlbumControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<int> CreateTestAlbumAsync(string title = "Test Album", string artistName = "Test Artist", string genreName = "Test Genre")
        {
            var dbName = "TestDb_Integration";
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            
            using var testContext = new DMADbContext(options);
            
            // Get or create artist
            var artist = await testContext.Artists.FirstOrDefaultAsync(a => a.Name == artistName);
            if (artist == null)
            {
                artist = Helpers.TestDataBuilder.CreateArtist(0, artistName);
                testContext.Artists.Add(artist);
                await testContext.SaveChangesAsync();
            }
            
            // Get or create genre
            var genre = await testContext.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
            if (genre == null)
            {
                genre = Helpers.TestDataBuilder.CreateGenre(0, genreName);
                testContext.Genres.Add(genre);
                await testContext.SaveChangesAsync();
            }

            var album = new Album
            {
                Title = title,
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            testContext.Albums.Add(album);
            await testContext.SaveChangesAsync();
            
            return album.Id;
        }

        [Fact]
        public async Task Index_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/album");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode_WhenAlbumExists()
        {
            // Arrange
            var albumId = await CreateTestAlbumAsync();

            // Act
            var response = await _client.GetAsync($"/album/{albumId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenAlbumNotExists()
        {
            // Act
            var response = await _client.GetAsync("/album/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.GetAsync("/album/0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/album/create");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsSuccessStatusCode_WhenAlbumExists()
        {
            // Arrange
            var albumId = await CreateTestAlbumAsync();

            // Act
            var response = await _client.GetAsync($"/album/edit/{albumId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenAlbumNotExists()
        {
            // Act
            var response = await _client.GetAsync("/album/edit/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.GetAsync("/album/edit/0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenAlbumExists()
        {
            // Arrange
            var albumId = await CreateTestAlbumAsync();

            // Act
            var response = await _client.DeleteAsync($"/album/delete?id={albumId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAlbumNotExists()
        {
            // Act
            var response = await _client.DeleteAsync("/album/delete?id=999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.DeleteAsync("/album/delete?id=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

