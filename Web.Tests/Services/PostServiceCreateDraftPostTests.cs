using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceCreateDraftPostTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task CreateDraftPostAsync_ValidModel_CreatesDraftPost()
    {
        var model = PostServiceTestFactory.CreateViewModel(title: "Draft Post");

        var result = await _factory.Service.CreateDraftPostAsync(model);

        Assert.True(result.IsDraft);
        Assert.Equal(_factory.FixedUtcDateTime, result.CreatedDate);
    }

    [Fact]
    public async Task CreateDraftPostAsync_WithCategory_LinksCategory()
    {
        var model = PostServiceTestFactory.CreateViewModel(category: "Drafts");

        var result = await _factory.Service.CreateDraftPostAsync(model);

        Assert.Equal("Drafts", result.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task CreateDraftPostAsync_PlaceholderCategory_SkipsCategoryLink()
    {
        var model = PostServiceTestFactory.CreateViewModel(category: "Category");

        var result = await _factory.Service.CreateDraftPostAsync(model);

        Assert.Empty(result.PostCategories);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateDraftPostAsync_WithoutCategory_CreatesPostWithoutCategory(string? category)
    {
        var model = PostServiceTestFactory.CreateViewModel();
        model.Category = category!;

        var result = await _factory.Service.CreateDraftPostAsync(model);

        Assert.Empty(result.PostCategories);
    }
}
