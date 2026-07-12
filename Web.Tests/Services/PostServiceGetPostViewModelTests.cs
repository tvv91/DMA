using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceGetPostViewModelTests : IDisposable
{
    private readonly PostServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetPostViewModelAsync_ExistingPost_ReturnsMappedViewModel()
    {
        var seeded = await _factory.SeedPostAsync(title: "VM Post", categoryTitle: "Tech");

        var result = await _factory.Service.GetPostViewModelAsync(seeded.Id);

        Assert.Equal(seeded.Id, result.Id);
        Assert.Equal("VM Post", result.Title);
        Assert.Equal("Tech", result.Category);
    }

    [Fact]
    public async Task GetPostViewModelAsync_NonExistingPost_ThrowsKeyNotFoundException()
    {
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _factory.Service.GetPostViewModelAsync(404));

        Assert.Contains("404", exception.Message);
    }
}
