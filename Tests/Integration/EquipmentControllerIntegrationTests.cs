using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Xunit;
using Tests.Helpers;

namespace Tests.Integration
{
    public class EquipmentControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private const string IntegrationDbName = "TestDb_Integration";

        public EquipmentControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<int> CreateTestEquipmentAsync(EntityType type = EntityType.Adc, string name = "Test Equipment")
        {
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase(IntegrationDbName)
                .Options;
            
            using var testContext = new DMADbContext(options);
            
            // Get or create manufacturer
            var manufacturerType = type == EntityType.Adc ? EntityType.AdcManufacturer : EntityType.PlayerManufacturer;
            var manufacturer = await testContext.Manufacturer.FirstOrDefaultAsync(m => m.Name == "Test Manufacturer" && m.Type == manufacturerType);
            if (manufacturer == null)
            {
                manufacturer = Helpers.TestDataBuilder.CreateManufacturer(0, "Test Manufacturer", manufacturerType);
                testContext.Manufacturer.Add(manufacturer);
                await testContext.SaveChangesAsync();
            }

            IManufacturer equipment = type switch
            {
                EntityType.Adc => new Adc { Name = name, Description = "Test Description", ManufacturerId = manufacturer.Id },
                EntityType.Player => new Player { Name = name, Description = "Test Description", ManufacturerId = manufacturer.Id },
                EntityType.Amplifier => new Amplifier { Name = name, Description = "Test Description", ManufacturerId = manufacturer.Id },
                EntityType.Cartridge => new Cartridge { Name = name, Description = "Test Description", ManufacturerId = manufacturer.Id },
                EntityType.Wire => new Wire { Name = name, Description = "Test Description", ManufacturerId = manufacturer.Id },
                _ => throw new ArgumentException("Invalid equipment type")
            };

            switch (type)
            {
                case EntityType.Adc:
                    testContext.Set<Adc>().Add((Adc)equipment);
                    break;
                case EntityType.Player:
                    testContext.Set<Player>().Add((Player)equipment);
                    break;
                case EntityType.Amplifier:
                    testContext.Set<Amplifier>().Add((Amplifier)equipment);
                    break;
                case EntityType.Cartridge:
                    testContext.Set<Cartridge>().Add((Cartridge)equipment);
                    break;
                case EntityType.Wire:
                    testContext.Set<Wire>().Add((Wire)equipment);
                    break;
            }
            
            await testContext.SaveChangesAsync();
            return equipment.Id;
        }

        [Fact]
        public async Task Index_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/equipment");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/equipment/create");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode_WhenEquipmentExists()
        {
            // Arrange
            var equipmentId = await CreateTestEquipmentAsync(EntityType.Adc);

            // Act
            var response = await _client.GetAsync($"/equipment/{EntityType.Adc}/{equipmentId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenEquipmentNotExists()
        {
            // Act
            var response = await _client.GetAsync($"/equipment/{EntityType.Adc}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.GetAsync($"/equipment/{EntityType.Adc}/0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsSuccessStatusCode_WhenEquipmentExists()
        {
            // Arrange
            var equipmentId = await CreateTestEquipmentAsync(EntityType.Player);

            // Act
            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/{equipmentId}/edit");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenEquipmentNotExists()
        {
            // Act
            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/999/edit");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/0/edit");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenEquipmentExists()
        {
            // Arrange
            var equipmentId = await CreateTestEquipmentAsync(EntityType.Amplifier);

            // Act
            var response = await _client.DeleteAsync($"/equipment/{EntityType.Amplifier}/delete/?id={equipmentId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenEquipmentNotExists()
        {
            // Act
            var response = await _client.DeleteAsync($"/equipment/{EntityType.Amplifier}/delete/?id=999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdInvalid()
        {
            // Act
            var response = await _client.DeleteAsync($"/equipment/{EntityType.Amplifier}/delete/?id=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Seeds an album and a digitization that uses the given player, so that the equipment Albums tab can show it.
        /// </summary>
        private async Task SeedAlbumDigitizedByPlayerAsync(int playerId, string albumTitle = "Album For Equipment Tab")
        {
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase(IntegrationDbName)
                .Options;

            using var ctx = new DMADbContext(options);

            var artist = await ctx.Artists.FirstOrDefaultAsync(a => a.Name == "Equipment Tab Artist");
            if (artist == null)
            {
                artist = TestDataBuilder.CreateArtist(0, "Equipment Tab Artist");
                ctx.Artists.Add(artist);
                await ctx.SaveChangesAsync();
            }

            var genre = await ctx.Genres.FirstOrDefaultAsync(g => g.Name == "Equipment Tab Genre");
            if (genre == null)
            {
                genre = TestDataBuilder.CreateGenre(0, "Equipment Tab Genre");
                ctx.Genres.Add(genre);
                await ctx.SaveChangesAsync();
            }

            var album = new Album
            {
                Title = albumTitle,
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            ctx.Albums.Add(album);
            await ctx.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { PlayerId = playerId };
            ctx.EquipmentInfos.Add(equipmentInfo);
            await ctx.SaveChangesAsync();

            var digitization = new Digitization
            {
                AlbumId = album.Id,
                EquipmentInfoId = equipmentInfo.Id,
                AddedDate = DateTime.UtcNow
            };
            ctx.Digitizations.Add(digitization);
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetById_WithTabAlbums_ReturnsSuccess_AndShowsAlbumsContent()
        {
            // Arrange: create player and an album digitized with that player
            var playerId = await CreateTestEquipmentAsync(EntityType.Player, "Player For Albums Tab");
            await SeedAlbumDigitizedByPlayerAsync(playerId, "Album Shown On Equipment Tab");

            // Act
            var response = await _client.GetAsync($"/equipment/player/{playerId}?tab=albums&page=1&pageSize=18");
            var html = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Album Shown On Equipment Tab", html);
            Assert.Contains("Equipment Tab Artist", html);
            Assert.Contains("tab=albums", html);
        }

        [Fact]
        public async Task GetById_WithTabAlbums_ReturnsSuccess_WhenNoAlbumsDigitized()
        {
            // Arrange: player with no digitizations
            var playerId = await CreateTestEquipmentAsync(EntityType.Player, "Player With No Albums");

            // Act
            var response = await _client.GetAsync($"/equipment/player/{playerId}?tab=albums&page=1&pageSize=18");
            var html = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("No albums have been digitized with this equipment", html);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

