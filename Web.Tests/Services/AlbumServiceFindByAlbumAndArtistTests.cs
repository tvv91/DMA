using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceFindByAlbumAndArtistTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [InlineData(null, "Artist")]
    [InlineData("Title", null)]
    [InlineData(null, null)]
    public async Task FindByAlbumAndArtistAsync_NullArguments_ReturnsNull(string? title, string? artist)
    {
        await _factory.SeedAlbumAsync("Existing", "Existing Artist");

        var result = await _factory.Service.FindByAlbumAndArtistAsync(title!, artist!);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("", "Artist")]
    [InlineData("Title", "")]
    [InlineData("   ", "Artist")]
    [InlineData("Title", "   ")]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public async Task FindByAlbumAndArtistAsync_EmptyOrWhitespaceArguments_ReturnsNull(string title, string artist)
    {
        await _factory.SeedAlbumAsync("Existing", "Existing Artist");

        var result = await _factory.Service.FindByAlbumAndArtistAsync(title, artist);

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByAlbumAndArtistAsync_ExactMatch_ReturnsAlbum()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("The Wall", "Pink Floyd");

        var result = await _factory.Service.FindByAlbumAndArtistAsync("The Wall", "Pink Floyd");

        Assert.NotNull(result);
        Assert.Equal(album.Id, result!.Id);
    }

    [Fact]
    public async Task FindByAlbumAndArtistAsync_TrimmedInput_ReturnsAlbum()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("The Wall", "Pink Floyd");

        var result = await _factory.Service.FindByAlbumAndArtistAsync("  The Wall  ", "  Pink Floyd  ");

        Assert.NotNull(result);
        Assert.Equal(album.Id, result!.Id);
    }

    [Fact]
    public async Task FindByAlbumAndArtistAsync_WrongArtist_ReturnsNull()
    {
        await _factory.SeedAlbumAsync("The Wall", "Pink Floyd");

        var result = await _factory.Service.FindByAlbumAndArtistAsync("The Wall", "Beatles");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByAlbumAndArtistAsync_WrongTitle_ReturnsNull()
    {
        await _factory.SeedAlbumAsync("The Wall", "Pink Floyd");

        var result = await _factory.Service.FindByAlbumAndArtistAsync("Dark Side", "Pink Floyd");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByAlbumAndArtistAsync_CaseMismatch_ReturnsNull()
    {
        await _factory.SeedAlbumAsync("The Wall", "Pink Floyd");

        var result = await _factory.Service.FindByAlbumAndArtistAsync("the wall", "pink floyd");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByAlbumAndArtistAsync_IncludesArtistNavigation()
    {
        await _factory.SeedAlbumAsync("The Wall", "Pink Floyd");

        var result = await _factory.Service.FindByAlbumAndArtistAsync("The Wall", "Pink Floyd");

        Assert.NotNull(result?.Artist);
        Assert.Equal("Pink Floyd", result.Artist.Name);
    }
}
