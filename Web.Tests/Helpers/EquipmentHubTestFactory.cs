using Microsoft.AspNetCore.SignalR;
using Moq;
using Web.Interfaces;
using Web.SignalRHubs;

namespace Web.Tests.Helpers;

internal sealed class EquipmentHubTestFactory
{
    public Mock<IImageService> ImageServiceMock { get; } = new();
    public Mock<IEquipmentService> EquipmentServiceMock { get; } = new();
    public HubSendRecorder SendRecorder { get; } = new();

    public EquipmentHub CreateHub()
    {
        var hub = new EquipmentHub(ImageServiceMock.Object, EquipmentServiceMock.Object);

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
}
