using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceUpdateTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task UpdateEquipmentAsync_ExistingEquipment_UpdatesFields()
    {
        var seeded = await _factory.SeedEquipmentAsync(EntityType.Player, "Old Name", "Old Brand", "Old desc");
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Player,
            id: seeded.Id,
            modelName: "New Name",
            manufacturer: "New Brand",
            description: "New desc");

        var result = await _factory.Service.UpdateEquipmentAsync(request);

        Assert.Equal(seeded.Id, result.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("New desc", result.Description);
        Assert.Equal("New Brand", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task UpdateEquipmentAsync_ClearManufacturer_SetsManufacturerToNull()
    {
        var seeded = await _factory.SeedEquipmentAsync(EntityType.Adc, "ADS-1", "Tascam");
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Adc,
            id: seeded.Id,
            modelName: "ADS-1",
            manufacturer: null,
            description: "Updated");

        var result = await _factory.Service.UpdateEquipmentAsync(request);

        Assert.Null(result.ManufacturerId);
        Assert.Null(result.Manufacturer);
    }

    [Fact]
    public async Task UpdateEquipmentAsync_ChangesManufacturer_UpdatesLink()
    {
        var seeded = await _factory.SeedEquipmentAsync(EntityType.Cartridge, "VM95", "Brand A");
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Cartridge,
            id: seeded.Id,
            modelName: "VM95",
            manufacturer: "Brand B");

        var result = await _factory.Service.UpdateEquipmentAsync(request);

        Assert.Equal("Brand B", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task UpdateEquipmentAsync_AllSupportedTypes_UpdatesExistingRow(EntityType type)
    {
        var seeded = await _factory.SeedEquipmentAsync(type, "Before Update");
        var request = EquipmentServiceTestFactory.CreateViewModel(
            type,
            id: seeded.Id,
            modelName: "After Update");

        var result = await _factory.Service.UpdateEquipmentAsync(request);

        Assert.Equal("After Update", result.Name);
        Assert.Equal(1, _factory.CountEquipment(type));
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
