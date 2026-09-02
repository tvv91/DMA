using System.Text.Json;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("StatisticService")]
public class StatisticServiceTopItemsTests : IDisposable
{
    private readonly StatisticServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ProcessAsync_GenreCounters_LimitedToTopTenOrderedByCount()
    {
        await _factory.SeedGenresWithAlbumCountsAsync(12);

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Equal(StatisticServiceTestFactory.TopStatisticsItems, counters.Genre!.Count);
        Assert.Equal(
            counters.Genre.OrderByDescending(x => x.Count).Select(x => x.Count),
            counters.Genre.Select(x => x.Count));
    }
}
