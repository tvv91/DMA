using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceHasAnyAlbumsTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task HasAnyAlbumsAsync_EmptyDatabase_ReturnsFalse()
    {
        var result = await _factory.Service.HasAnyAlbumsAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task HasAnyAlbumsAsync_WithAlbums_ReturnsTrue()
    {
        await _factory.SeedAlbumAsync();

        var result = await _factory.Service.HasAnyAlbumsAsync();

        Assert.True(result);
    }

    [Fact]
    public async Task HasAnyAlbumsAsync_AfterDelete_ReturnsFalse()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.Service.DeleteAlbumAsync(album.Id);

        var result = await _factory.Service.HasAnyAlbumsAsync();

        Assert.False(result);
    }
}
