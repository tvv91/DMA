using System.Text.Json;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("StatisticService")]
public class StatisticServiceProcessAsyncCreationTests : IDisposable
{
    private readonly StatisticServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ProcessAsync_NoExistingStatistic_CreatesAndPersistsStatistic()
    {
        await _factory.SeedAlbumAsync();

        var result = await _factory.Service.ProcessAsync();

        Assert.True(result.Id > 0);
        Assert.Equal(_factory.UtcNow, result.LastUpdate);
        Assert.Single(_factory.Context.Statistics);
        Assert.False(string.IsNullOrWhiteSpace(result.Data));
    }

    [Fact]
    public async Task ProcessAsync_NoExistingStatistic_ReturnsCurrentCounters()
    {
        await _factory.SeedAlbumAsync("Album One", "Artist One", "Rock");
        await _factory.SeedAlbumAsync("Album Two", "Artist Two", "Jazz");
        await _factory.SeedStorageAsync("Shelf A");

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Equal(2, counters.TotalAlbums);
        Assert.Equal(2, counters.TotalArtists);
        Assert.Equal(1, counters.StorageCount);
        Assert.Equal(0, counters.TotalReleases);
    }
}
