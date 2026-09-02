using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceGetAlbumDetailsTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAlbumDetailsAsync_ExistingAlbum_ReturnsMappedViewModel()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Details Album", "Details Artist", "Details Genre");
        await _factory.SeedReleaseAsync(album.Id, "Vinyl");

        var result = await _factory.Service.GetAlbumDetailsAsync(album.Id);

        Assert.Equal(album.Id, result.AlbumId);
        Assert.Equal("Details Album", result.Title);
        Assert.Equal("Details Artist", result.Artist);
        Assert.Equal("Details Genre", result.Genre);
        Assert.Equal(album.AddedDate, result.AddedDate);
        Assert.Equal(album.UpdateDate, result.UpdateDate);
        Assert.Single(result.Releases!);
        Assert.Equal("Vinyl", result.Releases!.First().Source);
    }

    [Fact]
    public async Task GetAlbumDetailsAsync_NonExistingAlbum_ThrowsKeyNotFoundException()
    {
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _factory.Service.GetAlbumDetailsAsync(9999));

        Assert.Contains("9999", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task GetAlbumDetailsAsync_InvalidId_ThrowsKeyNotFoundException(int id)
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _factory.Service.GetAlbumDetailsAsync(id));
    }
}
