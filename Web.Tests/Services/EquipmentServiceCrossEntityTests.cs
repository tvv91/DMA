using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceCrossEntityTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetListAsync_DifferentTypes_OnlyReturnsMatchingTableRows()
    {
        await _factory.SeedEquipmentAsync(EntityType.Player, "Player Model");
        await _factory.SeedEquipmentAsync(EntityType.Adc, "Adc Model");

        var players = await _factory.Service.GetListAsync(1, 10, EntityType.Player);
        var adcs = await _factory.Service.GetListAsync(1, 10, EntityType.Adc);

        Assert.Single(players.Items);
        Assert.Equal("Player Model", players.Items[0].Name);
        Assert.Single(adcs.Items);
        Assert.Equal("Adc Model", adcs.Items[0].Name);
    }

    [Fact]
    public async Task CreateAndGetByIdAsync_RoundTrip_ReturnsPersistedEntity()
    {
        var request = EquipmentServiceTestFactory.CreateViewModel(EntityType.Wire, modelName: "Round Trip Wire");
        var created = await _factory.Service.CreateEquipmentAsync(request);

        var loaded = await _factory.Service.GetByIdAsync(created.Id, EntityType.Wire);

        Assert.NotNull(loaded);
        Assert.Equal("Round Trip Wire", loaded!.Name);
    }

    [Fact]
    public async Task GetManufacturerByNameAsync_CaseMismatch_ReturnsNull()
    {
        await _factory.SeedEquipmentAsync(EntityType.Amplifier, "pm-6007");

        var result = await _factory.Service.GetManufacturerByNameAsync("PM-6007", EntityType.Amplifier);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateEquipmentAsync_ReusesManufacturerAcrossEquipmentTypes()
    {
        var player = await _factory.SeedEquipmentAsync(EntityType.Player, "PL-500", "Shared Brand");
        var request = EquipmentServiceTestFactory.CreateViewModel(
            EntityType.Cartridge,
            id: 0,
            modelName: "Cart-1",
            manufacturer: "Shared Brand");

        var created = await _factory.Service.CreateEquipmentAsync(request);

        Assert.Equal(player.ManufacturerId, created.ManufacturerId);
        Assert.Single(_factory.Context.Manufacturer);
    }
}
