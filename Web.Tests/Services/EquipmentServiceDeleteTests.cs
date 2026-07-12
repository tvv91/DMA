using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class EquipmentServiceDeleteTests : IDisposable
{
    private readonly EquipmentServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task DeleteEquipmentAsync_ExistingEquipment_ReturnsTrueAndRemovesRow(EntityType type)
    {
        var seeded = await _factory.SeedEquipmentAsync(type);

        var result = await _factory.Service.DeleteEquipmentAsync(seeded.Id, type);

        Assert.True(result);
        Assert.Equal(0, _factory.CountEquipment(type));
    }

    [Theory]
    [MemberData(nameof(SupportedTypes))]
    public async Task DeleteEquipmentAsync_NonExistingId_ReturnsFalse(EntityType type)
    {
        var result = await _factory.Service.DeleteEquipmentAsync(9999, type);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteEquipmentAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await _factory.Service.DeleteEquipmentAsync(id, EntityType.Player);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteEquipmentAsync_UnsupportedEntityType_ThrowsArgumentOutOfRangeException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _factory.Service.DeleteEquipmentAsync(1, EntityType.Country));
    }

    public static IEnumerable<object[]> SupportedTypes =>
        EquipmentServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
