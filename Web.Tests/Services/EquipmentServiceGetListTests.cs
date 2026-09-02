using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceGetListTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetListAsync_ReturnsPaginatedResults()
    {
        for (var i = 0; i < 5; i++)
            await _factory.SeedEquipmentAsync(EntityType.Player, $"Player {i + 1}");

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 2, EntityType.Player);

        Assert.Equal(5, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task GetListAsync_SecondPage_ReturnsRemainingItems()
    {
        for (var i = 0; i < 5; i++)
            await _factory.SeedEquipmentAsync(EntityType.Player, $"Player {i + 1}");

        var result = await _factory.Service.GetListAsync(page: 2, pageSize: 2, EntityType.Player);

        Assert.Equal(5, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetListAsync_PageZero_NormalizedToFirstPage()
    {
        await _factory.SeedEquipmentAsync(EntityType.Player, "Player One");
        await _factory.SeedEquipmentAsync(EntityType.Player, "Player Two");

        var result = await _factory.Service.GetListAsync(page: 0, pageSize: 10, EntityType.Player);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetListAsync_EmptyDatabase_ReturnsEmptyItems()
    {
        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10, EntityType.Player);

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalPages);
    }

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task GetListAsync_AllSupportedTypes_ReturnItemsForCorrectTable(EntityType type)
    {
        await _factory.SeedEquipmentAsync(type, $"{type} Model");

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 10, type);

        Assert.Single(result.Items);
        Assert.Equal($"{type} Model", result.Items[0].Name);
    }

    [Fact]
    public async Task GetListAsync_UnsupportedEntityType_ThrowsArgumentOutOfRangeException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _factory.Service.GetListAsync(1, 10, EntityType.AlbumCover));
    }

    [Fact]
    public async Task GetListAsync_PageSizeZero_ReturnsEmptyItems()
    {
        await _factory.SeedEquipmentAsync(EntityType.Player);

        var result = await _factory.Service.GetListAsync(page: 1, pageSize: 0, EntityType.Player);

        Assert.Equal(1, result.TotalItems);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.PageSize);
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
