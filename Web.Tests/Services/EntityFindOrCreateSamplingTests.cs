using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateSamplingTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateSamplingAsync_SeededValue_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateSamplingAsync(96);

        Assert.Equal(96, result.Value);
        Assert.Equal(1, _factory.Context.Samplings.Count(s => s.Value == 96));
    }

    [Fact]
    public async Task FindOrCreateSamplingAsync_SeededFractionalValue_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateSamplingAsync(2.8);

        Assert.Equal(2.8, result.Value);
    }

    [Fact]
    public async Task FindOrCreateSamplingAsync_NewValue_CreatesSampling()
    {
        var result = await _factory.Service.FindOrCreateSamplingAsync(48);

        Assert.True(result.Id > 0);
        Assert.Equal(48, result.Value);
    }

    [Fact]
    public async Task FindOrCreateSamplingAsync_CalledTwice_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateSamplingAsync(48);
        var second = await _factory.Service.FindOrCreateSamplingAsync(48);

        Assert.Equal(first.Id, second.Id);
    }
}
