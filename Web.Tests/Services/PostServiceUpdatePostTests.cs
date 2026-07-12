using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceUpdatePostTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task UpdatePostAsync_ExistingPost_UpdatesFieldsAndUpdatedDate()
    {
        var seeded = await _factory.SeedPostAsync(title: "Old", categoryTitle: "News");
        var model = PostServiceTestFactory.CreateViewModel(title: "New", description: "New desc", content: "New content", category: "News");

        var result = await _factory.Service.UpdatePostAsync(seeded.Id, model);

        Assert.Equal("New", result.Title);
        Assert.Equal("New desc", result.Description);
        Assert.Equal("New content", result.Content);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdatedDate);
    }

    [Fact]
    public async Task UpdatePostAsync_NonExistingPost_ThrowsKeyNotFoundException()
    {
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _factory.Service.UpdatePostAsync(999, PostServiceTestFactory.CreateViewModel()));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task UpdatePostAsync_ChangedCategory_ReplacesCategoryLink()
    {
        var seeded = await _factory.SeedPostAsync(title: "Post", categoryTitle: "OldCat");
        var model = PostServiceTestFactory.CreateViewModel(category: "NewCat");

        var result = await _factory.Service.UpdatePostAsync(seeded.Id, model);

        Assert.Single(result.PostCategories);
        Assert.Equal("NewCat", result.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task UpdatePostAsync_PlaceholderCategory_PreservesExistingCategory()
    {
        var seeded = await _factory.SeedPostAsync(title: "Post", categoryTitle: "KeepMe");
        var model = PostServiceTestFactory.CreateViewModel(category: "Category");

        var result = await _factory.Service.UpdatePostAsync(seeded.Id, model);

        Assert.Equal("KeepMe", result.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task UpdatePostAsync_SameCategoryAfterTrim_DoesNotReplaceCategory()
    {
        var seeded = await _factory.SeedPostAsync(title: "Post", categoryTitle: "News");
        var originalCategoryId = seeded.PostCategories.First().CategoryId;
        var model = PostServiceTestFactory.CreateViewModel(category: "  News  ");

        var result = await _factory.Service.UpdatePostAsync(seeded.Id, model);

        Assert.Equal(originalCategoryId, result.PostCategories.First().CategoryId);
    }

    [Fact]
    public async Task UpdatePostAsync_DraftPost_RemainsDraft()
    {
        var seeded = await _factory.SeedPostAsync(title: "Draft", isDraft: true);
        var model = PostServiceTestFactory.CreateViewModel(title: "Updated Draft");

        var result = await _factory.Service.UpdatePostAsync(seeded.Id, model);

        Assert.True(result.IsDraft);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdatePostAsync_EmptyCategory_PreservesExistingCategory(string? category)
    {
        var seeded = await _factory.SeedPostAsync(title: "Post", categoryTitle: "Stable");
        var model = PostServiceTestFactory.CreateViewModel(category: category!);

        var result = await _factory.Service.UpdatePostAsync(seeded.Id, model);

        Assert.Equal("Stable", result.PostCategories.First().Category.Title);
    }
}
