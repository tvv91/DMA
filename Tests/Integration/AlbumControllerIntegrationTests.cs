using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Web.Db;
using Web.Enums;
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

        /// <summary>
        /// Creates an album with one digitization that has EquipmentInfo with Player, Cartridge, Amplifier, Adc, Wire (each Id = 1).
        /// Used to verify the details page renders equipment links in the digitization table.
        /// </summary>
        private async Task<int> CreateAlbumWithDigitizationAndEquipmentAsync()
        {
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase("TestDb_Integration")
                .Options;

            using var testContext = new DMADbContext(options);

            var artist = await testContext.Artists.FirstOrDefaultAsync(a => a.Name == "Equipment Test Artist");
            if (artist == null)
            {
                artist = TestDataBuilder.CreateArtist(0, "Equipment Test Artist");
                testContext.Artists.Add(artist);
                await testContext.SaveChangesAsync();
            }

            var genre = await testContext.Genres.FirstOrDefaultAsync(g => g.Name == "Equipment Test Genre");
            if (genre == null)
            {
                genre = TestDataBuilder.CreateGenre(0, "Equipment Test Genre");
                testContext.Genres.Add(genre);
                await testContext.SaveChangesAsync();
            }

            var album = new Album
            {
                Title = "Album With Equipment",
                ArtistId = artist.Id,
                GenreId = genre.Id,
                AddedDate = DateTime.UtcNow
            };
            testContext.Albums.Add(album);
            await testContext.SaveChangesAsync();

            var playerMfr = new Manufacturer { Name = "Player Mfr", Type = EntityType.PlayerManufacturer };
            var cartridgeMfr = new Manufacturer { Name = "Cartridge Mfr", Type = EntityType.CartridgeManufacturer };
            var ampMfr = new Manufacturer { Name = "Amp Mfr", Type = EntityType.AmplifierManufacturer };
            var adcMfr = new Manufacturer { Name = "Adc Mfr", Type = EntityType.AdcManufacturer };
            var wireMfr = new Manufacturer { Name = "Wire Mfr", Type = EntityType.WireManufacturer };
            testContext.Set<Manufacturer>().AddRange(playerMfr, cartridgeMfr, ampMfr, adcMfr, wireMfr);
            await testContext.SaveChangesAsync();

            var player = new Player { Name = "Test Player", ManufacturerId = playerMfr.Id, Manufacturer = playerMfr };
            var cartridge = new Cartridge { Name = "Test Cartridge", ManufacturerId = cartridgeMfr.Id, Manufacturer = cartridgeMfr };
            var amplifier = new Amplifier { Name = "Test Amplifier", ManufacturerId = ampMfr.Id, Manufacturer = ampMfr };
            var adc = new Adc { Name = "Test ADC", ManufacturerId = adcMfr.Id, Manufacturer = adcMfr };
            var wire = new Wire { Name = "Test Wire", ManufacturerId = wireMfr.Id, Manufacturer = wireMfr };
            testContext.Set<Player>().Add(player);
            testContext.Set<Cartridge>().Add(cartridge);
            testContext.Set<Amplifier>().Add(amplifier);
            testContext.Set<Adc>().Add(adc);
            testContext.Set<Wire>().Add(wire);
            await testContext.SaveChangesAsync();

            var equipmentInfo = new EquipmentInfo
            {
                PlayerId = player.Id,
                CartridgeId = cartridge.Id,
                AmplifierId = amplifier.Id,
                AdcId = adc.Id,
                WireId = wire.Id
            };
            testContext.EquipmentInfos.Add(equipmentInfo);
            await testContext.SaveChangesAsync();

            var digitization = new Digitization
            {
                AlbumId = album.Id,
                EquipmentInfoId = equipmentInfo.Id,
                AddedDate = DateTime.UtcNow
            };
            testContext.Digitizations.Add(digitization);
            await testContext.SaveChangesAsync();

            return album.Id;
        }

        /// <summary>
        /// Creates an album with one digitization that has no EquipmentInfo (no Player, Cartridge, etc.).
        /// </summary>
        private async Task<int> CreateAlbumWithDigitizationWithoutEquipmentAsync()
        {
            var albumId = await CreateTestAlbumAsync("Album No Equipment", "No Equip Artist", "No Equip Genre");
            var options = new DbContextOptionsBuilder<DMADbContext>()
                .UseInMemoryDatabase("TestDb_Integration")
                .Options;
            using var testContext = new DMADbContext(options);
            var album = await testContext.Albums.FindAsync(albumId);
            if (album == null) return albumId;
            var digitization = new Digitization { AlbumId = album.Id, AddedDate = DateTime.UtcNow };
            testContext.Digitizations.Add(digitization);
            await testContext.SaveChangesAsync();
            return album.Id;
        }

        [Fact]
        public async Task GetById_DetailsPage_ContainsEquipmentLinksInDigitizationTable_WhenDigitizationHasEquipment()
        {
            // Arrange: album with digitization that has Player, Cartridge, Amplifier, Adc, Wire
            var albumId = await CreateAlbumWithDigitizationAndEquipmentAsync();

            // Act
            var response = await _client.GetAsync($"/album/{albumId}");
            var html = await response.Content.ReadAsStringAsync();

            // Assert: digitization table must contain links to equipment pages (target _blank)
            response.EnsureSuccessStatusCode();
            Assert.Contains("href=\"/equipment/Player/", html);
            Assert.Contains("href=\"/equipment/Cartridge/", html);
            Assert.Contains("href=\"/equipment/Amplifier/", html);
            Assert.Contains("href=\"/equipment/Adc/", html);
            Assert.Contains("href=\"/equipment/Wire/", html);
            Assert.Contains("target=\"_blank\"", html);
        }

        [Fact]
        public async Task GetById_DetailsPage_ShowsNoDataInDigitizationTable_WhenEquipmentIsMissing()
        {
            // Arrange: album with digitization but no equipment
            var albumId = await CreateAlbumWithDigitizationWithoutEquipmentAsync();

            // Act
            var response = await _client.GetAsync($"/album/{albumId}");
            var html = await response.Content.ReadAsStringAsync();

            // Assert: table shows "No data" for equipment columns, and no equipment links in table body
            response.EnsureSuccessStatusCode();
            Assert.Contains("tech-no-data", html);
            Assert.Contains("No data", html);
            // Digitization table body should not contain equipment links (only "No data" spans)
            var tableBodyStart = html.IndexOf("id=\"digitizationTableBody\"", StringComparison.OrdinalIgnoreCase);
            var tableBodyEnd = html.IndexOf("</div>", tableBodyStart + 1);
            if (tableBodyStart >= 0 && tableBodyEnd > tableBodyStart)
            {
                var tableBody = html[tableBodyStart..tableBodyEnd];
                Assert.DoesNotContain("href=\"/equipment/Player/", tableBody);
                Assert.DoesNotContain("href=\"/equipment/Cartridge/", tableBody);
            }
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

