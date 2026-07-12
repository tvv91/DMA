using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateVinylStateTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateVinylStateAsync_SeededValue_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateVinylStateAsync("Mint");

        Assert.Equal("Mint", result.Name);
    }

    [Fact]
    public async Task FindOrCreateVinylStateAsync_NewName_CreatesState()
    {
        var result = await _factory.Service.FindOrCreateVinylStateAsync("Fair");

        Assert.True(result.Id > 0);
        Assert.Equal("Fair", result.Name);
    }

    [Fact]
    public async Task FindOrCreateVinylStateAsync_TrimmedInput_FindsSeededState()
    {
        var result = await _factory.Service.FindOrCreateVinylStateAsync("  Near Mint  ");

        Assert.Equal("Near Mint", result.Name);
    }

    [Fact]
    public async Task FindOrCreateVinylStateAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateVinylStateAsync(null!));
    }
}
