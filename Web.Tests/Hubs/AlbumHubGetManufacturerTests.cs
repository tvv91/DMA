using Moq;
using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubGetManufacturerTests
{
    private readonly AlbumHubTestFactory _factory = new();
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
    [InlineData("player", EntityType.Player)]
    [InlineData("adc", EntityType.Adc)]
    public async Task GetManufacturer_ValidCategory_SendsManufacturerName(string category, EntityType type)
    {
        var player = new Player
        {
            Id = 1,
            Name = "SL-1200",
            Manufacturer = new Manufacturer { Name = "Technics" },
        };
        _factory.EquipmentServiceMock
            .Setup(s => s.GetManufacturerByNameAsync("SL-1200", type))
            .ReturnsAsync(player);

        var hub = _factory.CreateHub();
        await hub.GetManufacturer(ConnectionId, category, "SL-1200");

        var send = _factory.SendRecorder.FindSend("ReceivedManufacturer");
        Assert.NotNull(send);
        Assert.Equal(category, send.Value.Args[0]);
        Assert.Equal("Technics", send.Value.Args[1]);
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
