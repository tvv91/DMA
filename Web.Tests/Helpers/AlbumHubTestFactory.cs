using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Web.Interfaces;
using Web.SignalRHubs;

namespace Web.Tests.Helpers;

internal sealed class AlbumHubTestFactory
{
    private static readonly DateTimeOffset FixedUtcNow = new(2024, 9, 10, 12, 0, 0, TimeSpan.Zero);

    public Mock<IImageService> ImageServiceMock { get; } = new();
    public Mock<IResourceIconService> ResourceIconServiceMock { get; } = new();
    public Mock<IAlbumService> AlbumServiceMock { get; } = new();
    public Mock<IReleaseService> ReleaseServiceMock { get; } = new();
    public Mock<IEquipmentService> EquipmentServiceMock { get; } = new();
    public Mock<IEntityFindOrCreateService> EntityServiceMock { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(FixedUtcNow);
    public HubSendRecorder SendRecorder { get; } = new();

    public AlbumHubTestFactory()
    {
        ResetCoverCache();
    }

    public AlbumHub CreateHub()
    {
        var hub = new AlbumHub(
            ImageServiceMock.Object,
            ResourceIconServiceMock.Object,
            AlbumServiceMock.Object,
            ReleaseServiceMock.Object,
            EquipmentServiceMock.Object,
            EntityServiceMock.Object,
            TimeProvider);

        var mockClients = new Mock<IHubCallerClients>();
        var mockClient = new Mock<ISingleClientProxy>();
        mockClients.Setup(c => c.Client(It.IsAny<string>())).Returns(mockClient.Object);
        mockClient
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, object?[], CancellationToken>(
                (method, args, token) => SendRecorder.RecordSendAsync(method, args, token));

        hub.Clients = mockClients.Object;
        return hub;
    }

    public static void ResetCoverCache()
    {
        var field = typeof(AlbumHub).GetField(
            "_coverCache",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (field?.GetValue(null) is ConcurrentDictionary<int, string> cache)
            cache.Clear();
    }
}
