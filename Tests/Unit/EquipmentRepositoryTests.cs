using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Implementation;
using Web.Models;
using Xunit;

namespace Tests.Unit
{
    public class EquipmentRepositoryTests : IDisposable
    {
        private readonly DMADbContext _context;
        private readonly EquipmentRepository _repository;

        public EquipmentRepositoryTests()
        {
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _repository = new EquipmentRepository(_context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task AddAsync_AddsAdc_AndReturnsIt()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var adc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC", manufacturer);

            // Act
            var result = await _repository.AddAsync(adc, EntityType.Adc);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            var savedAdc = await _context.Set<Adc>().FindAsync(result.Id);
            Assert.NotNull(savedAdc);
            Assert.Equal(adc.Name, savedAdc.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAdc_WhenExists()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var adc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC", manufacturer);
            _context.Set<Adc>().Add(adc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(adc.Id, EntityType.Adc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(adc.Id, result.Id);
            Assert.Equal(adc.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(999, EntityType.Adc);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_IncludesManufacturer()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(1, "Test Manufacturer");
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var adc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC", manufacturer);
            _context.Set<Adc>().Add(adc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(adc.Id, EntityType.Adc) as Adc;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Manufacturer);
            Assert.Equal("Test Manufacturer", result.Manufacturer.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WorksForAllEquipmentTypes()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var player = Helpers.TestDataBuilder.CreatePlayer(1, "Test Player", manufacturer);
            var amplifier = Helpers.TestDataBuilder.CreateAmplifier(1, "Test Amplifier", manufacturer);
            var cartridge = Helpers.TestDataBuilder.CreateCartridge(1, "Test Cartridge", manufacturer);
            var wire = Helpers.TestDataBuilder.CreateWire(1, "Test Wire", manufacturer);

            _context.Set<Player>().Add(player);
            _context.Set<Amplifier>().Add(amplifier);
            _context.Set<Cartridge>().Add(cartridge);
            _context.Set<Wire>().Add(wire);
            await _context.SaveChangesAsync();

            // Act & Assert
            var playerResult = await _repository.GetByIdAsync(player.Id, EntityType.Player);
            Assert.NotNull(playerResult);

            var amplifierResult = await _repository.GetByIdAsync(amplifier.Id, EntityType.Amplifier);
            Assert.NotNull(amplifierResult);

            var cartridgeResult = await _repository.GetByIdAsync(cartridge.Id, EntityType.Cartridge);
            Assert.NotNull(cartridgeResult);

            var wireResult = await _repository.GetByIdAsync(wire.Id, EntityType.Wire);
            Assert.NotNull(wireResult);
        }

        [Fact]
        public async Task DeleteAsync_DeletesEquipment_WhenExists()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var adc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC", manufacturer);
            _context.Set<Adc>().Add(adc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(adc.Id, EntityType.Adc);

            // Assert
            Assert.True(result);
            var deletedAdc = await _context.Set<Adc>().FindAsync(adc.Id);
            Assert.Null(deletedAdc);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
        {
            // Act
            var result = await _repository.DeleteAsync(999, EntityType.Adc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetListAsync_ReturnsPagedResults()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            for (int i = 1; i <= 5; i++)
            {
                var adc = Helpers.TestDataBuilder.CreateAdc(i, $"ADC {i}", manufacturer);
                _context.Set<Adc>().Add(adc);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetListAsync(1, 3, EntityType.Adc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalItems);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEquipment()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var adc = Helpers.TestDataBuilder.CreateAdc(1, "Original Name", manufacturer);
            _context.Set<Adc>().Add(adc);
            await _context.SaveChangesAsync();

            adc.Name = "Updated Name";

            // Act
            var result = await _repository.UpdateAsync(adc, EntityType.Adc);

            // Assert
            Assert.NotNull(result);
            var updatedAdc = await _context.Set<Adc>().FindAsync(adc.Id);
            Assert.NotNull(updatedAdc);
            Assert.Equal("Updated Name", updatedAdc.Name);
        }
    }
}


