using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceMapAlbumToCreateUpdateVmTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task MapAlbumToCreateUpdateVMAsync_WithCover_SetsAlbumCoverToAlbumIdString()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Title", "Artist", "Genre");
        await _factory.SeedReleaseAsync(album.Id);

        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(album.Id, EntityType.AlbumCover))
            .ReturnsAsync($"/covers/{album.Id}.jpg");

        var result = await _factory.Service.MapAlbumToCreateUpdateVMAsync(album);

        Assert.Equal(album.Id.ToString(), result.AlbumCover);
        Assert.Equal(ActionType.Update, result.Action);
        Assert.Equal(album.Id, result.AlbumId);
        Assert.Equal("Title", result.Title);
        Assert.Equal("Artist", result.Artist);
        Assert.Equal("Genre", result.Genre);
        Assert.Single(result.Releases!);
    }

    [Fact]
    public async Task MapAlbumToCreateUpdateVMAsync_NoCoverUrlContainingNocover_SetsAlbumCoverNull()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Title", "Artist", "Genre");

        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(album.Id, EntityType.AlbumCover))
            .ReturnsAsync("/images/nocover.png");

        var result = await _factory.Service.MapAlbumToCreateUpdateVMAsync(album);

        Assert.Null(result.AlbumCover);
    }

    [Theory]
    [InlineData("/images/nocover.png")]
    [InlineData("/path/to/nocover/file.png")]
    public async Task MapAlbumToCreateUpdateVMAsync_CoverUrlContainsNocover_SetsAlbumCoverNull(string coverUrl)
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();

        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(album.Id, EntityType.AlbumCover))
            .ReturnsAsync(coverUrl);

        var result = await _factory.Service.MapAlbumToCreateUpdateVMAsync(album);

        Assert.Null(result.AlbumCover);
    }

    [Fact]
    public async Task MapAlbumToCreateUpdateVMAsync_NullArtistAndGenre_MapsEmptyStrings()
    {
        var album = new Album
        {
            Id = 1,
            Title = "Detached Title",
            Artist = null!,
            Genre = null!,
        };

        var result = await _factory.Service.MapAlbumToCreateUpdateVMAsync(album);

        Assert.Equal(string.Empty, result.Artist);
        Assert.Equal(string.Empty, result.Genre);
    }

    [Fact]
    public async Task MapAlbumToCreateUpdateVMAsync_LoadsReleasesFromReleaseService()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, "CD");
        await _factory.SeedReleaseAsync(album.Id, "Vinyl");

        var result = await _factory.Service.MapAlbumToCreateUpdateVMAsync(album);

        Assert.Equal(2, result.Releases!.Count());
        _factory.ImageServiceMock.Verify(s => s.GetUrlAsync(album.Id, EntityType.AlbumCover), Times.Once);
    }

    [Fact]
    public async Task MapAlbumToCreateUpdateVMAsync_EmptyReleases_ReturnsEmptyList()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();

        var result = await _factory.Service.MapAlbumToCreateUpdateVMAsync(album);

        Assert.NotNull(result.Releases);
        Assert.Empty(result.Releases);
    }
}
