using System.Text.Json;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("StatisticService")]
public class StatisticServiceProcessAsyncCooldownTests : IDisposable
{
    private readonly StatisticServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ProcessAsync_StaleStatisticWithinCooldown_SkipsRefresh()
    {
        var staleUpdate = _factory.UtcNow.AddDays(-2);
        var oldCounters = new StatisticCounters { TotalAlbums = 5, TotalReleases = 0 };
        await _factory.SeedExistingStatisticAsync(staleUpdate, oldCounters);
        StatisticServiceTestState.SetLastRefreshAttempt(_factory.UtcNow);
        await _factory.SeedAlbumAsync();

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Equal(5, counters.TotalAlbums);
        Assert.Equal(staleUpdate, result.LastUpdate);
    }

    [Fact]
    public async Task ProcessAsync_StaleStatisticAfterCooldown_RefreshesData()
    {
        var start = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        using var factory = new StatisticServiceTestFactory(start);
        var staleUpdate = start.UtcDateTime.AddDays(-2);
        await factory.SeedExistingStatisticAsync(staleUpdate, new StatisticCounters { TotalAlbums = 1 });
        StatisticServiceTestState.SetLastRefreshAttempt(start.UtcDateTime);
        await factory.SeedAlbumAsync("Updated Album");

        factory.TimeProvider.Advance(TimeSpan.FromMinutes(6));

        var result = await factory.Service.ProcessAsync();
        var counters = factory.DeserializeCounters(result.Data);

        Assert.Equal(1, counters.TotalAlbums);
        Assert.Equal(factory.UtcNow, result.LastUpdate);
    }
}
