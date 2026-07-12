using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceDeletePostTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task DeletePostAsync_ExistingPost_ReturnsTrueAndRemovesPost()
    {
        var seeded = await _factory.SeedPostAsync(categoryTitle: "News");

        var result = await _factory.Service.DeletePostAsync(seeded.Id);

        Assert.True(result);
        Assert.Empty(_factory.Context.Posts);
    }

    [Fact]
    public async Task DeletePostAsync_ExistingPost_KeepsCategoryEntity()
    {
        var seeded = await _factory.SeedPostAsync(categoryTitle: "Persistent");

        await _factory.Service.DeletePostAsync(seeded.Id);

        Assert.Single(_factory.Context.Categories.Where(c => c.Title == "Persistent"));
    }

    [Fact]
    public async Task DeletePostAsync_NonExistingPost_ReturnsFalse()
    {
        var result = await _factory.Service.DeletePostAsync(999);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeletePostAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await _factory.Service.DeletePostAsync(id);

        Assert.False(result);
    }
}
