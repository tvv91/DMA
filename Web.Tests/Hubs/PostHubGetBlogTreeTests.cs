using Moq;
using Web.Common;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

public class PostHubGetBlogTreeTests
{
    private readonly PostHubTestFactory _factory = new();
    private const string ConnectionId = "conn-tree";

    [Fact]
    public async Task GetBlogTree_ExcludesDraftsAndPostsWithoutCreatedDate()
    {
        var published = CreatePost(1, "Published", new DateTime(2024, 5, 10), "News");
        var draft = new Post
        {
            Id = 2,
            Title = "Draft",
            CreatedDate = new DateTime(2024, 5, 11),
            IsDraft = true,
        };
        var withoutDate = new Post
        {
            Id = 3,
            Title = "No date",
            IsDraft = false,
            CreatedDate = null,
        };

        var paged = new PagedResult<Post>([published, draft, withoutDate], totalItems: 3, currentPage: 1, pageSize: int.MaxValue);
        _factory.PostServiceMock
            .Setup(s => s.GetListAsync(1, int.MaxValue))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetBlogTree(ConnectionId);

        var send = _factory.SendRecorder.FindSend("ReceivedBlogTree");
        Assert.NotNull(send);

        var tree = Assert.IsAssignableFrom<IEnumerable<object>>(send.Value.Args[0]).ToList();
        Assert.Single(tree);

        var newsGroup = tree[0];
        Assert.Equal("News", PostHubTestFactory.GetProperty<string>(newsGroup, "Category"));

        var yearGroups = PostHubTestFactory.GetProperty<IEnumerable<object>>(newsGroup, "Posts").ToList();
        Assert.Single(yearGroups);
        Assert.Equal(2024, PostHubTestFactory.GetProperty<int>(yearGroups[0], "Year"));

        var posts = PostHubTestFactory.GetProperty<IEnumerable<object>>(yearGroups[0], "Posts").ToList();
        Assert.Single(posts);
        Assert.Equal(1, PostHubTestFactory.GetProperty<int>(posts[0], "Id"));
    }

    [Fact]
    public async Task GetBlogTree_GroupsUncategorizedPosts()
    {
        var uncategorized = new Post
        {
            Id = 5,
            Title = "Solo",
            CreatedDate = new DateTime(2023, 1, 1),
            IsDraft = false,
        };
        var paged = new PagedResult<Post>([uncategorized], totalItems: 1, currentPage: 1, pageSize: int.MaxValue);

        _factory.PostServiceMock
            .Setup(s => s.GetListAsync(1, int.MaxValue))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetBlogTree(ConnectionId);

        var send = _factory.SendRecorder.FindSend("ReceivedBlogTree");
        Assert.NotNull(send);

        var tree = Assert.IsAssignableFrom<IEnumerable<object>>(send.Value.Args[0]).ToList();
        Assert.Single(tree);
        Assert.Equal("Uncategorized", PostHubTestFactory.GetProperty<string>(tree[0], "Category"));
    }

    [Fact]
    public async Task GetBlogTree_OrdersCategoriesAlphabetically()
    {
        var zebra = CreatePost(1, "Z post", new DateTime(2024, 1, 1), "Zebra");
        var alpha = CreatePost(2, "A post", new DateTime(2024, 2, 1), "Alpha");
        var paged = new PagedResult<Post>([zebra, alpha], totalItems: 2, currentPage: 1, pageSize: int.MaxValue);

        _factory.PostServiceMock
            .Setup(s => s.GetListAsync(1, int.MaxValue))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetBlogTree(ConnectionId);

        var send = _factory.SendRecorder.FindSend("ReceivedBlogTree");
        Assert.NotNull(send);

        var tree = Assert.IsAssignableFrom<IEnumerable<object>>(send.Value.Args[0]).ToList();
        Assert.Equal(2, tree.Count);
        Assert.Equal("Alpha", PostHubTestFactory.GetProperty<string>(tree[0], "Category"));
        Assert.Equal("Zebra", PostHubTestFactory.GetProperty<string>(tree[1], "Category"));
    }

    private static Post CreatePost(int id, string title, DateTime createdDate, string categoryTitle)
    {
        var post = new Post
        {
            Id = id,
            Title = title,
            CreatedDate = createdDate,
            IsDraft = false,
        };
        post.PostCategories.Add(new PostCategory
        {
            Post = post,
            Category = new Category { Title = categoryTitle },
        });
        return post;
    }
}
