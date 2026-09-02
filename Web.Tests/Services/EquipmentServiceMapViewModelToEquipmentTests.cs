using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceMapViewModelToEquipmentTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task MapViewModelToEquipmentAsync_AllSupportedTypes_ReturnsCorrectEntityType(EntityType type)
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(type, modelName: "Mapped Model");

        var result = await _factory.Service.MapViewModelToEquipmentAsync(request);

        Assert.Equal("Mapped Model", result.Name);
        Assert.IsAssignableFrom<IManufacturer>(result);
        Assert.Equal(type switch
        {
            EntityType.Player => typeof(Player),
            EntityType.Adc => typeof(Adc),
            EntityType.Amplifier => typeof(Amplifier),
            EntityType.Cartridge => typeof(Cartridge),
            EntityType.Wire => typeof(Wire),
            _ => throw new InvalidOperationException(),
        }, result.GetType());
    }

    [Fact]
    public async Task MapViewModelToEquipmentAsync_UnsupportedType_ThrowsArgumentOutOfRangeException()
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(EntityType.Player);
        request.EquipmentType = EntityType.VinylState;

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _factory.Service.MapViewModelToEquipmentAsync(request));

        Assert.Equal("EquipmentType", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task MapViewModelToEquipmentAsync_WithoutManufacturer_DoesNotSetManufacturer(string? manufacturer)
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(EntityType.Amplifier, manufacturer: manufacturer);

        var result = await _factory.Service.MapViewModelToEquipmentAsync(request);

        Assert.Null(result.Manufacturer);
    }

    [Fact]
    public async Task MapViewModelToEquipmentAsync_WithExistingManufacturer_ReusesManufacturer()
    {
        var existing = new Manufacturer { Name = "Marantz" };
        _factory.Context.Manufacturer.Add(existing);
        await _factory.Context.SaveChangesAsync();

        var request = EquipmentServiceTestFactory.CreateViewModel(EntityType.Amplifier, manufacturer: "Marantz");

        var result = await _factory.Service.MapViewModelToEquipmentAsync(request);

        Assert.Equal(existing.Id, result.Manufacturer!.Id);
    }

    [Fact]
    public async Task MapViewModelToEquipmentAsync_PreservesRequestIdAndDescription()
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Player,
            id: 42,
            modelName: "Custom",
            description: "Custom description");

        var result = await _factory.Service.MapViewModelToEquipmentAsync(request);

        Assert.Equal(42, result.Id);
        Assert.Equal("Custom description", result.Description);
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
