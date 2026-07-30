using Moq;
using Web.Common;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

public class PostHubGetPostsTests
{
    private readonly PostHubTestFactory _factory = new();
    private const string ConnectionId = "conn-posts";

    [Fact]
    public async Task GetPosts_SendsMappedPostsAndTotalPages()
    {
        var category = new Category { Id = 1, Title = "News" };
        var post = new Post
        {
            Id = 10,
            Title = "Hello",
            Description = "Desc",
            Content = "Body",
            IsDraft = false,
            CreatedDate = new DateTime(2024, 3, 15),
        };
        post.PostCategories.Add(new PostCategory { Category = category, Post = post });

        var paged = new PagedResult<Post>([post], totalItems: 6, currentPage: 2, pageSize: 5);
        _factory.PostServiceMock
            .Setup(s => s.GetFilteredListAsync(2, 5, "query", "News", "2024", true))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetPosts(ConnectionId, 2, "query", "News", "2024", onlyDrafts: true);

        var send = _factory.SendRecorder.FindSend("ReceivedPosts");
        Assert.NotNull(send);
        Assert.Equal(2, send.Value.Args[1]);

        var items = Assert.IsAssignableFrom<IEnumerable<object>>(send.Value.Args[0]).ToList();
        Assert.Single(items);

        var item = items[0];
        Assert.Equal(10, PostHubTestFactory.GetProperty<int>(item, "Id"));
        Assert.Equal("Hello", PostHubTestFactory.GetProperty<string>(item, "Title"));
        Assert.Equal("Desc", PostHubTestFactory.GetProperty<string>(item, "Description"));
        Assert.False(PostHubTestFactory.GetProperty<bool>(item, "IsDraft"));
        Assert.Equal(new DateTime(2024, 3, 15).ToShortDateString(), PostHubTestFactory.GetProperty<string?>(item, "Created"));

        var categories = PostHubTestFactory.GetProperty<List<string>>(item, "Categories");
        Assert.Equal(["News"], categories);
    }

    [Fact]
    public async Task GetPosts_NullCreatedDate_SendsNullCreated()
    {
        var post = new Post
        {
            Id = 3,
            Title = "Draft",
            Description = "Desc",
            IsDraft = true,
            CreatedDate = null,
        };
        var paged = new PagedResult<Post>([post], totalItems: 1, currentPage: 1, pageSize: 5);

        _factory.PostServiceMock
            .Setup(s => s.GetFilteredListAsync(1, 5, "", "", "", false))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetPosts(ConnectionId, 1, "", "", "", onlyDrafts: false);

        var send = _factory.SendRecorder.FindSend("ReceivedPosts");
        Assert.NotNull(send);
        var item = Assert.IsAssignableFrom<IEnumerable<object>>(send.Value.Args[0]).Single();
        Assert.Null(PostHubTestFactory.GetProperty<string?>(item, "Created"));
    }
}
