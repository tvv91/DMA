using Microsoft.EntityFrameworkCore;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateAdcTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateAdcAsync_NewNameWithoutManufacturer_CreatesAdc()
    {
        var result = await _factory.Service.FindOrCreateAdcAsync("ADS-1");

        Assert.True(result.Id > 0);
        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreateAdcAsync_NewNameWithManufacturer_LinksManufacturer()
    {
        var result = await _factory.Service.FindOrCreateAdcAsync("ADS-1", "Tascam");

        Assert.NotNull(result.ManufacturerId);
        Assert.Equal("Tascam", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreateAdcAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateAdcAsync("UA-2G");
        var second = await _factory.Service.FindOrCreateAdcAsync("UA-2G");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateAdcAsync_ExistingWithoutManufacturer_AddsManufacturerWhenProvided()
    {
        await _factory.Service.FindOrCreateAdcAsync("UA-2G");

        var updated = await _factory.Service.FindOrCreateAdcAsync("UA-2G", "Edirol");

        Assert.NotNull(updated.ManufacturerId);
    }
}
