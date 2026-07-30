using Moq;
using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubGetAlbumCoversTests
{
    private readonly AlbumHubTestFactory _factory = new();

    [Fact]
    public async Task GetAlbumCovers_SendsCoverForEachAlbum()
    {
        const string connectionId = "conn-1";
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(It.IsAny<int>(), EntityType.AlbumCover))
            .ReturnsAsync("/covers/album/1.jpg");

        var hub = _factory.CreateHub();
        await hub.GetAlbumCovers(connectionId, [1, 2]);

        var sends = _factory.SendRecorder.FindSends("ReceivedAlbumCover");
        Assert.Equal(2, sends.Count);
        Assert.Contains(sends, s => (int)s.Args[0]! == 1);
        Assert.Contains(sends, s => (int)s.Args[0]! == 2);
    }

    [Fact]
    public async Task GetAlbumCovers_SecondCall_UsesCoverCache()
    {
        const string connectionId = "conn-1";
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(5, EntityType.AlbumCover))
            .ReturnsAsync("/covers/album/5.jpg");

        var hub = _factory.CreateHub();
        await hub.GetAlbumCovers(connectionId, [5]);
        _factory.SendRecorder.Sends.Clear();
        await hub.GetAlbumCovers(connectionId, [5]);

        _factory.ImageServiceMock.Verify(
            s => s.GetUrlAsync(5, EntityType.AlbumCover),
            Times.Once);
        Assert.Single(_factory.SendRecorder.FindSends("ReceivedAlbumCover"));
    }
}
