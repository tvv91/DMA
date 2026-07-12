using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceMapEquipmentToViewModelTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public void MapEquipmentToViewModel_FullEntity_MapsAllFields()
    {
        var equipment = new Player
        {
            Id = 10,
            Name = "SL-1200",
            Description = "Turntable",
            Manufacturer = new Manufacturer { Name = "Technics" },
        };

        var result = _factory.Service.MapEquipmentToViewModel(equipment, EntityType.Player, "/img/player.png");

        Assert.Equal(10, result.Id);
        Assert.Equal("SL-1200", result.ModelName);
        Assert.Equal("Turntable", result.Description);
        Assert.Equal(EntityType.Player, result.EquipmentType);
        Assert.Equal("/img/player.png", result.EquipmentCover);
        Assert.Equal("Technics", result.Manufacturer);
    }

    [Fact]
    public void MapEquipmentToViewModel_NullManufacturer_MapsNullManufacturer()
    {
        var equipment = new Wire { Id = 1, Name = "Cable", Manufacturer = null };

        var result = _factory.Service.MapEquipmentToViewModel(equipment, EntityType.Wire);

        Assert.Null(result.Manufacturer);
        Assert.Null(result.EquipmentCover);
    }

    [Fact]
    public void MapEquipmentToViewModel_NullDescription_AllowsNullDescription()
    {
        var equipment = new Adc { Id = 2, Name = "ADS-1", Description = null };

        var result = _factory.Service.MapEquipmentToViewModel(equipment, EntityType.Adc);

        Assert.Null(result.Description);
    }
}
