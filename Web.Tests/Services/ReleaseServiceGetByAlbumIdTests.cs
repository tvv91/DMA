using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceGetByAlbumIdTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetByAlbumIdAsync_WithReleases_ReturnsAlbumReleases()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, source: "Vinyl");
        await _factory.SeedReleaseAsync(album.Id, source: "CD");

        var result = await _factory.Service.GetByAlbumIdAsync(album.Id);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Source == "Vinyl");
        Assert.Contains(result, r => r.Source == "CD");
    }

    [Fact]
    public async Task GetByAlbumIdAsync_NoReleases_ReturnsEmpty()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();

        var result = await _factory.Service.GetByAlbumIdAsync(album.Id);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByAlbumIdAsync_DifferentAlbum_ReturnsOnlyMatchingReleases()
    {
        var (albumOne, _, _) = await _factory.SeedAlbumAsync("Album One");
        var (albumTwo, _, _) = await _factory.SeedAlbumAsync("Album Two");
        await _factory.SeedReleaseAsync(albumOne.Id, source: "One");
        await _factory.SeedReleaseAsync(albumTwo.Id, source: "Two");

        var result = await _factory.Service.GetByAlbumIdAsync(albumOne.Id);

        Assert.Single(result);
        Assert.Equal("One", result.First().Source);
    }

    [Fact]
    public async Task GetByAlbumIdAsync_ProjectsCountryAndYear()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(
            album.Id,
            source: "Vinyl",
            country: new Country { Name = "USA" },
            year: new Year { Value = 1973 });

        var result = await _factory.Service.GetByAlbumIdAsync(album.Id);
        var release = result.Single();

        Assert.Equal("USA", release.Country!.Name);
        Assert.Equal(1973, release.Year!.Value);
    }
}
