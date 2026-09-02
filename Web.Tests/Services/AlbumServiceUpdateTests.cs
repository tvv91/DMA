using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceUpdateTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateAlbumAsync_InvalidAlbumId_ThrowsInvalidDataException(int albumId)
    {
        var exception = await Assert.ThrowsAsync<InvalidDataException>(
            () => _factory.Service.UpdateAlbumAsync(albumId, "Title", "Artist", "Genre"));

        Assert.Equal("AlbumId is invalid", exception.Message);
    }

    [Fact]
    public async Task UpdateAlbumAsync_NonExistingAlbum_ThrowsKeyNotFoundException()
    {
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _factory.Service.UpdateAlbumAsync(9999, "Title", "Artist", "Genre"));

        Assert.Contains("9999", exception.Message);
    }

    [Fact]
    public async Task UpdateAlbumAsync_NoChanges_ReturnsExistingWithoutUpdateDate()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Same Title", "Same Artist", "Same Genre");
        var originalUpdateDate = album.UpdateDate;

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "Same Title", "Same Artist", "Same Genre");

        Assert.Equal(album.Id, result.Id);
        Assert.Null(result.UpdateDate);
        Assert.Equal(originalUpdateDate, result.UpdateDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateAlbumAsync_NullOrWhitespaceArtistAndGenreWithSameTitle_ReturnsWithoutChanges(string? blank)
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Same Title", "Same Artist", "Same Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "Same Title", blank, blank);

        Assert.Equal(album.Id, result.Id);
        Assert.Null(result.UpdateDate);
    }

    [Fact]
    public async Task UpdateAlbumAsync_TitleChange_UpdatesTitleAndUpdateDate()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Old Title", "Same Artist", "Same Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "New Title", "Same Artist", "Same Genre");

        Assert.Equal("New Title", result.Title);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdateDate);
    }

    [Fact]
    public async Task UpdateAlbumAsync_ArtistChange_FindsOrCreatesArtist()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Title", "Old Artist", "Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "Title", "New Artist", "Genre");

        Assert.Equal("New Artist", result.Artist!.Name);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdateDate);
    }

    [Fact]
    public async Task UpdateAlbumAsync_GenreChange_FindsOrCreatesGenre()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Title", "Artist", "Old Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "Title", "Artist", "New Genre");

        Assert.Equal("New Genre", result.Genre!.Name);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdateDate);
    }

    [Fact]
    public async Task UpdateAlbumAsync_ReusesExistingArtistOnUpdate()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Album One", "Artist One", "Rock");
        var (_, sharedArtist, _) = await _factory.SeedAlbumAsync("Album Two", "Shared Artist", "Rock");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "Album One", sharedArtist.Name, "Rock");

        Assert.Equal(sharedArtist.Id, result.ArtistId);
        Assert.Equal(2, _factory.Context.Artists.Count());
    }

    [Fact]
    public async Task UpdateAlbumAsync_OnlyTitleChangeWhenArtistAndGenreNull_UpdatesTitleOnly()
    {
        var (album, originalArtist, originalGenre) = await _factory.SeedAlbumAsync("Old Title", "Artist", "Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "New Title", null, null);

        Assert.Equal("New Title", result.Title);
        Assert.Equal(originalArtist.Id, result.ArtistId);
        Assert.Equal(originalGenre.Id, result.GenreId);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdateDate);
    }

    [Fact]
    public async Task UpdateAlbumAsync_WhitespaceArtistAndGenre_DoesNotChangeArtistOrGenre()
    {
        var (album, originalArtist, originalGenre) = await _factory.SeedAlbumAsync("Title", "Artist", "Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "Updated Title", "   ", "   ");

        Assert.Equal("Updated Title", result.Title);
        Assert.Equal(originalArtist.Id, result.ArtistId);
        Assert.Equal(originalGenre.Id, result.GenreId);
    }

    [Fact]
    public async Task UpdateAlbumAsync_AllFieldsChange_UpdatesEverything()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Old Title", "Old Artist", "Old Genre");

        var result = await _factory.Service.UpdateAlbumAsync(album.Id, "New Title", "New Artist", "New Genre");

        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Artist", result.Artist!.Name);
        Assert.Equal("New Genre", result.Genre!.Name);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdateDate);
    }
}
