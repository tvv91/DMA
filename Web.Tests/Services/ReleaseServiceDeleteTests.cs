using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceDeleteTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task DeleteAsync_ExistingRelease_ReturnsTrueAndRemovesRelease()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var seeded = await _factory.SeedReleaseAsync(album.Id);

        var result = await _factory.Service.DeleteAsync(seeded.Id);

        Assert.True(result);
        Assert.Empty(_factory.Context.Releases);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingRelease_ReturnsFalse()
    {
        var result = await _factory.Service.DeleteAsync(999);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await _factory.Service.DeleteAsync(id);

        Assert.False(result);
    }
}
