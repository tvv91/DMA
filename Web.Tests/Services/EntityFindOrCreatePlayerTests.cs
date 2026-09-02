using Microsoft.EntityFrameworkCore;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreatePlayerTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreatePlayerAsync_NewNameWithoutManufacturer_CreatesPlayer()
    {
        var result = await _factory.Service.FindOrCreatePlayerAsync("SL-1200");

        Assert.True(result.Id > 0);
        Assert.Equal("SL-1200", result.Name);
        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_NewNameWithManufacturer_LinksManufacturer()
    {
        var result = await _factory.Service.FindOrCreatePlayerAsync("SL-1210", "Technics");

        Assert.Equal("SL-1210", result.Name);
        Assert.NotNull(result.ManufacturerId);
        Assert.Equal("Technics", _factory.Context.Manufacturer.Single(m => m.Id == result.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreatePlayerAsync("PL-X500");
        var second = await _factory.Service.FindOrCreatePlayerAsync("PL-X500");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_TrimmedName_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreatePlayerAsync("PD-150");

        var second = await _factory.Service.FindOrCreatePlayerAsync("  PD-150  ");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_ExistingWithoutManufacturer_AddsManufacturerWhenProvided()
    {
        var player = await _factory.Service.FindOrCreatePlayerAsync("PD-150");

        var updated = await _factory.Service.FindOrCreatePlayerAsync("PD-150", "Denon");

        Assert.Equal(player.Id, updated.Id);
        Assert.NotNull(updated.ManufacturerId);
        Assert.Equal("Denon", _factory.Context.Manufacturer.Single(m => m.Id == updated.ManufacturerId).Name);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_ExistingWithSameManufacturer_DoesNotChangeManufacturer()
    {
        var first = await _factory.Service.FindOrCreatePlayerAsync("PD-150", "Denon");
        var manufacturerId = first.ManufacturerId;

        var second = await _factory.Service.FindOrCreatePlayerAsync("PD-150", "Denon");

        Assert.Equal(manufacturerId, second.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_ExistingWithDifferentManufacturer_UpdatesManufacturer()
    {
        var player = await _factory.Service.FindOrCreatePlayerAsync("PD-150", "Denon");

        var updated = await _factory.Service.FindOrCreatePlayerAsync("PD-150", "Technics");

        Assert.Equal(player.Id, updated.Id);
        Assert.Equal("Technics", _factory.Context.Manufacturer.Single(m => m.Id == updated.ManufacturerId).Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindOrCreatePlayerAsync_WhitespaceManufacturerOnCreate_DoesNotLinkManufacturer(string? manufacturer)
    {
        var result = await _factory.Service.FindOrCreatePlayerAsync("No Brand Player", manufacturer);

        Assert.Null(result.ManufacturerId);
    }

    [Fact]
    public async Task FindOrCreatePlayerAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreatePlayerAsync(null!));
    }
}
