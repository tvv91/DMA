using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateDigitalFormatTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateDigitalFormatAsync_SeededValue_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateDigitalFormatAsync("FLAC");

        Assert.Equal("FLAC", result.Name);
        Assert.Equal(1, _factory.Context.DigitalFormats.Count(f => f.Name == "FLAC"));
    }

    [Fact]
    public async Task FindOrCreateDigitalFormatAsync_NewName_CreatesFormat()
    {
        var result = await _factory.Service.FindOrCreateDigitalFormatAsync("APE");

        Assert.True(result.Id > 0);
        Assert.Equal("APE", result.Name);
    }

    [Fact]
    public async Task FindOrCreateDigitalFormatAsync_TrimmedInput_FindsSeededFormat()
    {
        var result = await _factory.Service.FindOrCreateDigitalFormatAsync("  FLAC  ");

        Assert.Equal("FLAC", result.Name);
    }

    [Fact]
    public async Task FindOrCreateDigitalFormatAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateDigitalFormatAsync(null!));
    }
}
