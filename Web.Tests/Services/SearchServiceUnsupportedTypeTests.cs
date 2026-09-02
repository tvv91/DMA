using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class SearchServiceUnsupportedTypeTests : IDisposable
{
    private readonly SearchServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Theory]
    [InlineData("query")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchAsync_AlbumCover_ReturnsEmpty(string value)
    {
        var result = await _factory.Service.SearchAsync(EntityType.AlbumCover, value);

        Assert.Empty(result);
    }
}
