using Moq;
using Web.Enums;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceGetIndexListTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetIndexListAsync_NoFilters_ReturnsAllAlbumsPaginated()
    {
        await _factory.SeedAlbumAsync("Album One", "Artist A", "Rock");
        await _factory.SeedAlbumAsync("Album Two", "Artist B", "Jazz");
        await _factory.SeedAlbumAsync("Album Three", "Artist C", "Pop");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 2);

        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetIndexListAsync_SecondPage_ReturnsRemainingItems()
    {
        await _factory.SeedAlbumAsync("Album One");
        await _factory.SeedAlbumAsync("Album Two");
        await _factory.SeedAlbumAsync("Album Three");

        var result = await _factory.Service.GetIndexListAsync(page: 2, pageSize: 2);

        Assert.Equal(3, result.TotalItems);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetIndexListAsync_PageBeyondRange_ReturnsEmptyItems()
    {
        await _factory.SeedAlbumAsync("Album One");

        var result = await _factory.Service.GetIndexListAsync(page: 5, pageSize: 10);

        Assert.Equal(1, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetIndexListAsync_PageZero_NormalizedToFirstPage()
    {
        await _factory.SeedAlbumAsync("Album One");
        await _factory.SeedAlbumAsync("Album Two");

        var result = await _factory.Service.GetIndexListAsync(page: 0, pageSize: 10);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetIndexListAsync_NullOrWhitespaceArtistName_IgnoresArtistFilter(string? artistName)
    {
        await _factory.SeedAlbumAsync("Matching Album", "Pink Floyd", "Rock");
        await _factory.SeedAlbumAsync("Other Album", "Beatles", "Rock");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, artistName: artistName);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetIndexListAsync_ArtistNameFilter_ReturnsPartialMatches()
    {
        await _factory.SeedAlbumAsync("Album One", "Pink Floyd", "Rock");
        await _factory.SeedAlbumAsync("Album Two", "Beatles", "Rock");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, artistName: "Floyd");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Album One", result.Items[0].Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetIndexListAsync_NullOrWhitespaceGenreName_IgnoresGenreFilter(string? genreName)
    {
        await _factory.SeedAlbumAsync("Album One", "Artist A", "Rock");
        await _factory.SeedAlbumAsync("Album Two", "Artist B", "Jazz");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, genreName: genreName);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetIndexListAsync_GenreNameFilter_ReturnsPartialMatches()
    {
        await _factory.SeedAlbumAsync("Album One", "Artist A", "Progressive Rock");
        await _factory.SeedAlbumAsync("Album Two", "Artist B", "Jazz");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, genreName: "Rock");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Album One", result.Items[0].Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetIndexListAsync_NullOrWhitespaceAlbumTitle_IgnoresTitleFilter(string? albumTitle)
    {
        await _factory.SeedAlbumAsync("Dark Side", "Artist A", "Rock");
        await _factory.SeedAlbumAsync("Abbey Road", "Artist B", "Rock");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, albumTitle: albumTitle);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetIndexListAsync_AlbumTitleFilter_ReturnsPartialMatches()
    {
        await _factory.SeedAlbumAsync("Dark Side of the Moon", "Artist A", "Rock");
        await _factory.SeedAlbumAsync("Abbey Road", "Artist B", "Rock");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, albumTitle: "Dark");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Dark Side of the Moon", result.Items[0].Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetIndexListAsync_NullOrWhitespaceYearValue_IgnoresYearFilter(string? yearValue)
    {
        await _factory.SeedAlbumAsync("Album One", releaseYear: 1973);
        await _factory.SeedAlbumAsync("Album Two", releaseYear: 1980);

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, yearValue: yearValue);

        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetIndexListAsync_YearValueAsInteger_ReturnsExactYearMatches()
    {
        await _factory.SeedAlbumAsync("Album 1973", releaseYear: 1973);
        await _factory.SeedAlbumAsync("Album 1980", releaseYear: 1980);

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, yearValue: "1973");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Album 1973", result.Items[0].Title);
    }

    [Fact]
    public async Task GetIndexListAsync_YearValueParseableAsInteger_MatchesExactReleaseYear()
    {
        await _factory.SeedAlbumAsync("Album 1973", releaseYear: 1973);
        await _factory.SeedAlbumAsync("Album 1980", releaseYear: 1980);
        await _factory.SeedAlbumAsync("Album 73", releaseYear: 73);

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, yearValue: "73");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Album 73", result.Items[0].Title);
    }

    [Fact]
    public async Task GetIndexListAsync_YearValueNonParseableWithoutMatch_ReturnsEmpty()
    {
        await _factory.SeedAlbumAsync("Album 1973", releaseYear: 1973);

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, yearValue: "73x");

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetIndexListAsync_YearValueNonParseableText_ReturnsEmpty()
    {
        await _factory.SeedAlbumAsync("Album 1973", releaseYear: 1973);

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, yearValue: "not-a-year");

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetIndexListAsync_AlbumWithoutReleaseYear_ExcludedByYearFilter()
    {
        await _factory.SeedAlbumAsync("Album With Year", releaseYear: 1973);
        await _factory.SeedAlbumAsync("Album Without Year");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, yearValue: "1973");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Album With Year", result.Items[0].Title);
    }

    [Fact]
    public async Task GetIndexListAsync_CombinedFilters_ReturnsMatchingAlbum()
    {
        await _factory.SeedAlbumAsync("Dark Side", "Pink Floyd", "Rock", releaseYear: 1973);
        await _factory.SeedAlbumAsync("Dark Side Live", "Pink Floyd", "Rock", releaseYear: 1980);
        await _factory.SeedAlbumAsync("Abbey Road", "Beatles", "Rock", releaseYear: 1969);

        var result = await _factory.Service.GetIndexListAsync(
            page: 1,
            pageSize: 10,
            artistName: "Floyd",
            genreName: "Rock",
            yearValue: "1973",
            albumTitle: "Dark Side");

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Dark Side", result.Items[0].Title);
    }

    [Fact]
    public async Task GetIndexListAsync_IncludesArtistAndGenreNavigation()
    {
        await _factory.SeedAlbumAsync("Album One", "Loaded Artist", "Loaded Genre");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10);

        var album = result.Items[0];
        Assert.NotNull(album.Artist);
        Assert.Equal("Loaded Artist", album.Artist.Name);
        Assert.NotNull(album.Genre);
        Assert.Equal("Loaded Genre", album.Genre.Name);
    }

    [Fact]
    public async Task GetIndexListAsync_NoMatches_ReturnsEmptyItems()
    {
        await _factory.SeedAlbumAsync("Album One", "Artist A", "Rock");

        var result = await _factory.Service.GetIndexListAsync(page: 1, pageSize: 10, artistName: "NonExistent");

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }
}
