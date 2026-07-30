using Web.Enums;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("LocalStorageImageService")]
public class LocalStorageImageServiceGetUrlTests : IDisposable
{
    private const string NoCoverWithSlash = "/resources/nocover.png";
    private const string NoCoverWithoutSlash = "resources/nocover.png";

    private readonly ImageTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Theory]
    [MemberData(nameof(MappedTypesWithIds))]
    public async Task GetUrlAsync_MappedEntityWithExistingFile_ReturnsCoverPath(EntityType type, int id)
    {
        _env.CreateCoverFile(type, id);

        var result = await _env.Service.GetUrlAsync(id, type);

        Assert.Equal(_env.GetExpectedCoverUrl(type, id), result);
        Assert.DoesNotContain('\\', result);
    }

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task GetUrlAsync_MappedEntityWithoutFile_ReturnsNoCoverWithLeadingSlash(EntityType type)
    {
        var result = await _env.Service.GetUrlAsync(999, type);

        Assert.Equal(NoCoverWithSlash, result);
    }

    [Theory]
    [MemberData(nameof(UnmappedTypes))]
    public async Task GetUrlAsync_UnmappedEntity_ReturnsNoCoverWithoutLeadingSlash(EntityType type)
    {
        var result = await _env.Service.GetUrlAsync(1, type);

        Assert.Equal(NoCoverWithoutSlash, result);
    }

    [Fact]
    public async Task GetUrlAsync_Amplifier_UsesAmpFolderNotAmplifierFolder()
    {
        _env.CreateCoverFile(EntityType.Amplifier, 3);

        var result = await _env.Service.GetUrlAsync(3, EntityType.Amplifier);

        Assert.Equal("/covers/amp/3.jpg", result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetUrlAsync_NonPositiveIdWithoutFile_ReturnsNoCoverWithLeadingSlash(int id)
    {
        var result = await _env.Service.GetUrlAsync(id, EntityType.Player);

        Assert.Equal(NoCoverWithSlash, result);
    }

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task GetUrlAsync_ResultForMappedTypeWithFile_StartsWithForwardSlash(EntityType type)
    {
        _env.CreateCoverFile(type, 1);

        var result = await _env.Service.GetUrlAsync(1, type);

        Assert.StartsWith("/", result);
    }

    public static IEnumerable<object[]> MappedTypes() =>
        ImageTestEnvironment.MappedEntityTypes.Select(t => new object[] { t });

    public static IEnumerable<object[]> UnmappedTypes() =>
        ImageTestEnvironment.UnmappedEntityTypes.Select(t => new object[] { t });

    public static IEnumerable<object[]> MappedTypesWithIds() =>
        ImageTestEnvironment.MappedEntityTypes.Select(t => new object[] { t, 42 });
}
