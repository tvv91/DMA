using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
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

        [Fact]
        public async Task GetDigitizationsByEquipmentAsync_ReturnsDigitizations_WhenEquipmentUsed()
        {
            // Arrange: Artist, Genre, Albums, Manufacturer, Player, EquipmentInfo, Digitizations
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album1 = new Album { Title = "Album A", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            var album2 = new Album { Title = "Album B", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.AddRange(album1, album2);
            await _context.SaveChangesAsync();

            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(0, "Mfr", EntityType.PlayerManufacturer);
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var player = new Player { Name = "Test Player", ManufacturerId = manufacturer.Id };
            _context.Set<Player>().Add(player);
            await _context.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
            _context.EquipmentInfos.Add(equipmentInfo);
            await _context.SaveChangesAsync();

            var digitization1 = Helpers.TestDataBuilder.CreateDigitization(0, album1.Id);
            digitization1.EquipmentInfoId = equipmentInfo.Id;
            var digitization2 = Helpers.TestDataBuilder.CreateDigitization(0, album2.Id);
            digitization2.EquipmentInfoId = equipmentInfo.Id;
            _context.Digitizations.AddRange(digitization1, digitization2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDigitizationsByEquipmentAsync(EntityType.Player, player.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, d => Assert.Equal(equipmentInfo.Id, d.EquipmentInfoId));
        }

        [Fact]
        public async Task GetDigitizationsByEquipmentAsync_ReturnsEmpty_WhenEquipmentNotUsed()
        {
            // Arrange: create player and equipment info but no digitizations using it
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(0, "Mfr", EntityType.PlayerManufacturer);
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();
            var player = new Player { Name = "Unused Player", ManufacturerId = manufacturer.Id };
            _context.Set<Player>().Add(player);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDigitizationsByEquipmentAsync(EntityType.Player, player.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAlbumsDigitizedByEquipmentPagedAsync_ReturnsPagedDistinctAlbums_OrderedByArtistAndTitle()
        {
            // Arrange: two albums, same player used in two digitizations for one album and one for the other
            var artist = Helpers.TestDataBuilder.CreateArtist(0, "Artist X");
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var album1 = new Album { Title = "Album First", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            var album2 = new Album { Title = "Album Second", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            _context.Albums.AddRange(album1, album2);
            await _context.SaveChangesAsync();

            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(0, "Mfr", EntityType.AdcManufacturer);
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();
            var adc = new Adc { Name = "Test ADC", ManufacturerId = manufacturer.Id };
            _context.Set<Adc>().Add(adc);
            await _context.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { AdcId = adc.Id };
            _context.EquipmentInfos.Add(equipmentInfo);
            await _context.SaveChangesAsync();

            var digitization1 = Helpers.TestDataBuilder.CreateDigitization(0, album1.Id);
            digitization1.EquipmentInfoId = equipmentInfo.Id;
            var digitization2 = Helpers.TestDataBuilder.CreateDigitization(0, album1.Id);
            digitization2.EquipmentInfoId = equipmentInfo.Id;
            var digitization3 = Helpers.TestDataBuilder.CreateDigitization(0, album2.Id);
            digitization3.EquipmentInfoId = equipmentInfo.Id;
            _context.Digitizations.AddRange(digitization1, digitization2, digitization3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAlbumsDigitizedByEquipmentPagedAsync(EntityType.Adc, adc.Id, page: 1, pageSize: 10);

            // Assert: distinct albums, so 2 not 3
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalItems);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(album1.Id, result.Items[0].Id);
            Assert.Equal(album2.Id, result.Items[1].Id);
        }

        [Fact]
        public async Task GetAlbumsDigitizedByEquipmentPagedAsync_ReturnsEmpty_WhenNoDigitizationsUseEquipment()
        {
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(0, "Mfr", EntityType.CartridgeManufacturer);
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();
            var cartridge = new Cartridge { Name = "Unused Cartridge", ManufacturerId = manufacturer.Id };
            _context.Set<Cartridge>().Add(cartridge);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAlbumsDigitizedByEquipmentPagedAsync(EntityType.Cartridge, cartridge.Id, 1, 10);

            Assert.NotNull(result);
            Assert.Equal(0, result.TotalItems);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAlbumsDigitizedByEquipmentPagedAsync_RespectsPageAndPageSize()
        {
            // Arrange: 3 albums with same wire
            var artist = Helpers.TestDataBuilder.CreateArtist();
            var genre = Helpers.TestDataBuilder.CreateGenre();
            _context.Artists.Add(artist);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var albums = new List<Album>();
            for (int i = 0; i < 3; i++)
            {
                var album = new Album { Title = $"Album {i}", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
                _context.Albums.Add(album);
                albums.Add(album);
            }
            await _context.SaveChangesAsync();

            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(0, "Mfr", EntityType.WireManufacturer);
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();
            var wire = new Wire { Name = "Test Wire", ManufacturerId = manufacturer.Id };
            _context.Set<Wire>().Add(wire);
            await _context.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { WireId = wire.Id };
            _context.EquipmentInfos.Add(equipmentInfo);
            await _context.SaveChangesAsync();

            foreach (var album in albums)
            {
                var digitization = Helpers.TestDataBuilder.CreateDigitization(0, album.Id);
                digitization.EquipmentInfoId = equipmentInfo.Id;
                _context.Digitizations.Add(digitization);
            }
            await _context.SaveChangesAsync();

            // Act: page 1, size 2
            var page1 = await _repository.GetAlbumsDigitizedByEquipmentPagedAsync(EntityType.Wire, wire.Id, 1, 2);
            var page2 = await _repository.GetAlbumsDigitizedByEquipmentPagedAsync(EntityType.Wire, wire.Id, 2, 2);

            // Assert
            Assert.Equal(3, page1.TotalItems);
            Assert.Equal(2, page1.Items.Count);
            Assert.Equal(2, page1.TotalPages);

            Assert.Equal(3, page2.TotalItems);
            Assert.Single(page2.Items);
            Assert.Equal(2, page2.TotalPages);
        }
    }
}

