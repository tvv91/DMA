using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubRemoveReleaseTests
{
    private readonly AlbumHubTestFactory _factory = new();
    private const string ConnectionId = "conn-remove";

    [Fact]
    public async Task RemoveRelease_ReleaseNotFound_SendsFailure()
    {
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(4))
            .ReturnsAsync((Release?)null);

        var hub = _factory.CreateHub();
        await hub.RemoveRelease(ConnectionId, 4);

        var send = _factory.SendRecorder.FindSend("ReleaseRemoved");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("Release not found", send.Value.Args[1]);
    }

    [Fact]
    public async Task RemoveRelease_Success_SendsRemainingReleases()
    {
        var release = new Release { Id = 4, AlbumId = 2, Source = "Vinyl" };
        var remaining = new Release { Id = 5, AlbumId = 2, Source = "CD" };

        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(4))
            .ReturnsAsync(release);
        _factory.ReleaseServiceMock
            .Setup(s => s.DeleteAsync(4))
            .ReturnsAsync(true);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByAlbumIdAsync(2))
            .ReturnsAsync([remaining]);

        var hub = _factory.CreateHub();
        await hub.RemoveRelease(ConnectionId, 4);

        var send = _factory.SendRecorder.FindSend("ReleaseRemoved");
        Assert.NotNull(send);
        Assert.True((bool)send.Value.Args[0]!);
        Assert.Equal(string.Empty, send.Value.Args[1]);
        Assert.IsType<List<object>>(send.Value.Args[2]);
    }

    [Fact]
    public async Task RemoveRelease_DeleteFails_SendsFailure()
    {
        var release = new Release { Id = 4, AlbumId = 2 };

        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(4))
            .ReturnsAsync(release);
        _factory.ReleaseServiceMock
            .Setup(s => s.DeleteAsync(4))
            .ReturnsAsync(false);

        var hub = _factory.CreateHub();
        await hub.RemoveRelease(ConnectionId, 4);

        var send = _factory.SendRecorder.FindSend("ReleaseRemoved");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("Failed to remove release", send.Value.Args[1]);
    }

    [Fact]
    public async Task RemoveRelease_ServiceThrows_SendsErrorMessage()
    {
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(4))
            .ThrowsAsync(new InvalidOperationException("delete error"));

        var hub = _factory.CreateHub();
        await hub.RemoveRelease(ConnectionId, 4);

        var send = _factory.SendRecorder.FindSend("ReleaseRemoved");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("delete error", send.Value.Args[1]);
    }
}
