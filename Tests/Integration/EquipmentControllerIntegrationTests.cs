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

        public EquipmentControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<int> CreateTestEquipmentAsync(EntityType type = EntityType.Adc, string name = "Test Equipment")
        {
            var dbName = "TestDb_Integration";
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase(dbName)
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
        /// Creates a Player equipment and an album that has a digitization using this player. Returns (playerId, albumId).
        /// </summary>
        private async Task<(int playerId, int albumId)> CreatePlayerAndAlbumUsingItAsync(string manufacturerName = "Parasound", string modelName = "JC 2 BP Black")
        {
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase("TestDb_Integration")
                .Options;
            using var testContext = new DMADbContext(options);

            var artist = await testContext.Artists.FirstOrDefaultAsync(a => a.Name == "Albums Section Artist");
            if (artist == null)
            {
                artist = TestDataBuilder.CreateArtist(0, "Albums Section Artist");
                testContext.Artists.Add(artist);
                await testContext.SaveChangesAsync();
            }
            var genre = await testContext.Genres.FirstOrDefaultAsync(g => g.Name == "Albums Section Genre");
            if (genre == null)
            {
                genre = TestDataBuilder.CreateGenre(0, "Albums Section Genre");
                testContext.Genres.Add(genre);
                await testContext.SaveChangesAsync();
            }

            var mfr = await testContext.Manufacturer.FirstOrDefaultAsync(m => m.Name == manufacturerName && m.Type == EntityType.PlayerManufacturer);
            if (mfr == null)
            {
                mfr = new Manufacturer { Name = manufacturerName, Type = EntityType.PlayerManufacturer };
                testContext.Manufacturer.Add(mfr);
                await testContext.SaveChangesAsync();
            }

            var player = new Player { Name = modelName, ManufacturerId = mfr.Id };
            testContext.Players.Add(player);
            await testContext.SaveChangesAsync();

            var album = new Album { Title = "Album Using Player", ArtistId = artist.Id, GenreId = genre.Id, AddedDate = DateTime.UtcNow };
            testContext.Albums.Add(album);
            await testContext.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
            testContext.EquipmentInfos.Add(equipmentInfo);
            await testContext.SaveChangesAsync();

            testContext.Digitizations.Add(new Digitization { AlbumId = album.Id, EquipmentInfoId = equipmentInfo.Id, AddedDate = DateTime.UtcNow });
            await testContext.SaveChangesAsync();

            return (player.Id, album.Id);
        }

        [Fact]
        public async Task GetById_DetailsPage_ContainsAlbumsSection_WhenEquipmentUsedInDigitizations()
        {
            var (playerId, _) = await CreatePlayerAndAlbumUsingItAsync();

            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/{playerId}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("album(s) that were digitized using", html);
            Assert.Contains("equipment-albums-section", html);
        }

        [Fact]
        public async Task GetById_DetailsPage_AcceptsPageAndPageSize()
        {
            var (playerId, _) = await CreatePlayerAndAlbumUsingItAsync();

            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/{playerId}?page=1&pageSize=5");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_DetailsPage_ContainsAlbumLinksWithTargetBlank()
        {
            var (playerId, albumId) = await CreatePlayerAndAlbumUsingItAsync();

            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/{playerId}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains($"href=\"/album/{albumId}\"", html);
            Assert.Contains("target=\"_blank\"", html);
        }

        [Fact]
        public async Task GetById_DetailsPage_ShowsEquipmentNameInHeading()
        {
            var (playerId, _) = await CreatePlayerAndAlbumUsingItAsync("Parasound", "JC 2 BP Black");

            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/{playerId}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("Parasound JC 2 BP Black", html);
        }

        [Fact]
        public async Task GetById_DetailsPage_ContainsAlbumCardGrid()
        {
            var (playerId, _) = await CreatePlayerAndAlbumUsingItAsync();

            var response = await _client.GetAsync($"/equipment/{EntityType.Player}/{playerId}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("album-card", html);
            Assert.Contains("wrapper", html);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

