using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Implementation;
using Web.Models;
using Xunit;

namespace Tests.Unit
{
    public class DigitizationRepositoryTests : IDisposable
    {
        private readonly DMADbContext _context;
        private readonly DigitizationRepository _repository;

        public DigitizationRepositoryTests()
        {
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _repository = new DigitizationRepository(_context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task AddAsync_AddsDigitization_AndReturnsIt()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var digitization = Helpers.TestDataBuilder.CreateDigitization(0, album.Id);

            // Act
            var result = await _repository.AddAsync(digitization);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            var savedDigitization = await _context.Digitizations.FindAsync(result.Id);
            Assert.NotNull(savedDigitization);
            Assert.Equal(digitization.AlbumId, savedDigitization.AlbumId);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDigitization_WhenExists()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var digitization = Helpers.TestDataBuilder.CreateDigitization(0, album.Id);
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(digitization.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(digitization.Id, result.Id);
            Assert.Equal(digitization.AlbumId, result.AlbumId);
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
        public async Task GetByAlbumIdAsync_ReturnsDigitizations_ForAlbum()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album1 = new Album { Title = "Album 1", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            var album2 = new Album { Title = "Album 2", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.AddRange(album1, album2);
            await _context.SaveChangesAsync();

            var digitization1 = Helpers.TestDataBuilder.CreateDigitization(0, album1.Id);
            var digitization2 = Helpers.TestDataBuilder.CreateDigitization(0, album1.Id);
            var digitization3 = Helpers.TestDataBuilder.CreateDigitization(0, album2.Id);
            _context.Digitizations.AddRange(digitization1, digitization2, digitization3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByAlbumIdAsync(album1.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, d => Assert.Equal(album1.Id, d.AlbumId));
        }

        [Fact]
        public async Task GetByAlbumIdAsync_ReturnsEmpty_WhenNoDigitizations()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByAlbumIdAsync(album.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_SavesChanges()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var digitization = Helpers.TestDataBuilder.CreateDigitization(0, album.Id, "Original Source");
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();

            digitization.Source = "Updated Source";

            // Act
            var result = await _repository.UpdateAsync(digitization);

            // Assert
            Assert.NotNull(result);
            var updatedDigitization = await _context.Digitizations.FindAsync(digitization.Id);
            Assert.NotNull(updatedDigitization);
            Assert.Equal("Updated Source", updatedDigitization.Source);
        }

        [Fact]
        public async Task DeleteAsync_DeletesDigitization_WhenExists()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var digitization = Helpers.TestDataBuilder.CreateDigitization(0, album.Id);
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(digitization.Id);

            // Assert
            Assert.True(result);
            var deletedDigitization = await _context.Digitizations.FindAsync(digitization.Id);
            Assert.Null(deletedDigitization);
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
        public async Task GetListAsync_ReturnsPagedResults()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            for (int i = 1; i <= 5; i++)
            {
                var digitization = Helpers.TestDataBuilder.CreateDigitization(0, album.Id);
                _context.Digitizations.Add(digitization);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetListAsync(1, 3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalItems);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(2, result.TotalPages);
        }
    }
}

