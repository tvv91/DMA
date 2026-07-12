using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceSamplingTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task SearchAsync_Sampling_KhzValue_ReturnsKhzLabel()
    {
        var result = await _factory.Service.SearchAsync(EntityType.Sampling, "96");

        Assert.Contains(result, x => x.Value == "96" && x.Label == "96 kHz");
    }

    [Fact]
    public async Task SearchAsync_Sampling_DsdValue_ReturnsMhzLabel()
    {
        var result = await _factory.Service.SearchAsync(EntityType.Sampling, "2.8");

        Assert.Contains(result, x => x.Value == "2.8" && x.Label == "2.8 MHz");
    }

    [Fact]
    public async Task SearchAsync_Sampling_SearchByLabelToken_FindsDsdEntry()
    {
        var result = await _factory.Service.SearchAsync(EntityType.Sampling, "MHz");

        Assert.Contains(result, x => x.Label.EndsWith("MHz"));
    }

    [Fact]
    public async Task SearchAsync_Sampling_BlankQuery_ReturnsFormattedLabels()
    {
        var result = await _factory.Service.SearchAsync(EntityType.Sampling, "");

        Assert.Contains(result, x => x.Label == "96 kHz");
        Assert.Contains(result, x => x.Label == "2.8 MHz");
        Assert.True(result.Count <= SearchServiceTestFactory.AutocompleteMaxItems);
    }

    [Fact]
    public async Task SearchAsync_Sampling_CustomValue_ReturnsFormattedResult()
    {
        await _factory.SeedSamplingAsync(48);

        var result = await _factory.Service.SearchAsync(EntityType.Sampling, "48");

        Assert.Single(result);
        Assert.Equal("48", result[0].Value);
        Assert.Equal("48 kHz", result[0].Label);
    }
}
