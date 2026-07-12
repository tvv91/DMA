using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateBitnessTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateBitnessAsync_SeededValue_ReturnsExisting()
    {
        var result = await _factory.Service.FindOrCreateBitnessAsync(24);

        Assert.Equal(24, result.Value);
        Assert.Equal(1, _factory.Context.Bitnesses.Count(b => b.Value == 24));
    }

    [Fact]
    public async Task FindOrCreateBitnessAsync_NewValue_CreatesBitness()
    {
        var result = await _factory.Service.FindOrCreateBitnessAsync(16);

        Assert.True(result.Id > 0);
        Assert.Equal(16, result.Value);
    }

    [Fact]
    public async Task FindOrCreateBitnessAsync_CalledTwice_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateBitnessAsync(16);
        var second = await _factory.Service.FindOrCreateBitnessAsync(16);

        Assert.Equal(first.Id, second.Id);
    }
}
