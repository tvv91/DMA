using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class ReleaseServiceAddTests : IDisposable
{
    private readonly ReleaseServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task AddAsync_NewRelease_SetsAddedDateAndPersists()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var release = new Release { AlbumId = album.Id, Source = "New Source" };

        var result = await _factory.Service.AddAsync(release);

        Assert.True(result.Id > 0);
        Assert.Equal(_factory.FixedLocalNow, result.AddedDate);
        Assert.Equal("New Source", result.Source);
        Assert.Single(_factory.Context.Releases.Where(r => r.AlbumId == album.Id));
    }
}
