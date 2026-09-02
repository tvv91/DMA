using Microsoft.EntityFrameworkCore;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateCrossEntityTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreatePlayerAndCartridge_SameManufacturer_ReusesManufacturerRow()
    {
        var player = await _factory.Service.FindOrCreatePlayerAsync("SL-1200", "Technics");
        var cartridge = await _factory.Service.FindOrCreateCartridgeAsync("AT-VM95", "Technics");

        Assert.NotNull(player.ManufacturerId);
        Assert.Equal(player.ManufacturerId, cartridge.ManufacturerId);
        Assert.Single(_factory.Context.Manufacturer);
    }

    [Fact]
    public async Task FindOrCreateCountryAsync_CaseMismatch_CreatesSeparateEntries()
    {
        var lower = await _factory.Service.FindOrCreateCountryAsync("ukraine");
        var upper = await _factory.Service.FindOrCreateCountryAsync("Ukraine");

        Assert.NotEqual(lower.Id, upper.Id);
        Assert.Equal(2, _factory.Context.Countries.Count());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindOrCreateLabelAsync_EmptyOrWhitespace_CreatesEntry(string name)
    {
        var result = await _factory.Service.FindOrCreateLabelAsync(name);

        Assert.Equal(name, result.Name);
    }

    [Fact]
    public async Task FindOrCreateDigitalFormatAsync_CalledTwice_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateDigitalFormatAsync("APE");
        var second = await _factory.Service.FindOrCreateDigitalFormatAsync("APE");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateBitnessAsync_SeededValueOne_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateBitnessAsync(1);

        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task FindOrCreateCartridgeAsync_TrimmedName_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreateCartridgeAsync("VM95");

        var second = await _factory.Service.FindOrCreateCartridgeAsync("  VM95  ");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateAmplifierAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateAmplifierAsync(null!));
    }

    [Fact]
    public async Task FindOrCreateAdcAsync_TrimmedName_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreateAdcAsync("UA-2G");

        var second = await _factory.Service.FindOrCreateAdcAsync("  UA-2G  ");

        Assert.Equal(first.Id, second.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindOrCreateCartridgeAsync_WhitespaceManufacturerOnExisting_KeepsManufacturer(string? manufacturer)
    {
        var original = await _factory.Service.FindOrCreateCartridgeAsync("VM95", "Ortofon");

        var updated = await _factory.Service.FindOrCreateCartridgeAsync("VM95", manufacturer);

        Assert.Equal(original.ManufacturerId, updated.ManufacturerId);
    }
}
