using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateYearTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateYearAsync_NewValue_CreatesYear()
    {
        var result = await _factory.Service.FindOrCreateYearAsync(1984);

        Assert.True(result.Id > 0);
        Assert.Equal(1984, result.Value);
        Assert.Single(_factory.Context.Years.Where(y => y.Value == 1984));
    }

    [Fact]
    public async Task FindOrCreateYearAsync_ExistingValue_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateYearAsync(1973);
        var second = await _factory.Service.FindOrCreateYearAsync(1973);

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(1, _factory.Context.Years.Count(y => y.Value == 1973));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task FindOrCreateYearAsync_VariousValues_CreatesDistinctEntries(int yearValue)
    {
        var result = await _factory.Service.FindOrCreateYearAsync(yearValue);

        Assert.Equal(yearValue, result.Value);
    }
}
