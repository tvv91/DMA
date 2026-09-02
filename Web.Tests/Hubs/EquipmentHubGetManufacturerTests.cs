using Moq;
using Web.Interfaces;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

public class EquipmentHubGetManufacturerTests
{
    private readonly EquipmentHubTestFactory _factory = new();
    private const string ConnectionId = "conn-mfg";

    [Fact]
    public async Task GetManufacturer_UnknownCategory_SendsEmptyManufacturer()
    {
        var hub = _factory.CreateHub();
        await hub.GetManufacturer(ConnectionId, "unknown", "Technics");

        var send = _factory.SendRecorder.FindSend("ReceivedManufacturer");
        Assert.NotNull(send);
        Assert.Equal("unknown", send.Value.Args[0]);
        Assert.Equal(string.Empty, send.Value.Args[1]);
        _factory.EquipmentServiceMock.Verify(
            s => s.GetManufacturerByNameAsync(It.IsAny<string>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Theory]
    [InlineData("amplifier", EntityType.Amplifier)]
    [InlineData("cartridge", EntityType.Cartridge)]
    public async Task GetManufacturer_ValidCategory_SendsManufacturerName(string category, EntityType type)
    {
        IManufacturer equipment = category switch
        {
            "amplifier" => new Amplifier
            {
                Id = 1,
                Name = "PM-6007",
                Manufacturer = new Manufacturer { Name = "Marantz" },
            },
            _ => new Cartridge
            {
                Id = 2,
                Name = "AT-VM95E",
                Manufacturer = new Manufacturer { Name = "Audio-Technica" },
            },
        };
        var modelName = equipment.Name;
        var manufacturerName = equipment.Manufacturer!.Name;

        _factory.EquipmentServiceMock
            .Setup(s => s.GetManufacturerByNameAsync(modelName, type))
            .ReturnsAsync(equipment);

        var hub = _factory.CreateHub();
        await hub.GetManufacturer(ConnectionId, category, modelName);

        var send = _factory.SendRecorder.FindSend("ReceivedManufacturer");
        Assert.NotNull(send);
        Assert.Equal(category, send.Value.Args[0]);
        Assert.Equal(manufacturerName, send.Value.Args[1]);
    }

    [Fact]
    public async Task GetManufacturer_EquipmentWithoutManufacturer_SendsEmptyString()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.GetManufacturerByNameAsync("Generic", EntityType.Wire))
            .ReturnsAsync(new Wire { Id = 1, Name = "Generic", Manufacturer = null });

        var hub = _factory.CreateHub();
        await hub.GetManufacturer(ConnectionId, "wire", "Generic");

        var send = _factory.SendRecorder.FindSend("ReceivedManufacturer");
        Assert.NotNull(send);
        Assert.Equal(string.Empty, send.Value.Args[1]);
    }
}
