using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EntityFindOrCreateReissueTests : IDisposable
{
    private readonly EntityFindOrCreateServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FindOrCreateReissueAsync_NewValue_CreatesReissue()
    {
        var result = await _factory.Service.FindOrCreateReissueAsync(2);

        Assert.True(result.Id > 0);
        Assert.Equal(2, result.Value);
        Assert.Single(_factory.Context.Reissues.Where(r => r.Value == 2));
    }

    [Fact]
    public async Task FindOrCreateReissueAsync_ExistingValue_ReturnsSameEntity()
    {
        var first = await _factory.Service.FindOrCreateReissueAsync(1);
        var second = await _factory.Service.FindOrCreateReissueAsync(1);

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(1, _factory.Context.Reissues.Count(r => r.Value == 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-3)]
    public async Task FindOrCreateReissueAsync_VariousValues_StoresValue(int value)
    {
        var result = await _factory.Service.FindOrCreateReissueAsync(value);

        Assert.Equal(value, result.Value);
    }
}
