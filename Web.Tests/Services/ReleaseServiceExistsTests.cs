using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceExistsTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ExistsByAlbumIdAndSourceAsync_ExistingMatch_ReturnsTrue()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, source: "My Source");

        var result = await _factory.Service.ExistsByAlbumIdAndSourceAsync(album.Id, "My Source");

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByAlbumIdAndSourceAsync_NoMatch_ReturnsFalse()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, source: "Actual");

        var result = await _factory.Service.ExistsByAlbumIdAndSourceAsync(album.Id, "Different");

        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExistsByAlbumIdAndSourceAsync_NullOrWhitespaceSource_ReturnsFalse(string? source)
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, source: "Actual");

        var result = await _factory.Service.ExistsByAlbumIdAndSourceAsync(album.Id, source!);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByAlbumIdAndSourceAsync_CaseMismatch_ReturnsFalse()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, source: "Vinyl");

        var result = await _factory.Service.ExistsByAlbumIdAndSourceAsync(album.Id, "vinyl");

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByAlbumIdAndSourceAsync_ReleaseWithNullSource_ReturnsFalse()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, source: null);

        var result = await _factory.Service.ExistsByAlbumIdAndSourceAsync(album.Id, "Any");

        Assert.False(result);
    }
}
