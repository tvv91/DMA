using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("LocalStorageImageService")]
public class LocalStorageImageServiceCrossTests : IDisposable
{
    private readonly ImageTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task SaveThenRemoveThenGetUrl_ReturnsNoCover()
    {
        var tempFilename = _env.CreateTempImageFile(".jpg");
        await _env.Service.SaveAsync(50, tempFilename, EntityType.Player);
        await _env.Service.RemoveAsync(50, EntityType.Player);

        var url = await _env.Service.GetUrlAsync(50, EntityType.Player);

        Assert.Equal("/resources/nocover.png", url);
    }

    [Theory]
    [MemberData(nameof(MappedTypes))]
    public async Task GetUrlAsync_CalledTwiceWithExistingFile_ReturnsSameUrl(EntityType type)
    {
        _env.CreateCoverFile(type, 60);

        var first = await _env.Service.GetUrlAsync(60, type);
        var second = await _env.Service.GetUrlAsync(60, type);

        Assert.Equal(first, second);
    }

    public static IEnumerable<object[]> MappedTypes() =>
        ImageTestEnvironment.MappedEntityTypes.Select(t => new object[] { t });
}
