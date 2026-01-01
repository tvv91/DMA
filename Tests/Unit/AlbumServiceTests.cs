using Moq;
using Web.Common;
using Web.Db;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Unit
{
    public class AlbumServiceTests : IDisposable
    {
        private readonly Mock<IAlbumRepository> _mockAlbumRepository;
        private readonly Mock<IDigitizationService> _mockDigitizationService;
        private readonly DMADbContext _context;
        private readonly AlbumService _service;

        public AlbumServiceTests()
        {
            _mockAlbumRepository = new Mock<IAlbumRepository>();
            _mockDigitizationService = new Mock<IDigitizationService>();
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _service = new AlbumService(_mockAlbumRepository.Object, _mockDigitizationService.Object, null, _context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAlbum_WhenExists()
        {
            // Arrange
            var album = Helpers.TestDataBuilder.CreateAlbum(1, "Test Album", "Test Artist", "Test Genre");
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(album.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Album?)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAlbumDetailsAsync_ReturnsViewModel_WhenAlbumExists()
        {
            // Arrange
            var album = Helpers.TestDataBuilder.CreateAlbum(1, "Test Album", "Test Artist", "Test Genre");
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(album);
            _mockDigitizationService.Setup(s => s.GetByAlbumIdAsync(1))
                .ReturnsAsync(Enumerable.Empty<Digitization>());

            // Act
            var result = await _service.GetAlbumDetailsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(album.Id, result.AlbumId);
            Assert.Equal(album.Title, result.Title);
        }

        [Fact]
        public async Task GetAlbumDetailsAsync_ThrowsKeyNotFoundException_WhenAlbumNotExists()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Album?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetAlbumDetailsAsync(1));
        }

        [Fact]
        public async Task CreateOrFindAlbumAsync_ReturnsExistingAlbum_WhenFound()
        {
            // Arrange
            var existingAlbum = Helpers.TestDataBuilder.CreateAlbum(1, "Existing Album");
            _mockAlbumRepository.Setup(r => r.FindByTitleAndArtistAsync("Existing Album", "Artist"))
                .ReturnsAsync(existingAlbum);

            // Act
            var result = await _service.CreateOrFindAlbumAsync("Existing Album", "Artist", "Genre");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingAlbum.Id, result.Id);
            _mockAlbumRepository.Verify(r => r.FindByTitleAndArtistAsync("Existing Album", "Artist"), Times.Once);
        }

        [Fact]
        public async Task CreateOrFindAlbumAsync_CreatesNewAlbum_WhenNotFound()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.FindByTitleAndArtistAsync("New Album", "New Artist"))
                .ReturnsAsync((Album?)null);

            var newAlbum = Helpers.TestDataBuilder.CreateAlbum(1, "New Album", "New Artist", "New Genre");
            _mockAlbumRepository.Setup(r => r.AddAsync(It.IsAny<Album>()))
                .ReturnsAsync(newAlbum);

            // Act
            var result = await _service.CreateOrFindAlbumAsync("New Album", "New Artist", "New Genre");

            // Assert
            Assert.NotNull(result);
            _mockAlbumRepository.Verify(r => r.FindByTitleAndArtistAsync("New Album", "New Artist"), Times.Once);
            _mockAlbumRepository.Verify(r => r.AddAsync(It.IsAny<Album>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAlbumAsync_UpdatesAlbum_WhenExists()
        {
            // Arrange
            var originalArtist = Helpers.TestDataBuilder.CreateArtist(0, "Original Artist");
            var originalGenre = Helpers.TestDataBuilder.CreateGenre(0, "Original Genre");
            _context.Artists.Add(originalArtist);
            _context.Genres.Add(originalGenre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Original Title",
                ArtistId = originalArtist.Id,
                GenreId = originalGenre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            // Create updated artist and genre
            var updatedArtist = Helpers.TestDataBuilder.CreateArtist(0, "Updated Artist");
            var updatedGenre = Helpers.TestDataBuilder.CreateGenre(0, "Updated Genre");
            _context.Artists.Add(updatedArtist);
            _context.Genres.Add(updatedGenre);
            await _context.SaveChangesAsync();

            // Setup mock to return the updated album
            _mockAlbumRepository.Setup(r => r.UpdateAsync(It.IsAny<Album>()))
                .ReturnsAsync((Album a) => a);

            // Act
            var result = await _service.UpdateAlbumAsync(album.Id, "Updated Title", "Updated Artist", "Updated Genre");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task UpdateAlbumAsync_ThrowsKeyNotFoundException_WhenNotExists()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.UpdateAlbumAsync(999, "Title", "Artist", "Genre"));
        }

        [Fact]
        public async Task UpdateAlbumAsync_ThrowsInvalidDataException_WhenIdInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => 
                _service.UpdateAlbumAsync(0, "Title", "Artist", "Genre"));
        }

        [Fact]
        public async Task DeleteAlbumAsync_ReturnsTrue_WhenExists()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAlbumAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAlbumAsync_ReturnsFalse_WhenNotExists()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAlbumAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MapAlbumToAlbumDetailsVM_MapsCorrectly()
        {
            // Arrange
            var album = Helpers.TestDataBuilder.CreateAlbum(1, "Test Album", "Test Artist", "Test Genre");
            var digitizations = new List<Digitization>
            {
                Helpers.TestDataBuilder.CreateDigitization(1, album.Id)
            };

            // Act
            var result = _service.MapAlbumToAlbumDetailsVM(album, digitizations);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(album.Id, result.AlbumId);
            Assert.Equal("Test Album", result.Title);
            Assert.Equal("Test Artist", result.Artist);
            Assert.Equal("Test Genre", result.Genre);
            Assert.NotNull(result.Digitizations);
            Assert.Single(result.Digitizations);
        }
    }
}

