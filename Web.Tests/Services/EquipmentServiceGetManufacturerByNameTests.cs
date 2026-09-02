using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceGetManufacturerByNameTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetManufacturerByNameAsync_ExactModelName_ReturnsEquipment()
    {
        var seeded = await _factory.SeedEquipmentAsync(EntityType.Player, "SL-1200", "Technics");

        var result = await _factory.Service.GetManufacturerByNameAsync("SL-1200", EntityType.Player);

        Assert.NotNull(result);
        Assert.Equal(seeded.Id, result!.Id);
        Assert.Equal("Technics", result.Manufacturer!.Name);
    }

    [Fact]
    public async Task GetManufacturerByNameAsync_DoesNotSearchByManufacturerName_ReturnsNull()
    {
        await _factory.SeedEquipmentAsync(EntityType.Player, "SL-1200", "Technics");

        var result = await _factory.Service.GetManufacturerByNameAsync("Technics", EntityType.Player);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetManufacturerByNameAsync_PaddedName_DoesNotMatchWithoutTrim()
    {
        await _factory.SeedEquipmentAsync(EntityType.Player, "SL-1200");

        var result = await _factory.Service.GetManufacturerByNameAsync("  SL-1200  ", EntityType.Player);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetManufacturerByNameAsync_UnknownName_ReturnsNull()
    {
        var result = await _factory.Service.GetManufacturerByNameAsync("Missing", EntityType.Player);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetManufacturerByNameAsync_NullName_ReturnsNull()
    {
        var result = await _factory.Service.GetManufacturerByNameAsync(null!, EntityType.Player);

        Assert.Null(result);
    }

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task GetManufacturerByNameAsync_AllSupportedTypes_FindsByModelName(EntityType type)
    {
        var seeded = await _factory.SeedEquipmentAsync(type, "Shared Lookup Name");

        var result = await _factory.Service.GetManufacturerByNameAsync("Shared Lookup Name", type);

        Assert.NotNull(result);
        Assert.Equal(seeded.Id, result!.Id);
    }

    [Fact]
    public async Task GetManufacturerByNameAsync_UnsupportedEntityType_ThrowsArgumentOutOfRangeException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _factory.Service.GetManufacturerByNameAsync("Name", EntityType.Genre));
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
