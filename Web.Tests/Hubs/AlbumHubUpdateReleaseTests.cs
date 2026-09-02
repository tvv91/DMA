using Moq;
using Web.Request;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubUpdateReleaseTests
{
    private readonly AlbumHubTestFactory _factory = new();
    private const string ConnectionId = "conn-update";

    [Fact]
    public async Task UpdateRelease_MissingReleaseId_SendsFailure()
    {
        var request = new CreateUpdateReleaseRequest { ReleaseId = 0 };

        var hub = _factory.CreateHub();
        await hub.UpdateRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseUpdated");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("Release ID is required", send.Value.Args[1]);
    }

    [Fact]
    public async Task UpdateRelease_ReleaseNotFound_SendsFailure()
    {
        var request = new CreateUpdateReleaseRequest { ReleaseId = 5 };
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(5))
            .ReturnsAsync((Release?)null);

        var hub = _factory.CreateHub();
        await hub.UpdateRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseUpdated");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("Release not found", send.Value.Args[1]);
    }

    [Fact]
    public async Task UpdateRelease_ValidRequest_SendsUpdatedReleaseList()
    {
        var request = new CreateUpdateReleaseRequest { ReleaseId = 7, Source = "Updated" };
        var existing = new Release { Id = 7, AlbumId = 2, Source = "Old" };
        var updated = new Release { Id = 7, AlbumId = 2, Source = "Updated" };

        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(7))
            .ReturnsAsync(existing);
        _factory.ReleaseServiceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Release>()))
            .ReturnsAsync(updated);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByAlbumIdAsync(2))
            .ReturnsAsync([updated]);

        var hub = _factory.CreateHub();
        await hub.UpdateRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseUpdated");
        Assert.NotNull(send);
        Assert.True((bool)send.Value.Args[0]!);
        Assert.Equal(string.Empty, send.Value.Args[1]);
        Assert.IsType<List<object>>(send.Value.Args[2]);
    }

    [Fact]
    public async Task UpdateRelease_ServiceThrows_SendsErrorMessage()
    {
        var request = new CreateUpdateReleaseRequest { ReleaseId = 7 };
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(7))
            .ThrowsAsync(new InvalidOperationException("update failed"));

        var hub = _factory.CreateHub();
        await hub.UpdateRelease(ConnectionId, request);

        var send = _factory.SendRecorder.FindSend("ReleaseUpdated");
        Assert.NotNull(send);
        Assert.False((bool)send.Value.Args[0]!);
        Assert.Equal("update failed", send.Value.Args[1]);
    }
}
