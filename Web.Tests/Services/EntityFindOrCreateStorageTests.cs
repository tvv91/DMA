using Microsoft.EntityFrameworkCore;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateStorageTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateStorageAsync_NewName_CreatesStorage()
    {
        var result = await _factory.Service.FindOrCreateStorageAsync("Shelf A");

        Assert.True(result.Id > 0);
        Assert.Equal("Shelf A", result.Name);
    }

    [Fact]
    public async Task FindOrCreateStorageAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateStorageAsync("Box 1");
        var second = await _factory.Service.FindOrCreateStorageAsync("Box 1");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateStorageAsync_TrimmedInput_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreateStorageAsync("Archive");

        var second = await _factory.Service.FindOrCreateStorageAsync("  Archive  ");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateStorageAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateStorageAsync(null!));
    }
}
