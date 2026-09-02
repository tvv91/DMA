using Moq;
using Web.Request;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubAddReleaseTests
{
    private readonly AlbumHubTestFactory _factory = new();
    private const string ConnectionId = "conn-add";

    [Fact]
    public async Task AddRelease_NewAlbum_CreatesAlbumAndAddsRelease()
    {
        var request = new CreateUpdateReleaseRequest
        {
            AlbumId = 0,
            Album = "New Album",
            Artist = "Artist",
            Genre = "Rock",
            Source = "Vinyl",
        };
        var album = new Album { Id = 10, Title = request.Album };
        var release = new Release { Id = 20, AlbumId = 10, Source = "Vinyl" };

        _factory.AlbumServiceMock
            .Setup(s => s.CreateOrFindAlbumAsync(request.Album, request.Artist, request.Genre))
            .ReturnsAsync(album);
        _factory.ReleaseServiceMock
            .Setup(s => s.AddAsync(It.IsAny<Release>()))
            .ReturnsAsync(release);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByAlbumIdAsync(10))
            .ReturnsAsync([release]);

        var hub = _factory.CreateHub();
        await hub.AddRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseAdded");
        Assert.NotNull(send);
        Assert.True((bool)send.Value.Args[0]!);
        Assert.Equal(string.Empty, send.Value.Args[1]);
        Assert.Equal(10, send.Value.Args[2]);
        Assert.IsType<List<object>>(send.Value.Args[3]);
    }

    [Fact]
    public async Task AddRelease_ExistingAlbumId_UsesExistingAlbum()
    {
        var request = new CreateUpdateReleaseRequest
        {
            AlbumId = 3,
            Source = "CD",
        };
        var album = new Album { Id = 3, Title = "Existing" };
        var release = new Release { Id = 8, AlbumId = 3, Source = "CD" };

        _factory.AlbumServiceMock
            .Setup(s => s.GetByIdAsync(3))
            .ReturnsAsync(album);
        _factory.ReleaseServiceMock
            .Setup(s => s.AddAsync(It.IsAny<Release>()))
            .ReturnsAsync(release);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByAlbumIdAsync(3))
            .ReturnsAsync([release]);

        var hub = _factory.CreateHub();
        await hub.AddRelease(ConnectionId, request);

        _factory.AlbumServiceMock.Verify(
            s => s.CreateOrFindAlbumAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        var send = _factory.SendRecorder.FindSend("ReleaseAdded");
        Assert.NotNull(send);
        Assert.True((bool)send.Value.Args[0]!);
    }

    [Fact]
    public async Task AddRelease_AlbumNotFound_SendsFailure()
    {
        var request = new CreateUpdateReleaseRequest { AlbumId = 99, Source = "Vinyl" };
        _factory.AlbumServiceMock
            .Setup(s => s.GetByIdAsync(99))
            .ReturnsAsync((Album?)null);

        var hub = _factory.CreateHub();
        await hub.AddRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseAdded");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("Album not found", send.Value.Args[1]);
        Assert.Equal(0, send.Value.Args[2]);
    }

    [Fact]
    public async Task AddRelease_ServiceThrows_SendsErrorMessage()
    {
        var request = new CreateUpdateReleaseRequest
        {
            AlbumId = 0,
            Album = "A",
            Artist = "B",
            Genre = "Rock",
        };
        _factory.AlbumServiceMock
            .Setup(s => s.CreateOrFindAlbumAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var hub = _factory.CreateHub();
        await hub.AddRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseAdded");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("db error", send.Value.Args[1]);
    }
}
