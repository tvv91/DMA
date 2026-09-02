using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubGetAlbumCoverTests
{
    private readonly AlbumHubTestFactory _factory = new();

    [Fact]
    public async Task GetAlbumCover_SendsCoverUrlToClient()
    {
        const string connectionId = "conn-1";
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(7, EntityType.AlbumCover))
            .ReturnsAsync("/covers/album/7.jpg");

        var hub = _factory.CreateHub();
        await hub.GetAlbumCover(connectionId, 7);

        var send = _factory.SendRecorder.FindSend("ReceivedAlbumCoverDetailed");
        Assert.NotNull(send);
        Assert.Equal("/covers/album/7.jpg", send.Value.Args[0]);
    }
}
