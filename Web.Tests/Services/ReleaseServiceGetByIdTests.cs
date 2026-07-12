using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceGetByIdTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetByIdAsync_ExistingRelease_ReturnsProjectedRelease()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var seeded = await _factory.SeedReleaseAsync(album.Id, source: "Loaded Source", discogs: "123");

        var result = await _factory.Service.GetByIdAsync(seeded.Id);

        Assert.NotNull(result);
        Assert.Equal("Loaded Source", result!.Source);
        Assert.Equal("123", result.Discogs);
        Assert.Equal(album.Id, result.AlbumId);
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
