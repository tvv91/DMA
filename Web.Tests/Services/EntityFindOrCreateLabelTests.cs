using Microsoft.EntityFrameworkCore;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateLabelTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateLabelAsync_NewName_CreatesLabel()
    {
        var result = await _factory.Service.FindOrCreateLabelAsync("EMI");

        Assert.True(result.Id > 0);
        Assert.Equal("EMI", result.Name);
    }

    [Fact]
    public async Task FindOrCreateLabelAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateLabelAsync("Columbia");
        var second = await _factory.Service.FindOrCreateLabelAsync("Columbia");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateLabelAsync_TrimmedInput_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreateLabelAsync("Decca");

        var second = await _factory.Service.FindOrCreateLabelAsync("  Decca  ");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateLabelAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateLabelAsync(null!));
    }
}
