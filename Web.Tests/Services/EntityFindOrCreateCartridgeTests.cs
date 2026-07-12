using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateCartridgeTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateCartridgeAsync_NewNameWithoutManufacturer_CreatesCartridge()
    {
        var result = await _factory.Service.FindOrCreateCartridgeAsync("AT-95E");

        Assert.True(result.Id > 0);
        Assert.Equal("AT-95E", result.Name);
        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreateCartridgeAsync_NewNameWithManufacturer_LinksManufacturer()
    {
        var result = await _factory.Service.FindOrCreateCartridgeAsync("2M Red", "Ortofon");

        Assert.NotNull(result.ManufacturerId);
        Assert.Equal("Ortofon", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreateCartridgeAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateCartridgeAsync("VM95");
        var second = await _factory.Service.FindOrCreateCartridgeAsync("VM95");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateCartridgeAsync_ExistingWithoutManufacturer_AddsManufacturerWhenProvided()
    {
        await _factory.Service.FindOrCreateCartridgeAsync("VM95");

        var updated = await _factory.Service.FindOrCreateCartridgeAsync("VM95", "Audio-Technica");

        Assert.NotNull(updated.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreateCartridgeAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateCartridgeAsync(null!));
    }
}
