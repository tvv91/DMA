using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Web.Common;
using Web.Interfaces;
using Web.SignalRHubs;

namespace Web.Tests.Helpers;

internal sealed class PostHubTestFactory
{
    private static readonly DateTimeOffset FixedUtcNow = new(2024, 8, 20, 14, 30, 0, TimeSpan.Zero);

    public Mock<IPostService> PostServiceMock { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(FixedUtcNow);
    public HubSendRecorder SendRecorder { get; } = new();

    public DateTime FixedUtcDateTime => FixedUtcNow.UtcDateTime;

    public PostHub CreateHub(bool asAdmin = false)
    {
        var hub = new PostHub(PostServiceMock.Object, TimeProvider);

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

        var claims = asAdmin
            ? new[] { new Claim(ClaimTypes.Role, RoleNames.Admin) }
            : Array.Empty<Claim>();
        var identity = new ClaimsIdentity(claims, asAdmin ? "TestAuth" : null);
        var contextMock = new Mock<HubCallerContext>();
        contextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(identity));
        hub.Context = contextMock.Object;

        return hub;
    }

    internal static T GetProperty<T>(object target, string name) =>
        (T)target.GetType().GetProperty(name)!.GetValue(target)!;
}
