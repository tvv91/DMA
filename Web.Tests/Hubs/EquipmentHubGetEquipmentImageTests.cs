using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

public class EquipmentHubGetEquipmentImageTests
{
    private readonly EquipmentHubTestFactory _factory = new();
    private const string ConnectionId = "conn-image";

    [Fact]
    public async Task GetEquipmentImage_SendsImageUrlToClient()
    {
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(5, EntityType.Player))
            .ReturnsAsync("/covers/player/5.jpg");

        var hub = _factory.CreateHub();
        await hub.GetEquipmentImage(ConnectionId, 5, "Player");

        var send = _factory.SendRecorder.FindSend("ReceivedEquipmentImageDetailed");
        Assert.NotNull(send);
        Assert.Equal("/covers/player/5.jpg", send.Value.Args[0]);
    }

    [Fact]
    public async Task GetEquipmentImage_InvalidType_Throws()
    {
        var hub = _factory.CreateHub();

        await Assert.ThrowsAsync<ArgumentException>(
            () => hub.GetEquipmentImage(ConnectionId, 1, "NotAnEntityType"));
    }
}
