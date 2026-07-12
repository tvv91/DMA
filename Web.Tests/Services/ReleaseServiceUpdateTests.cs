using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceUpdateTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task UpdateAsync_ExistingRelease_UpdatesScalarFields()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var seeded = await _factory.SeedReleaseAsync(album.Id, source: "Old", discogs: "1");
        var update = new Release
        {
            Id = seeded.Id,
            AlbumId = album.Id,
            Source = "New",
            Discogs = "2",
            IsFirstPress = true,
            Size = 42.5,
        };

        var result = await _factory.Service.UpdateAsync(update);

        Assert.Equal("New", result.Source);
        Assert.Equal("2", result.Discogs);
        Assert.True(result.IsFirstPress);
        Assert.Equal(42.5, result.Size);
        Assert.Equal(_factory.FixedUtcDateTime, result.UpdateDate);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingRelease_ThrowsKeyNotFoundException()
    {
        var update = new Release { Id = 999, AlbumId = 1, Source = "X" };

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _factory.Service.UpdateAsync(update));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_AddsFormatInfoWhenMissing()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var seeded = await _factory.SeedReleaseAsync(album.Id);
        var update = new Release
        {
            Id = seeded.Id,
            AlbumId = album.Id,
            FormatInfo = new FormatInfo { BitnessId = 2, DigitalFormatId = 1 },
        };

        var result = await _factory.Service.UpdateAsync(update);

        Assert.NotNull(result.FormatInfo);
        Assert.Equal(2, result.FormatInfo!.BitnessId);
        Assert.Equal(1, result.FormatInfo.DigitalFormatId);
    }

    [Fact]
    public async Task UpdateAsync_AddsEquipmentInfoWhenMissing()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var player = new Player { Name = "Updated Player" };
        _factory.Context.Players.Add(player);
        await _factory.Context.SaveChangesAsync();

        var seeded = await _factory.SeedReleaseAsync(album.Id);
        var update = new Release
        {
            Id = seeded.Id,
            AlbumId = album.Id,
            EquipmentInfo = new EquipmentInfo { PlayerId = player.Id },
        };

        var result = await _factory.Service.UpdateAsync(update);

        Assert.NotNull(result.EquipmentInfo);
        Assert.Equal(player.Id, result.EquipmentInfo!.PlayerId);
    }

    [Fact]
    public async Task UpdateAsync_ExistingFormatInfo_UpdatesFormatFields()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var formatInfo = new FormatInfo { BitnessId = 1 };
        var seeded = await _factory.SeedReleaseAsync(album.Id, formatInfo: formatInfo);
        var update = new Release
        {
            Id = seeded.Id,
            AlbumId = album.Id,
            FormatInfo = new FormatInfo { BitnessId = 3, VinylStateId = 2 },
        };

        var result = await _factory.Service.UpdateAsync(update);

        Assert.Equal(3, result.FormatInfo!.BitnessId);
        Assert.Equal(2, result.FormatInfo.VinylStateId);
    }

    [Fact]
    public async Task UpdateAsync_ExistingEquipmentInfo_UpdatesEquipmentFields()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var player = new Player { Name = "P1" };
        var cartridge = new Cartridge { Name = "C1" };
        _factory.Context.Players.Add(player);
        _factory.Context.Cartridges.Add(cartridge);
        await _factory.Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
        var seeded = await _factory.SeedReleaseAsync(album.Id, equipmentInfo: equipmentInfo);
        var update = new Release
        {
            Id = seeded.Id,
            AlbumId = album.Id,
            EquipmentInfo = new EquipmentInfo { CartridgeId = cartridge.Id, WireId = null },
        };

        var result = await _factory.Service.UpdateAsync(update);

        Assert.Equal(cartridge.Id, result.EquipmentInfo!.CartridgeId);
        Assert.Null(result.EquipmentInfo.PlayerId);
    }
}
