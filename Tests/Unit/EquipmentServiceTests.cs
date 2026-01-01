using Microsoft.EntityFrameworkCore;
using Moq;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Unit
{
    public class EquipmentServiceTests : IDisposable
    {
        private readonly Mock<IEquipmentRepository> _mockRepository;
        private readonly DMADbContext _context;
        private readonly EquipmentService _service;

        public EquipmentServiceTests()
        {
            _mockRepository = new Mock<IEquipmentRepository>();
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _service = new EquipmentService(_mockRepository.Object, _context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEquipment_WhenExists()
        {
            // Arrange
            var adc = Helpers.TestDataBuilder.CreateAdc();
            _mockRepository.Setup(r => r.GetByIdAsync(1, EntityType.Adc)).ReturnsAsync(adc);

            // Act
            var result = await _service.GetByIdAsync(1, EntityType.Adc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(adc.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, EntityType.Adc)).ReturnsAsync((IManufacturer?)null);

            // Act
            var result = await _service.GetByIdAsync(999, EntityType.Adc);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateEquipmentAsync_CreatesAdc()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var viewModel = new EquipmentViewModel
            {
                ModelName = "Test ADC",
                Description = "Test Description",
                EquipmentType = EntityType.Adc,
                Manufacturer = manufacturer.Name
            };

            var createdAdc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC", manufacturer);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<IManufacturer>(), EntityType.Adc))
                .ReturnsAsync(createdAdc);

            // Act
            var result = await _service.CreateEquipmentAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<IManufacturer>(), EntityType.Adc), Times.Once);
        }

        [Fact]
        public async Task CreateEquipmentAsync_CreatesManufacturer_WhenNotExists()
        {
            // Arrange
            var viewModel = new EquipmentViewModel
            {
                ModelName = "Test ADC",
                Description = "Test Description",
                EquipmentType = EntityType.Adc,
                Manufacturer = "New Manufacturer"
            };

            var createdAdc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC");
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<IManufacturer>(), EntityType.Adc))
                .ReturnsAsync(createdAdc);

            // Act
            var result = await _service.CreateEquipmentAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            var manufacturer = await _context.Manufacturer.FirstOrDefaultAsync(m => m.Name == "New Manufacturer");
            Assert.NotNull(manufacturer);
        }

        [Fact]
        public async Task UpdateEquipmentAsync_UpdatesEquipment()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var existingAdc = Helpers.TestDataBuilder.CreateAdc(1, "Original Name", manufacturer);
            _mockRepository.Setup(r => r.GetByIdAsync(1, EntityType.Adc)).ReturnsAsync(existingAdc);

            var viewModel = new EquipmentViewModel
            {
                Id = 1,
                ModelName = "Updated Name",
                Description = "Updated Description",
                EquipmentType = EntityType.Adc,
                Manufacturer = manufacturer.Name
            };

            var updatedAdc = Helpers.TestDataBuilder.CreateAdc(1, "Updated Name", manufacturer);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<IManufacturer>(), EntityType.Adc))
                .ReturnsAsync(updatedAdc);

            // Act
            var result = await _service.UpdateEquipmentAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<IManufacturer>(), EntityType.Adc), Times.Once);
        }

        [Fact]
        public async Task DeleteEquipmentAsync_ReturnsTrue_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1, EntityType.Adc)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteEquipmentAsync(1, EntityType.Adc);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteEquipmentAsync_ReturnsFalse_WhenNotExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999, EntityType.Adc)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteEquipmentAsync(999, EntityType.Adc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MapEquipmentToViewModel_MapsCorrectly()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer(1, "Test Manufacturer");
            var adc = Helpers.TestDataBuilder.CreateAdc(1, "Test ADC", manufacturer);

            // Act
            var result = _service.MapEquipmentToViewModel(adc, EntityType.Adc, "http://example.com/image.jpg");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(adc.Id, result.Id);
            Assert.Equal("Test ADC", result.ModelName);
            Assert.Equal("Test Description", result.Description);
            Assert.Equal(EntityType.Adc, result.EquipmentType);
            Assert.Equal("Test Manufacturer", result.Manufacturer);
            Assert.Equal("http://example.com/image.jpg", result.EquipmentCover);
        }

        [Fact]
        public async Task MapViewModelToEquipmentAsync_CreatesCorrectEquipmentType()
        {
            // Arrange
            var manufacturer = Helpers.TestDataBuilder.CreateManufacturer();
            _context.Manufacturer.Add(manufacturer);
            await _context.SaveChangesAsync();

            var viewModel = new EquipmentViewModel
            {
                ModelName = "Test Player",
                Description = "Test Description",
                EquipmentType = EntityType.Player,
                Manufacturer = manufacturer.Name
            };

            // Act
            var result = await _service.MapViewModelToEquipmentAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Player>(result);
            Assert.Equal("Test Player", result.Name);
        }
    }
}


