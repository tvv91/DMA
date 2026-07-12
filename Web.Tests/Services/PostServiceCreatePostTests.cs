using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceCreatePostTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task CreatePostAsync_ValidModel_CreatesPublishedPost()
    {
        var model = PostServiceTestFactory.CreateViewModel(title: "Published Post");

        var result = await _factory.Service.CreatePostAsync(model);

        Assert.True(result.Id > 0);
        Assert.False(result.IsDraft);
        Assert.Equal(_factory.FixedUtcDateTime, result.CreatedDate);
        Assert.Equal("Published Post", result.Title);
    }

    [Fact]
    public async Task CreatePostAsync_WithCategory_LinksCategory()
    {
        var model = PostServiceTestFactory.CreateViewModel(category: "Reviews");

        var result = await _factory.Service.CreatePostAsync(model);

        Assert.Single(result.PostCategories);
        Assert.Equal("Reviews", result.PostCategories.First().Category.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreatePostAsync_WithoutCategory_CreatesPostWithoutCategory(string? category)
    {
        var model = PostServiceTestFactory.CreateViewModel();
        model.Category = category!;

        var result = await _factory.Service.CreatePostAsync(model);

        Assert.Empty(result.PostCategories);
    }

    [Fact]
    public async Task CreatePostAsync_PlaceholderCategory_StillCreatesCategoryLink()
    {
        var model = PostServiceTestFactory.CreateViewModel(category: "Category");

        var result = await _factory.Service.CreatePostAsync(model);

        Assert.Single(result.PostCategories);
        Assert.Equal("Category", result.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task CreatePostAsync_ExistingCategory_ReusesCategory()
    {
        await _factory.FindOrAddCategoryAsync("Shared");

        var result = await _factory.Service.CreatePostAsync(PostServiceTestFactory.CreateViewModel(category: "Shared"));

        Assert.Single(_factory.Context.Categories.Where(c => c.Title == "Shared"));
        Assert.Equal("Shared", result.PostCategories.First().Category.Title);
    }
}
