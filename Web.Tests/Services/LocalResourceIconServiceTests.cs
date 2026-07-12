using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("LocalResourceIconService")]
public class LocalResourceIconServiceTests : IDisposable
{
    private const string NoCoverUrl = "/resources/nocover.png";

    private readonly ResourceIconTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Theory]
    [MemberData(nameof(MappedTypesWithExistingFiles))]
    public async Task GetIconUrlAsync_MappedEntityWithExistingFile_ReturnsIconPath(EntityType type, int id)
    {
        _env.CreateIconFile(type, id);

        var result = await _env.Service.GetIconUrlAsync(id, type);

        Assert.Equal(_env.GetExpectedUrl(type, id), result);
        Assert.DoesNotContain('\\', result);
    }

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task GetIconUrlAsync_MappedEntityWithoutFile_ReturnsNoCover(EntityType type)
    {
        var result = await _env.Service.GetIconUrlAsync(999, type);

        Assert.Equal(NoCoverUrl, result);
    }

    [Theory]
    [MemberData(nameof(UnmappedTypes))]
    public async Task GetIconUrlAsync_UnmappedEntity_ReturnsNoCover(EntityType type)
    {
        var result = await _env.Service.GetIconUrlAsync(1, type);

        Assert.Equal(NoCoverUrl, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetIconUrlAsync_NonPositiveIdWithoutFile_ReturnsNoCover(int id)
    {
        var result = await _env.Service.GetIconUrlAsync(id, EntityType.VinylState);

        Assert.Equal(NoCoverUrl, result);
    }

    [Fact]
    public async Task GetIconUrlAsync_NonPositiveIdWithExistingFile_ReturnsIconPath()
    {
        _env.CreateIconFile(EntityType.Bitness, -1);

        var result = await _env.Service.GetIconUrlAsync(-1, EntityType.Bitness);

        Assert.Equal("/resources/bitness/-1.png", result);
    }

    [Fact]
    public async Task GetIconUrlAsync_DigitalFormat_UsesCodecFolderNotDigitalFormatFolder()
    {
        _env.CreateIconFile(EntityType.DigitalFormat, 3);

        var result = await _env.Service.GetIconUrlAsync(3, EntityType.DigitalFormat);

        Assert.Equal("/resources/codec/3.png", result);
        Assert.False(Directory.Exists(Path.Combine(_env.Root, "wwwroot", "resources", "digitalformat")));
    }

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task GetIconUrlAsync_ResultAlwaysStartsWithForwardSlash(EntityType type)
    {
        _env.CreateIconFile(type, 1);

        var result = await _env.Service.GetIconUrlAsync(1, type);

        Assert.StartsWith("/", result);
    }

    [Fact]
    public async Task GetIconUrlAsync_ReturnsCompletedTask()
    {
        var result = await _env.Service.GetIconUrlAsync(1, EntityType.Player);

        Assert.Equal(NoCoverUrl, result);
    }

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task GetIconUrlAsync_CalledTwice_ReturnsSameResult(EntityType type)
    {
        _env.CreateIconFile(type, 7);

        var first = await _env.Service.GetIconUrlAsync(7, type);
        var second = await _env.Service.GetIconUrlAsync(7, type);

        Assert.Equal(first, second);
    }

    [Fact]
    public async Task GetIconUrlAsync_DifferentIdsSameType_ReturnDifferentPathsWhenFilesExist()
    {
        _env.CreateIconFile(EntityType.Sampling, 1);
        _env.CreateIconFile(EntityType.Sampling, 2);

        var first = await _env.Service.GetIconUrlAsync(1, EntityType.Sampling);
        var second = await _env.Service.GetIconUrlAsync(2, EntityType.Sampling);

        Assert.Equal("/resources/sampling/1.png", first);
        Assert.Equal("/resources/sampling/2.png", second);
    }

    [Fact]
    public async Task GetIconUrlAsync_SourceFormatExistingFile_ReturnsSourceFormatPath()
    {
        _env.CreateIconFile(EntityType.SourceFormat, 4);

        var result = await _env.Service.GetIconUrlAsync(4, EntityType.SourceFormat);

        Assert.Equal("/resources/sourceformat/4.png", result);
    }

    [Fact]
    public async Task GetIconUrlAsync_VinylStateExistingFile_ReturnsVinylStatePath()
    {
        _env.CreateIconFile(EntityType.VinylState, 1);

        var result = await _env.Service.GetIconUrlAsync(1, EntityType.VinylState);

        Assert.Equal("/resources/vinylstate/1.png", result);
    }

    [Theory]
    [InlineData(EntityType.Genre)]
    [InlineData(EntityType.Year)]
    [InlineData(EntityType.Reissue)]
    [InlineData(EntityType.Label)]
    [InlineData(EntityType.Storage)]
    [InlineData(EntityType.PlayerManufacturer)]
    [InlineData(EntityType.CartridgeManufacturer)]
    [InlineData(EntityType.AmplifierManufacturer)]
    [InlineData(EntityType.AdcManufacturer)]
    [InlineData(EntityType.WireManufacturer)]
    public async Task GetIconUrlAsync_AdditionalUnmappedEntityTypes_ReturnNoCover(EntityType type)
    {
        var result = await _env.Service.GetIconUrlAsync(1, type);

        Assert.Equal(NoCoverUrl, result);
    }

    public static IEnumerable<object[]> MappedTypes() =>
        ResourceIconTestEnvironment.MappedEntityTypes.Select(t => new object[] { t });

    public static IEnumerable<object[]> UnmappedTypes() =>
        ResourceIconTestEnvironment.UnmappedEntityTypes.Select(t => new object[] { t });

    public static IEnumerable<object[]> MappedTypesWithExistingFiles()
    {
        foreach (var type in ResourceIconTestEnvironment.MappedEntityTypes)
            yield return new object[] { type, 42 };
    }
}
