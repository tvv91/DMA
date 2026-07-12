using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceListAllLimitsTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task SearchAsync_BlankQuery_Player_LimitsToMaxItems()
    {
        await _factory.SeedPlayersAsync(15);

        var result = await _factory.Service.SearchAsync(EntityType.Player, "");

        Assert.Equal(SearchServiceTestFactory.AutocompleteMaxItems, result.Count);
    }

    [Fact]
    public async Task SearchAsync_BlankQuery_Player_OrdersByName()
    {
        await _factory.SeedPlayerAsync("Zebra");
        await _factory.SeedPlayerAsync("Alpha");

        var result = await _factory.Service.SearchAsync(EntityType.Player, "");

        Assert.Equal("Alpha", result[0].Label);
        Assert.Equal("Zebra", result[1].Label);
    }

    [Fact]
    public async Task SearchAsync_BlankQuery_Bitness_ReturnsNumericStringValues()
    {
        var result = await _factory.Service.SearchAsync(EntityType.Bitness, "");

        Assert.Contains(result, x => x.Value == "24");
        Assert.All(result, x => Assert.Equal(x.Label, x.Value));
    }
}
