using Microsoft.EntityFrameworkCore;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateSourceFormatTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateSourceFormatAsync_SeededValue_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateSourceFormatAsync("LP 12'' 33RPM");

        Assert.Equal("LP 12'' 33RPM", result.Name);
    }

    [Fact]
    public async Task FindOrCreateSourceFormatAsync_NewName_CreatesFormat()
    {
        var result = await _factory.Service.FindOrCreateSourceFormatAsync("Custom Format");

        Assert.True(result.Id > 0);
        Assert.Equal("Custom Format", result.Name);
    }

    [Fact]
    public async Task FindOrCreateSourceFormatAsync_ExistingName_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateSourceFormatAsync("Custom Format");
        var second = await _factory.Service.FindOrCreateSourceFormatAsync("Custom Format");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task FindOrCreateSourceFormatAsync_NullName_ThrowsNullReferenceException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _factory.Service.FindOrCreateSourceFormatAsync(null!));
    }
}
