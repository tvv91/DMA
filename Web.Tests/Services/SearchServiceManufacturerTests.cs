using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceManufacturerTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [MemberData(nameof(ManufacturerTypes))]
    public async Task SearchAsync_ManufacturerTypes_SubstringMatch_ReturnsManufacturer(EntityType type)
    {
        await _factory.SeedManufacturerAsync("Audio-Technica");

        var result = await _factory.Service.SearchAsync(type, "Technica");

        Assert.Single(result);
        Assert.Equal("Audio-Technica", result[0].Label);
        Assert.Equal("Audio-Technica", result[0].Value);
    }

    [Theory]
    [MemberData(nameof(ManufacturerTypes))]
    public async Task SearchAsync_ManufacturerTypes_BlankQuery_ReturnsManufacturers(EntityType type)
    {
        await _factory.SeedManufacturerAsync("Denon");

        var result = await _factory.Service.SearchAsync(type, "");

        Assert.Contains(result, x => x.Label == "Denon");
    }

    [Fact]
    public async Task SearchAsync_Manufacturer_NoMatch_ReturnsEmpty()
    {
        await _factory.SeedManufacturerAsync("Marantz");

        var result = await _factory.Service.SearchAsync(EntityType.PlayerManufacturer, "Sony");

        Assert.Empty(result);
    }

    public static IEnumerable<object[]> ManufacturerTypes() =>
        SearchServiceTestFactory.ManufacturerEntityTypes.Select(t => new object[] { t });
}
