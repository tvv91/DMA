using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("LocalStorageImageService")]
public class LocalStorageImageServiceRemoveTests : IDisposable
{
    private readonly ImageTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task RemoveAsync_ExistingCover_DeletesFile(EntityType type)
    {
        _env.CreateCoverFile(type, 5);
        Assert.True(File.Exists(_env.GetCoverFilePath(type, 5)));

        await _env.Service.RemoveAsync(5, type);

        Assert.False(File.Exists(_env.GetCoverFilePath(type, 5)));
    }

    [Fact]
    public async Task RemoveAsync_NonExistingCover_DoesNotThrow()
    {
        await _env.Service.RemoveAsync(999, EntityType.AlbumCover);
    }

    [Fact]
    public async Task RemoveAsync_UnmappedEntity_DoesNotThrow()
    {
        await _env.Service.RemoveAsync(1, EntityType.VinylState);
    }

    public static IEnumerable<object[]> MappedTypes() =>
        ImageTestEnvironment.MappedEntityTypes.Select(t => new object[] { t });
}
