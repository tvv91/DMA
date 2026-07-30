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
    public async Task GetPosts_AdminDefaultView_RequestsAllPosts()
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
            .Setup(s => s.GetFilteredListAsync(2, 5, "query", "News", "2024", false, false))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub(asAdmin: true);
        await hub.GetPosts(ConnectionId, 2, "query", "News", "2024", onlyDrafts: false);

        _factory.PostServiceMock.Verify(
            s => s.GetFilteredListAsync(2, 5, "query", "News", "2024", false, false),
            Times.Once);
    }

    [Fact]
    public async Task GetPosts_NonAdmin_ExcludesDrafts()
    {
        var post = new Post
        {
            Id = 3,
            Title = "Published",
            Description = "Desc",
            IsDraft = false,
            CreatedDate = new DateTime(2024, 1, 1),
        };
        var paged = new PagedResult<Post>([post], totalItems: 1, currentPage: 1, pageSize: 5);

        _factory.PostServiceMock
            .Setup(s => s.GetFilteredListAsync(1, 5, "", "", "", false, true))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetPosts(ConnectionId, 1, "", "", "", onlyDrafts: false);

        _factory.PostServiceMock.Verify(
            s => s.GetFilteredListAsync(1, 5, "", "", "", false, true),
            Times.Once);
    }

    [Fact]
    public async Task GetPosts_NonAdmin_IgnoresOnlyDraftsFilter()
    {
        var paged = new PagedResult<Post>([], totalItems: 0, currentPage: 1, pageSize: 5);
        _factory.PostServiceMock
            .Setup(s => s.GetFilteredListAsync(1, 5, "", "", "", false, true))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetPosts(ConnectionId, 1, "", "", "", onlyDrafts: true);

        _factory.PostServiceMock.Verify(
            s => s.GetFilteredListAsync(1, 5, "", "", "", false, true),
            Times.Once);
    }

    [Fact]
    public async Task GetPosts_AdminOnlyDrafts_RequestsDraftPostsOnly()
    {
        var paged = new PagedResult<Post>([], totalItems: 0, currentPage: 1, pageSize: 5);
        _factory.PostServiceMock
            .Setup(s => s.GetFilteredListAsync(1, 5, "", "", "", true, false))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub(asAdmin: true);
        await hub.GetPosts(ConnectionId, 1, "", "", "", onlyDrafts: true);

        _factory.PostServiceMock.Verify(
            s => s.GetFilteredListAsync(1, 5, "", "", "", true, false),
            Times.Once);
    }
}
