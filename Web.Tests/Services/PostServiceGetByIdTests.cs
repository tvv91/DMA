using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceGetByIdTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetByIdAsync_ExistingPost_ReturnsPostWithCategories()
    {
        var seeded = await _factory.SeedPostAsync(title: "Loaded Post", categoryTitle: "News");

        var result = await _factory.Service.GetByIdAsync(seeded.Id);

        Assert.NotNull(result);
        Assert.Equal("Loaded Post", result!.Title);
        Assert.Single(result.PostCategories);
        Assert.Equal("News", result.PostCategories.First().Category.Title);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _factory.Service.GetByIdAsync(9999);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await _factory.Service.GetByIdAsync(id);

        Assert.Null(result);
    }
}
