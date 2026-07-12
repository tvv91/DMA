using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateCountryTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateCountryAsync_NewName_CreatesCountry()
    {
        var result = await _factory.Service.FindOrCreateCountryAsync("Ukraine");

        Assert.True(result.Id > 0);
        Assert.Equal("Ukraine", result.Name);
        Assert.Single(_factory.Context.Countries);
    }

    [Fact]
    public async Task FindOrCreateCountryAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateCountryAsync("Germany");
        var second = await _factory.Service.FindOrCreateCountryAsync("Germany");

        Assert.Equal(first.Id, second.Id);
        Assert.Single(_factory.Context.Countries);
    }

    [Fact]
    public async Task FindOrCreateCountryAsync_TrimmedInput_FindsExistingByNormalizedName()
    {
        var first = await _factory.Service.FindOrCreateCountryAsync("Japan");

        var second = await _factory.Service.FindOrCreateCountryAsync("  Japan  ");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateCountryAsync_PaddedCreate_StoresOriginalName()
    {
        var result = await _factory.Service.FindOrCreateCountryAsync("  Poland  ");

        Assert.Equal("  Poland  ", result.Name);
    }

    [Fact]
    public async Task FindOrCreateCountryAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateCountryAsync(null!));
    }

    [Fact]
    public async Task FindOrCreateCountryAsync_WhitespaceOnly_CreatesEntryWithWhitespace()
    {
        var result = await _factory.Service.FindOrCreateCountryAsync("   ");

        Assert.Equal("   ", result.Name);
    }
}
