using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateManufacturerTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindOrCreateManufacturerAsync_NullOrWhitespace_ReturnsNull(string? name)
    {
        var result = await _factory.Service.FindOrCreateManufacturerAsync(name!);

        Assert.Null(result);
        Assert.Empty(_factory.Context.Manufacturer);
    }

    [Fact]
    public async Task FindOrCreateManufacturerAsync_NewName_CreatesWithTrimmedName()
    {
        var result = await _factory.Service.FindOrCreateManufacturerAsync("  Technics  ");

        Assert.NotNull(result);
        Assert.Equal("Technics", result!.Name);
    }

    [Fact]
    public async Task FindOrCreateManufacturerAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateManufacturerAsync("Rega");
        var second = await _factory.Service.FindOrCreateManufacturerAsync("Rega");

        Assert.Equal(first!.Id, second!.Id);
        Assert.Single(_factory.Context.Manufacturer);
    }

    [Fact]
    public async Task FindOrCreateManufacturerAsync_TrimmedInput_FindsExisting()
    {
        var first = await _factory.Service.FindOrCreateManufacturerAsync("Audio-Technica");

        var second = await _factory.Service.FindOrCreateManufacturerAsync("  Audio-Technica  ");

        Assert.Equal(first!.Id, second!.Id);
    }
}
