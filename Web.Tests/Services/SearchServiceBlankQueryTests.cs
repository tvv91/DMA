using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceBlankQueryTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [MemberData(nameof(BlankValues))]
    public async Task SearchAsync_BlankQuery_EntityWithoutListAll_ReturnsEmpty(EntityType type, string value)
    {
        await _factory.SeedArtistAsync("Pink Floyd");

        var result = await _factory.Service.SearchAsync(type, value);

        Assert.Empty(result);
    }

    [Theory]
    [MemberData(nameof(ListAllTypes))]
    public async Task SearchAsync_BlankQuery_ListAllEntity_ReturnsItems(EntityType type)
    {
        await SeedListAllEntityAsync(type);

        var result = await _factory.Service.SearchAsync(type, "");

        Assert.NotEmpty(result);
        Assert.All(result, item =>
        {
            Assert.False(string.IsNullOrWhiteSpace(item.Label));
            Assert.False(string.IsNullOrWhiteSpace(item.Value));
        });
    }

    private async Task SeedListAllEntityAsync(EntityType type)
    {
        switch (type)
        {
            case EntityType.VinylState:
            case EntityType.DigitalFormat:
            case EntityType.SourceFormat:
            case EntityType.Bitness:
            case EntityType.Sampling:
                return;
            case EntityType.Year:
                await _factory.SeedYearAsync(1973);
                return;
            case EntityType.Reissue:
                await _factory.SeedReissueAsync(1);
                return;
            case EntityType.Country:
                await _factory.SeedCountryAsync("Japan");
                return;
            case EntityType.Label:
                await _factory.SeedLabelAsync("Blue Note");
                return;
            case EntityType.Player:
                await _factory.SeedPlayerAsync("Test Player");
                return;
            case EntityType.Cartridge:
                await _factory.SeedCartridgeAsync("VM95");
                return;
            case EntityType.Amplifier:
                await _factory.SeedAmplifierAsync("PM6007");
                return;
            case EntityType.Adc:
                await _factory.SeedAdcAsync("ADS-1");
                return;
            case EntityType.Wire:
                await _factory.SeedWireAsync("Reference");
                return;
            case EntityType.PlayerManufacturer:
            case EntityType.CartridgeManufacturer:
            case EntityType.AmplifierManufacturer:
            case EntityType.AdcManufacturer:
            case EntityType.WireManufacturer:
                await _factory.SeedManufacturerAsync("Test Manufacturer");
                return;
            default:
                throw new InvalidOperationException($"Unexpected list-all type: {type}");
        }
    }

    [Fact]
    public async Task SearchAsync_WhitespaceQuery_ListAllEntity_ReturnsItems()
    {
        await _factory.SeedCountryAsync("Japan");

        var result = await _factory.Service.SearchAsync(EntityType.Country, "   ");

        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task SearchAsync_BlankQuery_VinylState_ReturnsSeededStatesOrderedByName()
    {
        var result = await _factory.Service.SearchAsync(EntityType.VinylState, "");

        Assert.Contains(result, x => x.Label == "Mint");
        Assert.Contains(result, x => x.Label == "Near Mint");
        Assert.True(result.Count <= SearchServiceTestFactory.AutocompleteMaxItems);
        Assert.Equal(result.OrderBy(x => x.Label).Select(x => x.Label), result.Select(x => x.Label));
    }

    public static IEnumerable<object[]> BlankValues()
    {
        foreach (var type in SearchServiceTestFactory.EmptyOnBlankQueryTypes)
        {
            yield return new object[] { type, "" };
            yield return new object[] { type, "   " };
        }
    }

    public static IEnumerable<object[]> ListAllTypes() =>
        SearchServiceTestFactory.ListAllOnEmptyQueryTypes.Select(t => new object[] { t });
}
