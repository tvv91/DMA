using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceStringSearchTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task SearchAsync_Artist_SubstringMatch_ReturnsMatchingArtist()
    {
        await _factory.SeedArtistAsync("Pink Floyd");
        await _factory.SeedArtistAsync("Led Zeppelin");

        var result = await _factory.Service.SearchAsync(EntityType.Artist, "Pink");

        Assert.Single(result);
        Assert.Equal("Pink Floyd", result[0].Label);
        Assert.Equal("Pink Floyd", result[0].Value);
    }

    [Fact]
    public async Task SearchAsync_Artist_CaseInsensitiveMatch_ReturnsArtist()
    {
        await _factory.SeedArtistAsync("Pink Floyd");

        var result = await _factory.Service.SearchAsync(EntityType.Artist, "pink");

        Assert.Single(result);
        Assert.Equal("Pink Floyd", result[0].Label);
    }

    [Fact]
    public async Task SearchAsync_Artist_NoMatch_ReturnsEmpty()
    {
        await _factory.SeedArtistAsync("Pink Floyd");

        var result = await _factory.Service.SearchAsync(EntityType.Artist, "Beatles");

        Assert.Empty(result);
    }

    [Theory]
    [MemberData(nameof(StringSearchTypes))]
    public async Task SearchAsync_StringEntities_SubstringMatch_ReturnsItem(EntityType type)
    {
        var (name, query) = GetStringSeedData(type);

        await SeedStringEntityAsync(type, name);

        var result = await _factory.Service.SearchAsync(type, query);

        Assert.Single(result);
        Assert.Equal(name, result[0].Label);
        Assert.Equal(name, result[0].Value);
    }

    [Fact]
    public async Task SearchAsync_Storage_WithValue_ReturnsMatchDespiteBlankListAll()
    {
        await _factory.SeedStorageAsync("Shelf A1");

        var result = await _factory.Service.SearchAsync(EntityType.Storage, "Shelf");

        Assert.Single(result);
        Assert.Equal("Shelf A1", result[0].Label);
    }

    [Fact]
    public async Task SearchAsync_StringSearch_RespectsMaxItemsLimit()
    {
        for (var i = 0; i < 15; i++)
            await _factory.SeedArtistAsync($"Rock Band {i:D2}");

        var result = await _factory.Service.SearchAsync(EntityType.Artist, "Rock");

        Assert.Equal(SearchServiceTestFactory.AutocompleteMaxItems, result.Count);
    }

    private async Task SeedStringEntityAsync(EntityType type, string name)
    {
        switch (type)
        {
            case EntityType.Artist: await _factory.SeedArtistAsync(name); break;
            case EntityType.Genre: await _factory.SeedGenreAsync(name); break;
            case EntityType.VinylState: await _factory.SeedVinylStateAsync(name); break;
            case EntityType.DigitalFormat: await _factory.SeedDigitalFormatAsync(name); break;
            case EntityType.SourceFormat: await _factory.SeedSourceFormatAsync(name); break;
            case EntityType.Country: await _factory.SeedCountryAsync(name); break;
            case EntityType.Label: await _factory.SeedLabelAsync(name); break;
            case EntityType.Storage: await _factory.SeedStorageAsync(name); break;
            case EntityType.Player: await _factory.SeedPlayerAsync(name); break;
            case EntityType.Cartridge: await _factory.SeedCartridgeAsync(name); break;
            case EntityType.Amplifier: await _factory.SeedAmplifierAsync(name); break;
            case EntityType.Adc: await _factory.SeedAdcAsync(name); break;
            case EntityType.Wire: await _factory.SeedWireAsync(name); break;
            default: throw new InvalidOperationException();
        }
    }

    private static (string Name, string Query) GetStringSeedData(EntityType type) => type switch
    {
        EntityType.Artist => ("Jazz Artist", "Jazz"),
        EntityType.Genre => ("Progressive Rock", "Progressive"),
        EntityType.VinylState => ("Custom State", "Custom"),
        EntityType.DigitalFormat => ("Custom Codec", "Custom"),
        EntityType.SourceFormat => ("Custom LP Format", "Custom"),
        EntityType.Country => ("United Kingdom", "Kingdom"),
        EntityType.Label => ("Blue Note Records", "Blue"),
        EntityType.Storage => ("Warehouse B", "Warehouse"),
        EntityType.Player => ("Technics SL-1200", "Technics"),
        EntityType.Cartridge => ("Audio-Technica VM95", "Audio"),
        EntityType.Amplifier => ("Marantz PM6007", "Marantz"),
        EntityType.Adc => ("RME ADI-2", "RME"),
        EntityType.Wire => ("Ortofon Reference", "Ortofon"),
        _ => throw new InvalidOperationException(),
    };

    public static IEnumerable<object[]> StringSearchTypes() =>
        SearchServiceTestFactory.StringSearchTypes.Select(t => new object[] { t });
}
