using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateAmplifierTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateAmplifierAsync_NewNameWithoutManufacturer_CreatesAmplifier()
    {
        var result = await _factory.Service.FindOrCreateAmplifierAsync("PM-6007");

        Assert.True(result.Id > 0);
        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreateAmplifierAsync_NewNameWithManufacturer_LinksManufacturer()
    {
        var result = await _factory.Service.FindOrCreateAmplifierAsync("PM-6007", "Marantz");

        Assert.NotNull(result.ManufacturerId);
        Assert.Equal("Marantz", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreateAmplifierAsync_ExistingWithDifferentManufacturer_UpdatesManufacturer()
    {
        await _factory.Service.FindOrCreateAmplifierAsync("AX-50", "Brand A");

        var updated = await _factory.Service.FindOrCreateAmplifierAsync("AX-50", "Brand B");

        Assert.Equal("Brand B", _factory.Context.Manufacturer.Single(m => m.Id == updated.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreateAmplifierAsync_TrimmedName_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreateAmplifierAsync("AX-50");

        var second = await _factory.Service.FindOrCreateAmplifierAsync("  AX-50  ");

        Assert.Equal(first.Id, second.Id);
    }
}
