using System.Text.Json;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("StatisticService")]
public class StatisticServiceProcessAsyncStaleStatTests : IDisposable
{
    private readonly StatisticServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ProcessAsync_StaleStatistic_RefreshesData()
    {
        var staleUpdate = _factory.UtcNow.AddDays(-2);
        await _factory.SeedExistingStatisticAsync(staleUpdate, new StatisticCounters { TotalAlbums = 1 });
        await _factory.SeedAlbumAsync("Fresh Album");

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Equal(1, counters.TotalAlbums);
        Assert.Equal(_factory.UtcNow, result.LastUpdate);
        Assert.NotEqual(staleUpdate, result.LastUpdate);
    }

    [Fact]
    public async Task ProcessAsync_StaleStatistic_UpdatesPersistedEntity()
    {
        var staleUpdate = _factory.UtcNow.AddDays(-3);
        var existing = await _factory.SeedExistingStatisticAsync(
            staleUpdate,
            new StatisticCounters { TotalAlbums = 0, TotalReleases = 0 });
        await _factory.SeedAlbumAsync();
        var (album, _, _) = await _factory.SeedAlbumAsync("Second");
        await _factory.SeedReleaseAsync(album.Id, size: 1.5);

        await _factory.Service.ProcessAsync();

        var persisted = await _factory.Context.Statistics.FindAsync(existing.Id);
        var counters = _factory.DeserializeCounters(persisted!.Data);

        Assert.Equal(2, counters.TotalAlbums);
        Assert.Equal(1, counters.TotalReleases);
        Assert.Equal(1.5, counters.TotalSize);
        Assert.Equal(_factory.UtcNow, persisted.LastUpdate);
    }
}
