using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceGetAlbumsByEquipmentTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_Player_ReturnsMatchingAlbums()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Player Album");
        var (player, _) = await _factory.SeedReleaseWithPlayerAsync(album.Id);

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, player.Id, 1, 10);

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("Player Album", result.Items[0].Title);
        Assert.NotNull(result.Items[0].Artist);
    }

    [Theory]
    [MemberData(nameof(EquipmentTypes))]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_AllSupportedTypes_ReturnMatchingAlbum(EntityType type)
    {
        var (album, _, _) = await _factory.SeedAlbumAsync($"{type} Album");
        var equipmentId = await SeedForEquipmentTypeAsync(type, album.Id);

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(type, equipmentId, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal($"{type} Album", result.Items[0].Title);
    }

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_UnsupportedType_ReturnsEmpty()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var (player, _) = await _factory.SeedReleaseWithPlayerAsync(album.Id);

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.AlbumCover, player.Id, 1, 10);

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_DuplicateAlbums_ReturnsDistinctAlbum()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync("Shared Album");
        var player = new Player { Name = "Shared Player" };
        _factory.Context.Players.Add(player);
        await _factory.Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
        await _factory.SeedReleaseAsync(album.Id, source: "A", equipmentInfo: equipmentInfo);
        await _factory.SeedReleaseAsync(album.Id, source: "B", equipmentInfo: equipmentInfo);

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, player.Id, 1, 10);

        Assert.Equal(1, result.TotalItems);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_OrdersByArtistThenTitle()
    {
        var (albumZ, _, _) = await _factory.SeedAlbumAsync("Zulu", "Zebra Artist");
        var (albumA, _, _) = await _factory.SeedAlbumAsync("Alpha", "Alpha Artist");
        var player = new Player { Name = "Sorter" };
        _factory.Context.Players.Add(player);
        await _factory.Context.SaveChangesAsync();
        var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
        await _factory.SeedReleaseAsync(albumZ.Id, equipmentInfo: equipmentInfo);
        await _factory.SeedReleaseAsync(albumA.Id, equipmentInfo: equipmentInfo);

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, player.Id, 1, 10);

        Assert.Equal("Alpha", result.Items[0].Title);
        Assert.Equal("Zulu", result.Items[1].Title);
    }

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_PageZero_NormalizedToFirstPage()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var (player, _) = await _factory.SeedReleaseWithPlayerAsync(album.Id);

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, player.Id, 0, 10);

        Assert.Equal(1, result.CurrentPage);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_SecondPage_ReturnsRemainingAlbums()
    {
        var player = new Player { Name = "Pager" };
        _factory.Context.Players.Add(player);
        await _factory.Context.SaveChangesAsync();
        var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };

        for (var i = 0; i < 3; i++)
        {
            var (album, _, _) = await _factory.SeedAlbumAsync($"Album {i}", $"Artist {i}");
            await _factory.SeedReleaseAsync(album.Id, equipmentInfo: equipmentInfo);
        }

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, player.Id, 2, 2);

        Assert.Equal(3, result.TotalItems);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAlbumsReleasedByEquipmentPagedAsync_ReleaseWithoutEquipment_Excluded()
    {
        var (albumWith, _, _) = await _factory.SeedAlbumAsync("With Equipment");
        var (albumWithout, _, _) = await _factory.SeedAlbumAsync("Without Equipment");
        var (player, _) = await _factory.SeedReleaseWithPlayerAsync(albumWith.Id);
        await _factory.SeedReleaseAsync(albumWithout.Id, source: "No gear");

        var result = await _factory.Service.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, player.Id, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal("With Equipment", result.Items[0].Title);
    }

    private async Task<int> SeedForEquipmentTypeAsync(EntityType type, int albumId) => type switch
    {
        EntityType.Player => (await _factory.SeedReleaseWithPlayerAsync(albumId)).Player.Id,
        EntityType.Cartridge => (await _factory.SeedReleaseWithCartridgeAsync(albumId)).Cartridge.Id,
        EntityType.Amplifier => (await _factory.SeedReleaseWithAmplifierAsync(albumId)).Amplifier.Id,
        EntityType.Adc => (await _factory.SeedReleaseWithAdcAsync(albumId)).Adc.Id,
        EntityType.Wire => (await _factory.SeedReleaseWithWireAsync(albumId)).Wire.Id,
        _ => throw new InvalidOperationException(),
    };

    public static IEnumerable<object[]> EquipmentTypes() =>
        ReleaseServiceTestFactory.SupportedEquipmentTypes.Select(t => new object[] { t });
}
