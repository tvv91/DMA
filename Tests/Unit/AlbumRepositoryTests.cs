using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Implementation;
using Web.Models;
using Xunit;

namespace Tests.Unit
{
    public class AlbumRepositoryTests : IDisposable
    {
        private readonly DMADbContext _context;
        private readonly AlbumRepository _repository;

        public AlbumRepositoryTests()
        {
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _repository = new AlbumRepository(_context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task AddAsync_AddsAlbum_AndReturnsIt()
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

            // Act
            var result = await _repository.AddAsync(album);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            var savedAlbum = await _context.Albums.FindAsync(result.Id);
            Assert.NotNull(savedAlbum);
            Assert.Equal(album.Title, savedAlbum.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAlbum_WhenExists()
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
            var result = await _repository.GetByIdAsync(album.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(album.Id, result.Id);
            Assert.Equal(album.Title, result.Title);
            Assert.NotNull(result.Artist);
            Assert.NotNull(result.Genre);
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
        public async Task GetByIdAsync_IncludesArtistAndGenre()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist(0, "Test Artist");
            var genre = Helpers.TestDataBuilder.CreateGenre(0, "Test Genre");
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
            var result = await _repository.GetByIdAsync(album.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Artist);
            Assert.Equal("Test Artist", result.Artist.Name);
            Assert.NotNull(result.Genre);
            Assert.Equal("Test Genre", result.Genre.Name);
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
                Title = "Original Title",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            album.Title = "Updated Title";

            // Act
            var result = await _repository.UpdateAsync(album);

            // Assert
            Assert.NotNull(result);
            var updatedAlbum = await _context.Albums.FindAsync(album.Id);
            Assert.NotNull(updatedAlbum);
            Assert.Equal("Updated Title", updatedAlbum.Title);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAlbum_WhenExists()
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
            var result = await _repository.DeleteAsync(album.Id);

            // Assert
            Assert.True(result);
            var deletedAlbum = await _context.Albums.FindAsync(album.Id);
            Assert.Null(deletedAlbum);
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

            for (int i = 1; i <= 5; i++)
            {
                var album = new Album
                {
                    Title = $"Album {i}",
                    ArtistId = artist.Id,
                    GenreId = genre.Id,
                    AddedDate = DateTime.UtcNow
                };
                _context.Albums.Add(album);
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

        [Fact]
        public async Task GetIndexListAsync_FiltersByArtistName()
        {
            // Arrange
            var artist1 = Helpers.TestDataBuilder.CreateArtist(0, "Artist One");
            var artist2 = Helpers.TestDataBuilder.CreateArtist(0, "Artist Two");
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.AddRange(artist1, artist2);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album1 = new Album { Title = "Album 1", ArtistId = artist1.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            var album2 = new Album { Title = "Album 2", ArtistId = artist2.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.AddRange(album1, album2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetIndexListAsync(1, 10, "Artist One", null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Album 1", result.Items[0].Title);
        }

        [Fact]
        public async Task GetIndexListAsync_FiltersByGenreName()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre1 = Helpers.TestDataBuilder.CreateGenre(0, "Rock");
            var genre2 = Helpers.TestDataBuilder.CreateGenre(0, "Jazz");
            _context.Artists.Add(artist);
            _context.Genres.AddRange(genre1, genre2);
            await _context.SaveChangesAsync();

            var album1 = new Album { Title = "Album 1", ArtistId = artist.Id, GenreId = genre1.Id, AddedDate = DateTime.UtcNow };
            var album2 = new Album { Title = "Album 2", ArtistId = artist.Id, GenreId = genre2.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.AddRange(album1, album2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetIndexListAsync(1, 10, null, "Rock", null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Album 1", result.Items[0].Title);
        }

        [Fact]
        public async Task GetIndexListAsync_FiltersByAlbumTitle()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album1 = new Album { Title = "Rock Album", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            var album2 = new Album { Title = "Jazz Album", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.AddRange(album1, album2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetIndexListAsync(1, 10, null, null, null, "Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Rock Album", result.Items[0].Title);
        }

        [Fact]
        public async Task FindByTitleAndArtistAsync_ReturnsAlbum_WhenFound()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist(0, "Test Artist");
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
            var result = await _repository.FindByAlbumAndArtistAsync("Test Album", "Test Artist");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(album.Id, result.Id);
        }

        [Fact]
        public async Task FindByTitleAndArtistAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _repository.FindByAlbumAndArtistAsync("Non-existent", "Artist");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByTitleAndArtistAsync_IsCaseInsensitive()
        {
            // Arrange
            var artist = Helpers.TestDataBuilder.CreateArtist(0, "Test Artist");
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
            var result = await _repository.FindByAlbumAndArtistAsync("test album", "test artist");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(album.Id, result.Id);
        }

        [Fact]
        public async Task GetAlbumsByEquipmentAsync_ReturnsAlbums_WhenPlayerUsed()
        {
            // Arrange: album with digitization that has EquipmentInfo with Player
            var artist = Helpers.TestDataBuilder.CreateArtist(0, "Test Artist");
            var genre = Helpers.TestDataBuilder.CreateGenre(0, "Test Genre");
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Album With Player", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var mfr = new Manufacturer { Name = "Player Mfr", Type = EntityType.PlayerManufacturer };
            _context.Manufacturer.Add(mfr);
            await _context.SaveChangesAsync();

            var player = new Player { Name = "Test Player", ManufacturerId = mfr.Id };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
            _context.EquipmentInfos.Add(equipmentInfo);
            await _context.SaveChangesAsync();

            var digitization = new Digitization { AlbumId = album.Id, EquipmentInfoId = equipmentInfo.Id, AddedDate = DateTime.UtcNow };
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAlbumsByEquipmentAsync(EntityType.Player, player.Id, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalItems);
            Assert.Single(result.Items);
            Assert.Equal(album.Id, result.Items[0].Id);
            Assert.Equal("Album With Player", result.Items[0].Title);
        }

        [Fact]
        public async Task GetAlbumsByEquipmentAsync_ReturnsEmpty_WhenNoDigitizationsUseEquipment()
        {
            var mfr = new Manufacturer { Name = "Orphan Mfr", Type = EntityType.PlayerManufacturer };
            _context.Manufacturer.Add(mfr);
            await _context.SaveChangesAsync();

            var player = new Player { Name = "Orphan Player", ManufacturerId = mfr.Id };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAlbumsByEquipmentAsync(EntityType.Player, player.Id, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalItems);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAlbumsByEquipmentAsync_RespectsPagination()
        {
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var mfr = new Manufacturer { Name = "Mfr", Type = EntityType.AdcManufacturer };
            _context.Manufacturer.Add(mfr);
            await _context.SaveChangesAsync();

            var adc = new Adc { Name = "Test ADC", ManufacturerId = mfr.Id };
            _context.Adces.Add(adc);
            await _context.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { AdcId = adc.Id };
            _context.EquipmentInfos.Add(equipmentInfo);
            await _context.SaveChangesAsync();

            for (int i = 1; i <= 5; i++)
            {
                var album = new Album { Title = $"Album {i}", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
                var d = new Digitization { AlbumId = album.Id, EquipmentInfoId = equipmentInfo.Id, AddedDate = DateTime.UtcNow };
                _context.Digitizations.Add(d);
            }
            await _context.SaveChangesAsync();

            var page1 = await _repository.GetAlbumsByEquipmentAsync(EntityType.Adc, adc.Id, 1, 2);
            var page2 = await _repository.GetAlbumsByEquipmentAsync(EntityType.Adc, adc.Id, 2, 2);

            Assert.Equal(5, page1.TotalItems);
            Assert.Equal(2, page1.Items.Count);
            Assert.Equal(5, page2.TotalItems);
            Assert.Equal(2, page2.Items.Count);
            Assert.NotEqual(page1.Items[0].Id, page2.Items[0].Id);
        }

        [Fact]
        public async Task GetAlbumsByEquipmentAsync_ReturnsAlbums_WhenCartridgeUsed()
        {
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Cartridge Album", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var mfr = new Manufacturer { Name = "Cart Mfr", Type = EntityType.CartridgeManufacturer };
            _context.Manufacturer.Add(mfr);
            await _context.SaveChangesAsync();

            var cartridge = new Cartridge { Name = "Test Cartridge", ManufacturerId = mfr.Id };
            _context.Cartridges.Add(cartridge);
            await _context.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { CartridgeId = cartridge.Id };
            _context.EquipmentInfos.Add(equipmentInfo);
            await _context.SaveChangesAsync();

            _context.Digitizations.Add(new Digitization { AlbumId = album.Id, EquipmentInfoId = equipmentInfo.Id, AddedDate = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAlbumsByEquipmentAsync(EntityType.Cartridge, cartridge.Id, 1, 10);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Cartridge Album", result.Items[0].Title);
        }

        [Fact]
        public async Task GetAlbumsByEquipmentAsync_ReturnsDistinctAlbums_WhenMultipleDigitizationsSameAlbum()
        {
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "One Album", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var mfr = new Manufacturer { Name = "Wire Mfr", Type = EntityType.WireManufacturer };
            _context.Manufacturer.Add(mfr);
            await _context.SaveChangesAsync();

            var wire = new Wire { Name = "Test Wire", ManufacturerId = mfr.Id };
            _context.Wires.Add(wire);
            await _context.SaveChangesAsync();

            var eq1 = new EquipmentInfo { WireId = wire.Id };
            var eq2 = new EquipmentInfo { WireId = wire.Id };
            _context.EquipmentInfos.AddRange(eq1, eq2);
            await _context.SaveChangesAsync();

            _context.Digitizations.Add(new Digitization { AlbumId = album.Id, EquipmentInfoId = eq1.Id, AddedDate = DateTime.UtcNow });
            _context.Digitizations.Add(new Digitization { AlbumId = album.Id, EquipmentInfoId = eq2.Id, AddedDate = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAlbumsByEquipmentAsync(EntityType.Wire, wire.Id, 1, 10);

            Assert.Equal(1, result.TotalItems);
            Assert.Single(result.Items);
        }
    }
}

