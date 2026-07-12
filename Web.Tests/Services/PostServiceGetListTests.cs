using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceGetListTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetListAsync_EmptyDatabase_ReturnsEmptyItems()
    {
        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10);

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetListAsync_ReturnsPaginatedResults()
    {
        for (var i = 0; i < 5; i++)
            await _factory.SeedPostAsync(title: $"Post {i + 1}");

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 2);

        Assert.Equal(5, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetListAsync_SecondPage_ReturnsRemainingItems()
    {
        for (var i = 0; i < 5; i++)
            await _factory.SeedPostAsync(title: $"Post {i + 1}");

        var result = await _factory.Service.GetListAsync(page: 2, pageSize: 2);

        Assert.Equal(5, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetListAsync_OrdersByCreatedDateDescendingThenId()
    {
        await _factory.SeedPostAsync(title: "Older", createdDate: new DateTime(2020, 1, 1));
        await _factory.SeedPostAsync(title: "Newer", createdDate: new DateTime(2024, 1, 1));

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10);

        Assert.Equal("Newer", result.Items[0].Title);
        Assert.Equal("Older", result.Items[1].Title);
    }

    [Fact]
    public async Task GetListAsync_NullCreatedDate_SortedAfterDatedPosts()
    {
        await _factory.SeedPostAsync(title: "With Date", createdDate: new DateTime(2024, 1, 1));
        await _factory.SeedPostAsync(title: "No Date", clearCreatedDate: true);

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10);

        Assert.Equal("With Date", result.Items[0].Title);
        Assert.Equal("No Date", result.Items[1].Title);
    }

    [Fact]
    public async Task GetListAsync_IncludesCategories()
    {
        await _factory.SeedPostAsync(title: "Categorized", categoryTitle: "Reviews");

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10);

        var post = result.Items[0];
        Assert.Single(post.PostCategories);
        Assert.Equal("Reviews", post.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task GetListAsync_IncludesDraftAndPublishedPosts()
    {
        await _factory.SeedPostAsync(title: "Published", isDraft: false);
        await _factory.SeedPostAsync(title: "Draft", isDraft: true);

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10);

        Assert.Equal(2, result.TotalItems);
    }
}
