using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceCreateOrFindTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task CreateOrFindAlbumAsync_NewAlbum_CreatesArtistGenreAndAlbum()
    {
        var result = await _factory.Service.CreateOrFindAlbumAsync("New Album", "New Artist", "New Genre");

        Assert.True(result.Id > 0);
        Assert.Equal("New Album", result.Title);
        Assert.Equal(_factory.FixedLocalNow, result.AddedDate);

        var artist = _factory.Context.Artists.Single(a => a.Name == "New Artist");
        var genre = _factory.Context.Genres.Single(g => g.Name == "New Genre");
        Assert.Equal(artist.Id, result.ArtistId);
        Assert.Equal(genre.Id, result.GenreId);
        Assert.Equal(1, _factory.Context.Albums.Count());
    }

    [Fact]
    public async Task CreateOrFindAlbumAsync_TrimsInputValues()
    {
        var result = await _factory.Service.CreateOrFindAlbumAsync("  Trimmed Album  ", "  Trimmed Artist  ", "  Trimmed Genre  ");

        Assert.Equal("Trimmed Album", result.Title);
        Assert.Single(_factory.Context.Artists.Where(a => a.Name == "Trimmed Artist"));
        Assert.Single(_factory.Context.Genres.Where(g => g.Name == "Trimmed Genre"));
    }

    [Fact]
    public async Task CreateOrFindAlbumAsync_DuplicateTitleAndArtist_ReturnsExistingAlbum()
    {
        var (existing, _, _) = await _factory.SeedAlbumAsync("Existing Album", "Existing Artist", "Rock");

        var result = await _factory.Service.CreateOrFindAlbumAsync("Existing Album", "Existing Artist", "Different Genre");

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal(1, _factory.Context.Albums.Count());
    }

    [Fact]
    public async Task CreateOrFindAlbumAsync_DuplicateWithWhitespace_ReturnsExistingAlbum()
    {
        var (existing, _, _) = await _factory.SeedAlbumAsync("Existing Album", "Existing Artist", "Rock");

        var result = await _factory.Service.CreateOrFindAlbumAsync("  Existing Album  ", "  Existing Artist  ", "  Jazz  ");

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal(1, _factory.Context.Albums.Count());
    }

    [Fact]
    public async Task CreateOrFindAlbumAsync_ReusesExistingArtist()
    {
        await _factory.SeedAlbumAsync("First Album", "Shared Artist", "Rock");

        await _factory.Service.CreateOrFindAlbumAsync("Second Album", "Shared Artist", "Jazz");

        Assert.Equal(1, _factory.Context.Artists.Count(a => a.Name == "Shared Artist"));
        Assert.Equal(2, _factory.Context.Albums.Count());
    }

    [Fact]
    public async Task CreateOrFindAlbumAsync_ReusesExistingGenre()
    {
        await _factory.SeedAlbumAsync("First Album", "Artist One", "Shared Genre");

        await _factory.Service.CreateOrFindAlbumAsync("Second Album", "Artist Two", "Shared Genre");

        Assert.Equal(1, _factory.Context.Genres.Count(g => g.Name == "Shared Genre"));
        Assert.Equal(2, _factory.Context.Albums.Count());
    }

    [Fact]
    public async Task CreateOrFindAlbumAsync_SameTitleDifferentArtist_CreatesNewAlbum()
    {
        await _factory.SeedAlbumAsync("Same Title", "Artist One", "Rock");

        var result = await _factory.Service.CreateOrFindAlbumAsync("Same Title", "Artist Two", "Rock");

        Assert.Equal(2, _factory.Context.Albums.Count());
        Assert.Equal("Artist Two", _factory.Context.Artists.Single(a => a.Id == result.ArtistId).Name);
    }
}
