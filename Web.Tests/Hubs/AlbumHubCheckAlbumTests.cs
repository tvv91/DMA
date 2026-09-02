using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubCheckAlbumTests
{
    private readonly AlbumHubTestFactory _factory = new();
    private const string ConnectionId = "conn-check";

    [Fact]
    public async Task CheckAlbum_AlbumNotFound_SendsZeroStatus()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.FindByAlbumAndArtistAsync("Unknown", "Artist"))
            .ReturnsAsync((Album?)null);

        var hub = _factory.CreateHub();
        await hub.CheckAlbum(ConnectionId, 1, "Unknown", "Artist", "Vinyl");

        var send = _factory.SendRecorder.FindSend("AlbumIsExist");
        Assert.NotNull(send);
        Assert.Equal(0, send.Value.Args[0]);
        Assert.Equal(0, send.Value.Args[1]);
    }

    [Fact]
    public async Task CheckAlbum_DifferentExistingAlbum_SendsDuplicateStatus()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.FindByAlbumAndArtistAsync("Dark Side", "Pink Floyd"))
            .ReturnsAsync(new Album { Id = 42, Title = "Dark Side" });

        var hub = _factory.CreateHub();
        await hub.CheckAlbum(ConnectionId, 1, "Dark Side", "Pink Floyd", "Vinyl");

        var send = _factory.SendRecorder.FindSend("AlbumIsExist");
        Assert.NotNull(send);
        Assert.Equal(1, send.Value.Args[0]);
        Assert.Equal(42, send.Value.Args[1]);
    }

    [Fact]
    public async Task CheckAlbum_SameAlbumWithExistingSource_SendsSourceExistsStatus()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.FindByAlbumAndArtistAsync("Album", "Artist"))
            .ReturnsAsync(new Album { Id = 5, Title = "Album" });
        _factory.ReleaseServiceMock
            .Setup(s => s.ExistsByAlbumIdAndSourceAsync(5, "CD"))
            .ReturnsAsync(true);

        var hub = _factory.CreateHub();
        await hub.CheckAlbum(ConnectionId, 5, "Album", "Artist", "CD");

        var send = _factory.SendRecorder.FindSend("AlbumIsExist");
        Assert.NotNull(send);
        Assert.Equal(100, send.Value.Args[0]);
        Assert.Equal(5, send.Value.Args[1]);
    }

    [Fact]
    public async Task CheckAlbum_SameAlbumWithoutExistingSource_SendsZeroStatus()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.FindByAlbumAndArtistAsync("Album", "Artist"))
            .ReturnsAsync(new Album { Id = 5, Title = "Album" });
        _factory.ReleaseServiceMock
            .Setup(s => s.ExistsByAlbumIdAndSourceAsync(5, "Vinyl"))
            .ReturnsAsync(false);

        var hub = _factory.CreateHub();
        await hub.CheckAlbum(ConnectionId, 5, "Album", "Artist", "Vinyl");

        var send = _factory.SendRecorder.FindSend("AlbumIsExist");
        Assert.NotNull(send);
        Assert.Equal(0, send.Value.Args[0]);
        Assert.Equal(0, send.Value.Args[1]);
    }

    [Fact]
    public async Task CheckAlbum_SameAlbumWithBlankSource_SkipsSourceCheck()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.FindByAlbumAndArtistAsync("Album", "Artist"))
            .ReturnsAsync(new Album { Id = 5, Title = "Album" });

        var hub = _factory.CreateHub();
        await hub.CheckAlbum(ConnectionId, 5, "Album", "Artist", "   ");

        var send = _factory.SendRecorder.FindSend("AlbumIsExist");
        Assert.NotNull(send);
        Assert.Equal(0, send.Value.Args[0]);
        _factory.ReleaseServiceMock.Verify(
            s => s.ExistsByAlbumIdAndSourceAsync(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }
}
