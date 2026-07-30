using System.Text.Json;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("StatisticService")]
public class StatisticServiceProcessAsyncFreshStatTests : IDisposable
{
    private readonly StatisticServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ProcessAsync_RecentStatistic_ReturnsWithoutRefreshing()
    {
        var existingData = JsonSerializer.Serialize(new StatisticCounters { TotalAlbums = 99 });
        await _factory.SeedExistingStatisticAsync(_factory.UtcNow, new StatisticCounters { TotalAlbums = 99 });
        await _factory.SeedAlbumAsync();

        var result = await _factory.Service.ProcessAsync();

        Assert.Equal(existingData, result.Data);
        Assert.Equal(_factory.UtcNow, result.LastUpdate);
    }
}
