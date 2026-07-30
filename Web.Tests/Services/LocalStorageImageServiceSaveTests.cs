using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("LocalStorageImageService")]
public class LocalStorageImageServiceSaveTests : IDisposable
{
    private readonly ImageTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task SaveAsync_ValidTempJpg_CopiesToCoverAndDeletesTemp(EntityType type)
    {
        var tempFilename = _env.CreateTempImageFile(".jpg");

        await _env.Service.SaveAsync(10, tempFilename, type);

        Assert.True(File.Exists(_env.GetCoverFilePath(type, 10)));
        Assert.False(File.Exists(_env.TempFilePath(tempFilename)));
    }

    [Fact]
    public async Task SaveAsync_ValidTempPng_CopiesToCover()
    {
        var tempFilename = _env.CreateTempImageFile(".png");

        await _env.Service.SaveAsync(11, tempFilename, EntityType.Player);

        Assert.True(File.Exists(_env.GetCoverFilePath(EntityType.Player, 11)));
    }

    [Fact]
    public async Task SaveAsync_AfterSave_GetUrlReturnsCoverPath()
    {
        var tempFilename = _env.CreateTempImageFile(".jpg");

        await _env.Service.SaveAsync(12, tempFilename, EntityType.Adc);
        var url = await _env.Service.GetUrlAsync(12, EntityType.Adc);

        Assert.Equal("/covers/adc/12.jpg", url);
    }

    [Theory]
    [InlineData("cover.jpg")]
    [InlineData("not-a-guid.png")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SaveAsync_InvalidFilename_DoesNotCreateCover(string filename)
    {
        await _env.Service.SaveAsync(20, filename, EntityType.Player);

        Assert.False(File.Exists(_env.GetCoverFilePath(EntityType.Player, 20)));
    }

    [Theory]
    [InlineData(".gif")]
    [InlineData(".bmp")]
    [InlineData(".webp")]
    public async Task SaveAsync_UnsupportedExtension_DoesNotCreateCover(string extension)
    {
        var guidName = $"{Guid.NewGuid():N}{extension}";
        File.WriteAllBytes(_env.TempFilePath(guidName), [0xFF, 0xD8]);

        await _env.Service.SaveAsync(21, guidName, EntityType.Player);

        Assert.False(File.Exists(_env.GetCoverFilePath(EntityType.Player, 21)));
    }

    [Fact]
    public async Task SaveAsync_PathTraversalFilename_DoesNotCreateCover()
    {
        await _env.Service.SaveAsync(22, "../outside.jpg", EntityType.Player);

        Assert.False(File.Exists(_env.GetCoverFilePath(EntityType.Player, 22)));
    }

    [Fact]
    public async Task SaveAsync_MissingTempFile_DoesNotCreateCover()
    {
        var missingTemp = $"{Guid.NewGuid():N}.jpg";

        await _env.Service.SaveAsync(23, missingTemp, EntityType.Cartridge);

        Assert.False(File.Exists(_env.GetCoverFilePath(EntityType.Cartridge, 23)));
    }

    [Fact]
    public async Task SaveAsync_UnmappedEntity_DoesNotCreateFile()
    {
        var tempFilename = _env.CreateTempImageFile(".jpg");

        await _env.Service.SaveAsync(24, tempFilename, EntityType.VinylState);

        Assert.True(File.Exists(_env.TempFilePath(tempFilename)));
    }

    [Fact]
    public async Task SaveAsync_OverwritesExistingCover()
    {
        _env.CreateCoverFile(EntityType.AlbumCover, 30);
        var originalLength = new FileInfo(_env.GetCoverFilePath(EntityType.AlbumCover, 30)).Length;
        var tempFilename = _env.CreateTempImageFile(".jpg");
        File.WriteAllBytes(_env.TempFilePath(tempFilename), [1, 2, 3, 4, 5, 6, 7, 8]);

        await _env.Service.SaveAsync(30, tempFilename, EntityType.AlbumCover);

        var newLength = new FileInfo(_env.GetCoverFilePath(EntityType.AlbumCover, 30)).Length;
        Assert.Equal(8, newLength);
        Assert.NotEqual(originalLength, newLength);
    }

    [Fact]
    public async Task SaveAsync_CreatesCoverDirectoryWhenMissing()
    {
        var coverDir = _env.GetCoverDirectory(EntityType.Wire);
        if (Directory.Exists(coverDir))
            Directory.Delete(coverDir, recursive: true);

        var tempFilename = _env.CreateTempImageFile(".jpg");
        await _env.Service.SaveAsync(40, tempFilename, EntityType.Wire);

        Assert.True(Directory.Exists(coverDir));
        Assert.True(File.Exists(_env.GetCoverFilePath(EntityType.Wire, 40)));
    }

    public static IEnumerable<object[]> MappedTypes() =>
        ImageTestEnvironment.MappedEntityTypes.Select(t => new object[] { t });
}
