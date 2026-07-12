using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceNumberSearchTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task SearchAsync_Year_SubstringMatch_ReturnsMatchingYears()
    {
        await _factory.SeedYearAsync(1973);
        await _factory.SeedYearAsync(2019);

        var result = await _factory.Service.SearchAsync(EntityType.Year, "19");

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Value == "1973");
        Assert.Contains(result, x => x.Value == "2019");
        Assert.All(result, x => Assert.Equal(x.Label, x.Value));
    }

    [Fact]
    public async Task SearchAsync_Year_CaseInsensitiveMatch_WorksForDigits()
    {
        await _factory.SeedYearAsync(1984);

        var result = await _factory.Service.SearchAsync(EntityType.Year, "1984");

        Assert.Single(result);
        Assert.Equal("1984", result[0].Value);
    }

    [Fact]
    public async Task SearchAsync_Bitness_FindsSeededValue()
    {
        var result = await _factory.Service.SearchAsync(EntityType.Bitness, "24");

        Assert.Single(result);
        Assert.Equal("24", result[0].Value);
    }

    [Fact]
    public async Task SearchAsync_Reissue_NoMatch_ReturnsEmpty()
    {
        await _factory.SeedReissueAsync(1);

        var result = await _factory.Service.SearchAsync(EntityType.Reissue, "99");

        Assert.Empty(result);
    }

    [Theory]
    [MemberData(nameof(NumberSearchTypes))]
    public async Task SearchAsync_NumberEntities_ReturnLabelEqualsValue(EntityType type)
    {
        var result = type switch
        {
            EntityType.Year => await SearchYearAsync(),
            EntityType.Reissue => await SearchReissueAsync(),
            EntityType.Bitness => await _factory.Service.SearchAsync(type, "32"),
            _ => throw new InvalidOperationException(),
        };

        Assert.NotEmpty(result);
        Assert.All(result, x => Assert.Equal(x.Label, x.Value));
    }

    private async Task<List<Web.Response.AutocompleteResponse>> SearchYearAsync()
    {
        await _factory.SeedYearAsync(2001);
        return await _factory.Service.SearchAsync(EntityType.Year, "2001");
    }

    private async Task<List<Web.Response.AutocompleteResponse>> SearchReissueAsync()
    {
        await _factory.SeedReissueAsync(3);
        return await _factory.Service.SearchAsync(EntityType.Reissue, "3");
    }

    public static IEnumerable<object[]> NumberSearchTypes() =>
        SearchServiceTestFactory.NumberSearchTypes.Select(t => new object[] { t });
}
