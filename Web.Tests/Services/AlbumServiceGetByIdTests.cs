using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceGetByIdTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsAlbumWithIncludes()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Loaded Album", "Loaded Artist", "Loaded Genre");

        var result = await _factory.Service.GetByIdAsync(album.Id);

        Assert.NotNull(result);
        Assert.Equal("Loaded Album", result!.Title);
        Assert.NotNull(result.Artist);
        Assert.Equal("Loaded Artist", result.Artist.Name);
        Assert.NotNull(result.Genre);
        Assert.Equal("Loaded Genre", result.Genre.Name);
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
        await _factory.SeedAlbumAsync();

        var result = await _factory.Service.GetByIdAsync(id);

        Assert.Null(result);
    }
}
