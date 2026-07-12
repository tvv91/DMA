using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceGetByIdTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task GetByIdAsync_ExistingId_ReturnsEquipmentWithManufacturer(EntityType type)
    {
        var seeded = await _factory.SeedEquipmentAsync(type, "Loaded Model", "Loaded Brand");

        var result = await _factory.Service.GetByIdAsync(seeded.Id, type);

        Assert.NotNull(result);
        Assert.Equal("Loaded Model", result!.Name);
        Assert.NotNull(result.Manufacturer);
        Assert.Equal("Loaded Brand", result.Manufacturer!.Name);
    }

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull(EntityType type)
    {
        var result = await _factory.Service.GetByIdAsync(9999, type);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await _factory.Service.GetByIdAsync(id, EntityType.Player);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_UnsupportedEntityType_ThrowsArgumentOutOfRangeException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _factory.Service.GetByIdAsync(1, EntityType.Artist));
    }

    [Fact]
    public async Task GetByIdAsync_WithoutManufacturer_ReturnsEntityWithNullManufacturer()
    {
        var seeded = await _factory.SeedEquipmentAsync(EntityType.Wire, "Bare Wire", manufacturerName: null);

        var result = await _factory.Service.GetByIdAsync(seeded.Id, EntityType.Wire);

        Assert.NotNull(result);
        Assert.Null(result!.Manufacturer);
        Assert.Null(result.ManufacturerId);
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
