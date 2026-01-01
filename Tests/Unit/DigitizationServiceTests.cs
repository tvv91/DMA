using Microsoft.EntityFrameworkCore;
using Moq;
using Web.Db;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Xunit;

namespace Tests.Unit
{
    public class DigitizationServiceTests : IDisposable
    {
        private readonly Mock<IDigitizationRepository> _mockRepository;
        private readonly DMADbContext _context;
        private readonly DigitizationService _service;

        public DigitizationServiceTests()
        {
            _mockRepository = new Mock<IDigitizationRepository>();
            _context = Helpers.TestDbContextFactory.CreateInMemoryContext();
            _service = new DigitizationService(_mockRepository.Object, _context);
        }

        public void Dispose()
        {
            Helpers.TestDbContextFactory.Cleanup(_context);
        }

        [Fact]
        public async Task GetByAlbumIdAsync_ReturnsDigitizations()
        {
            // Arrange
            var digitizations = new List<Digitization>
            {
                Helpers.TestDataBuilder.CreateDigitization(1, 1),
                Helpers.TestDataBuilder.CreateDigitization(2, 1)
            };
            _mockRepository.Setup(r => r.GetByAlbumIdAsync(1))
                .ReturnsAsync(digitizations);

            // Act
            var result = await _service.GetByAlbumIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_SetsAddedDate_AndReturnsDigitization()
        {
            // Arrange
            var digitization = Helpers.TestDataBuilder.CreateDigitization(1, 1);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Digitization>()))
                .ReturnsAsync((Digitization d) => d);

            // Act
            var result = await _service.AddAsync(digitization);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AddedDate);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Digitization>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SetsUpdateDate_AndUpdatesDigitization()
        {
            // Arrange
            var existingDigitization = Helpers.TestDataBuilder.CreateDigitization(1, 1);
            _context.Digitizations.Add(existingDigitization);
            await _context.SaveChangesAsync();

            var updatedDigitization = Helpers.TestDataBuilder.CreateDigitization(1, 1, "Updated Source");
            updatedDigitization.UpdateDate = DateTime.UtcNow;

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Digitization>()))
                .ReturnsAsync((Digitization d) => d);

            // Act
            var result = await _service.UpdateAsync(updatedDigitization);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UpdateDate);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Digitization>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsKeyNotFoundException_WhenNotExists()
        {
            // Arrange
            var digitization = Helpers.TestDataBuilder.CreateDigitization(999, 1);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.UpdateAsync(digitization));
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}


