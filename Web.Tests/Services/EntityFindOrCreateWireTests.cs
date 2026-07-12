using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateWireTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateWireAsync_NewNameWithoutManufacturer_CreatesWire()
    {
        var result = await _factory.Service.FindOrCreateWireAsync("Reference Cable");

        Assert.True(result.Id > 0);
        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreateWireAsync_NewNameWithManufacturer_LinksManufacturer()
    {
        var result = await _factory.Service.FindOrCreateWireAsync("Reference Cable", "Van den Hul");

        Assert.NotNull(result.ManufacturerId);
        Assert.Equal("Van den Hul", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreateWireAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateWireAsync("Budget Wire");
        var second = await _factory.Service.FindOrCreateWireAsync("Budget Wire");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateWireAsync_ExistingWithDifferentManufacturer_UpdatesManufacturer()
    {
        await _factory.Service.FindOrCreateWireAsync("Budget Wire", "Brand A");

        var updated = await _factory.Service.FindOrCreateWireAsync("Budget Wire", "Brand B");

        Assert.Equal("Brand B", _factory.Context.Manufacturer.Single(m => m.Id == updated.ManufacturerId).Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindOrCreateWireAsync_WhitespaceManufacturerOnExisting_DoesNotClearManufacturer(string? manufacturer)
    {
        var original = await _factory.Service.FindOrCreateWireAsync("Budget Wire", "Brand A");
        var manufacturerId = original.ManufacturerId;

        var updated = await _factory.Service.FindOrCreateWireAsync("Budget Wire", manufacturer);

        Assert.Equal(manufacturerId, updated.ManufacturerId);
    }
}
