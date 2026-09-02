using Moq;
using Web.SignalRHubs;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubInvalidateAlbumCacheTests
{
    [Fact]
    public async Task InvalidateAlbumCache_ForcesImageServiceReload()
    {
        var factory = new AlbumHubTestFactory();
        factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(9, EntityType.AlbumCover))
            .ReturnsAsync("/covers/album/9.jpg");

        var hub = factory.CreateHub();
        await hub.GetAlbumCovers("conn", [9]);

        AlbumHub.InvalidateAlbumCache(9);
        factory.SendRecorder.Sends.Clear();

        await hub.GetAlbumCovers("conn", [9]);

        factory.ImageServiceMock.Verify(
            s => s.GetUrlAsync(9, EntityType.AlbumCover),
            Times.Exactly(2));
    }
}
