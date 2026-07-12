using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceCreateTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task CreateEquipmentAsync_NewEquipment_CreatesRow(EntityType type)
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(type, modelName: $"{type} Created");

        var result = await _factory.Service.CreateEquipmentAsync(request);

        Assert.True(result.Id > 0);
        Assert.Equal($"{type} Created", result.Name);
        Assert.Equal(1, _factory.CountEquipment(type));
    }

    [Fact]
    public async Task CreateEquipmentAsync_WithNewManufacturer_CreatesManufacturerAndLinksEquipment()
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Player,
            modelName: "PL-500",
            manufacturer: "Denon");

        var result = await _factory.Service.CreateEquipmentAsync(request);

        Assert.NotNull(result.ManufacturerId);
        Assert.Equal("Denon", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task CreateEquipmentAsync_WithExistingManufacturer_ReusesManufacturer()
    {
        var existing = new Manufacturer { Name = "Rega" };
        _factory.Context.Manufacturer.Add(existing);
        await _factory.Context.SaveChangesAsync();

        var request = EquipmentServiceTestFactory.CreateViewModel(EntityType.Cartridge, manufacturer: "Rega");

        var result = await _factory.Service.CreateEquipmentAsync(request);

        Assert.Equal(existing.Id, result.ManufacturerId);
        Assert.Single(_factory.Context.Manufacturer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateEquipmentAsync_WithoutManufacturer_DoesNotLinkManufacturer(string? manufacturer)
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(EntityType.Amplifier, manufacturer: manufacturer);

        var result = await _factory.Service.CreateEquipmentAsync(request);

        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task CreateEquipmentAsync_ManufacturerWithWhitespace_StoresTrimmedManufacturerName()
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Wire,
            manufacturer: "  Van den Hul  ");

        var result = await _factory.Service.CreateEquipmentAsync(request);

        Assert.Equal("Van den Hul", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
