using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceGetFilteredListTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetFilteredListAsync_NullOrWhitespaceSearchText_IgnoresSearch(string? searchText)
    {
        await _factory.SeedPostAsync(title: "Unique Alpha Title");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, searchText, null, null, false);

        Assert.Equal(1, result.TotalItems);
    }

    [Fact]
    public async Task GetFilteredListAsync_SearchByTitle_ReturnsMatchingPost()
    {
        await _factory.SeedPostAsync(title: "Vinyl Review", content: "Other");
        await _factory.SeedPostAsync(title: "Other", content: "Unrelated");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, "Vinyl", null, null, false);

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Vinyl Review", result.Items[0].Title);
    }

    [Fact]
    public async Task GetFilteredListAsync_SearchByDescription_ReturnsMatchingPost()
    {
        await _factory.SeedPostAsync(title: "Title", description: "Special keyword here");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, "keyword", null, null, false);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetFilteredListAsync_SearchByContent_ReturnsMatchingPost()
    {
        await _factory.SeedPostAsync(title: "Title", content: "Deep content match");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, "content match", null, null, false);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetFilteredListAsync_CategoryFilter_ReturnsExactMatch()
    {
        await _factory.SeedPostAsync(title: "In Category", categoryTitle: "Hardware");
        await _factory.SeedPostAsync(title: "Other", categoryTitle: "Software");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, "Hardware", null, false);

        Assert.Single(result.Items);
        Assert.Equal("In Category", result.Items[0].Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetFilteredListAsync_NullOrEmptyCategory_IgnoresCategoryFilter(string? category)
    {
        await _factory.SeedPostAsync(title: "Post One", categoryTitle: "News");
        await _factory.SeedPostAsync(title: "Post Two", categoryTitle: "Reviews");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, category, null, false);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetFilteredListAsync_ValidYearFilter_ReturnsMatchingPosts()
    {
        await _factory.SeedPostAsync(title: "2023 Post", createdDate: new DateTime(2023, 6, 1));
        await _factory.SeedPostAsync(title: "2024 Post", createdDate: new DateTime(2024, 6, 1));

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, null, "2023", false);

        Assert.Single(result.Items);
        Assert.Equal("2023 Post", result.Items[0].Title);
    }

    [Fact]
    public async Task GetFilteredListAsync_NonParseableYear_IgnoresYearFilter()
    {
        await _factory.SeedPostAsync(title: "2023 Post", createdDate: new DateTime(2023, 6, 1));
        await _factory.SeedPostAsync(title: "2024 Post", createdDate: new DateTime(2024, 6, 1));

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, null, "not-a-year", false);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetFilteredListAsync_PostWithoutCreatedDate_ExcludedByYearFilter()
    {
        await _factory.SeedPostAsync(title: "Undated", clearCreatedDate: true);
        await _factory.SeedPostAsync(title: "2024 Post", createdDate: new DateTime(2024, 1, 1));

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, null, "2024", false);

        Assert.Single(result.Items);
        Assert.Equal("2024 Post", result.Items[0].Title);
    }

    [Fact]
    public async Task GetFilteredListAsync_ExcludeDrafts_ExcludesDraftPosts()
    {
        await _factory.SeedPostAsync(title: "Published", isDraft: false);
        await _factory.SeedPostAsync(title: "Draft", isDraft: true);

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, null, null, onlyDrafts: false, excludeDrafts: true);

        Assert.Single(result.Items);
        Assert.Equal("Published", result.Items[0].Title);
        Assert.False(result.Items[0].IsDraft);
    }

    [Fact]
    public async Task GetFilteredListAsync_AdminDefaultView_IncludesDraftAndPublished()
    {
        await _factory.SeedPostAsync(title: "Published", isDraft: false);
        await _factory.SeedPostAsync(title: "Draft", isDraft: true);

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, null, null, onlyDrafts: false, excludeDrafts: false);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetFilteredListAsync_OnlyDrafts_ReturnsDraftPosts()
    {
        await _factory.SeedPostAsync(title: "Published", isDraft: false);
        await _factory.SeedPostAsync(title: "Draft", isDraft: true);

        var result = await _factory.Service.GetFilteredListAsync(1, 10, null, null, null, onlyDrafts: true);

        Assert.Single(result.Items);
        Assert.True(result.Items[0].IsDraft);
    }

    [Fact]
    public async Task GetFilteredListAsync_CombinedFilters_ReturnsMatchingPost()
    {
        await _factory.SeedPostAsync(
            title: "Vinyl Guide",
            description: "Hardware tips",
            categoryTitle: "Reviews",
            isDraft: true,
            createdDate: new DateTime(2024, 3, 1));
        await _factory.SeedPostAsync(title: "Other", categoryTitle: "News", isDraft: false);

        var result = await _factory.Service.GetFilteredListAsync(1, 10, "Vinyl", "Reviews", "2024", onlyDrafts: true);

        Assert.Single(result.Items);
        Assert.Equal("Vinyl Guide", result.Items[0].Title);
    }

    [Fact]
    public async Task GetFilteredListAsync_NoMatches_ReturnsEmptyItems()
    {
        await _factory.SeedPostAsync(title: "Existing");

        var result = await _factory.Service.GetFilteredListAsync(1, 10, "missing", null, null, false);

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }
}
