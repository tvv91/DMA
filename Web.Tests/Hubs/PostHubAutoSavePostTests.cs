using Moq;
using Web.Models;
using Web.Tests.Helpers;
using Web.ViewModels;

namespace Web.Tests.Hubs;

public class PostHubAutoSavePostTests
{
    private readonly PostHubTestFactory _factory = new();
    private const string ConnectionId = "conn-autosave";

    [Fact]
    public async Task AutoSavePost_NewPost_CreatesDraftAndSendsPostCreated()
    {
        PostViewModel? captured = null;
        _factory.PostServiceMock
            .Setup(s => s.CreateDraftPostAsync(It.IsAny<PostViewModel>()))
            .Callback<PostViewModel>(m => captured = m)
            .ReturnsAsync(new Post { Id = 42 });

        var hub = _factory.CreateHub();
        await hub.AutoSavePost(ConnectionId, 0, "Title", "Desc", "Content", "News");

        Assert.NotNull(captured);
        Assert.Equal("Title", captured.Title);
        Assert.Equal("Desc", captured.Description);
        Assert.Equal("Content", captured.Content);
        Assert.Equal("News", captured.Category);

        var send = _factory.SendRecorder.FindSend("PostCreated");
        Assert.NotNull(send);
        Assert.Equal(42, send.Value.Args[0]);
        Assert.Equal(_factory.FixedUtcDateTime, send.Value.Args[1]);
    }

    [Fact]
    public async Task AutoSavePost_ExistingPost_UpdatesAndSendsPostUpdated()
    {
        PostViewModel? captured = null;
        _factory.PostServiceMock
            .Setup(s => s.UpdatePostAsync(7, It.IsAny<PostViewModel>()))
            .Callback<int, PostViewModel>((_, m) => captured = m)
            .ReturnsAsync(new Post { Id = 7 });

        var hub = _factory.CreateHub();
        await hub.AutoSavePost(ConnectionId, 7, "Updated", "New desc", "New content", "Reviews");

        Assert.NotNull(captured);
        Assert.Equal("Updated", captured.Title);
        Assert.Equal("Reviews", captured.Category);

        var send = _factory.SendRecorder.FindSend("PostUpdated");
        Assert.NotNull(send);
        Assert.Equal(_factory.FixedUtcDateTime, send.Value.Args[0]);
        _factory.PostServiceMock.Verify(s => s.CreateDraftPostAsync(It.IsAny<PostViewModel>()), Times.Never);
    }

    [Fact]
    public async Task AutoSavePost_PostNotFound_DoesNotSendAnything()
    {
        _factory.PostServiceMock
            .Setup(s => s.UpdatePostAsync(99, It.IsAny<PostViewModel>()))
            .ThrowsAsync(new KeyNotFoundException());

        var hub = _factory.CreateHub();
        await hub.AutoSavePost(ConnectionId, 99, "Title", "Desc", "Content", "News");

        Assert.Empty(_factory.SendRecorder.Sends);
    }

    [Fact]
    public async Task AutoSavePost_ServiceThrows_DoesNotSendAnything()
    {
        _factory.PostServiceMock
            .Setup(s => s.CreateDraftPostAsync(It.IsAny<PostViewModel>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var hub = _factory.CreateHub();
        await hub.AutoSavePost(ConnectionId, 0, "Title", "Desc", "Content", "News");

        Assert.Empty(_factory.SendRecorder.Sends);
    }
}
