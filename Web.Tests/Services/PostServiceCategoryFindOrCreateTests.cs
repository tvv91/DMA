using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceCategoryFindOrCreateTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task CreatePostAsync_TrimmedCategoryLookup_FindsExistingCategory()
    {
        await _factory.FindOrAddCategoryAsync("Reviews");

        var result = await _factory.Service.CreatePostAsync(PostServiceTestFactory.CreateViewModel(category: "  Reviews  "));

        Assert.Single(_factory.Context.Categories.Where(c => c.Title == "Reviews"));
        Assert.Equal("Reviews", result.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task CreatePostAsync_PaddedCategoryCreate_StoresOriginalTitle()
    {
        var result = await _factory.Service.CreatePostAsync(PostServiceTestFactory.CreateViewModel(category: "  New Cat  "));

        Assert.Equal("  New Cat  ", result.PostCategories.First().Category.Title);
    }
}
