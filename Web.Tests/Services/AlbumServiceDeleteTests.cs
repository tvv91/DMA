using Moq;
using Web.Enums;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceDeleteTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task DeleteAlbumAsync_ExistingAlbum_ReturnsTrueAndRemovesAlbum()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();

        var result = await _factory.Service.DeleteAlbumAsync(album.Id);

        Assert.True(result);
        Assert.Empty(_factory.Context.Albums);
    }

    [Fact]
    public async Task DeleteAlbumAsync_NonExistingAlbum_ReturnsFalse()
    {
        var result = await _factory.Service.DeleteAlbumAsync(9999);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteAlbumAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await _factory.Service.DeleteAlbumAsync(id);

        Assert.False(result);
    }
}
