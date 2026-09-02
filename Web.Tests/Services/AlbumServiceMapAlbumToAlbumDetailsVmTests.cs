using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class AlbumServiceMapAlbumToAlbumDetailsVmTests : IDisposable
{
    private readonly AlbumServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public void MapAlbumToAlbumDetailsVM_FullAlbum_MapsAllFields()
    {
        var album = new Album
        {
            Id = 42,
            Title = "Mapped Title",
            AddedDate = new DateTime(2020, 1, 1),
            UpdateDate = new DateTime(2021, 2, 2),
            Artist = new Artist { Name = "Mapped Artist" },
            Genre = new Genre { Name = "Mapped Genre" },
        };
        var releases = new List<Release> { new() { Id = 1, AlbumId = 42 } };

        var result = _factory.Service.MapAlbumToAlbumDetailsVM(album, releases);

        Assert.Equal(42, result.AlbumId);
        Assert.Equal("Mapped Title", result.Title);
        Assert.Equal("Mapped Artist", result.Artist);
        Assert.Equal("Mapped Genre", result.Genre);
        Assert.Equal(new DateTime(2020, 1, 1), result.AddedDate);
        Assert.Equal(new DateTime(2021, 2, 2), result.UpdateDate);
        Assert.Same(releases, result.Releases);
    }

    [Fact]
    public void MapAlbumToAlbumDetailsVM_NullArtistAndGenre_MapsEmptyStrings()
    {
        var album = new Album
        {
            Id = 1,
            Title = "Title",
            Artist = null!,
            Genre = null!,
        };

        var result = _factory.Service.MapAlbumToAlbumDetailsVM(album);

        Assert.Equal(string.Empty, result.Artist);
        Assert.Equal(string.Empty, result.Genre);
    }

    [Fact]
    public void MapAlbumToAlbumDetailsVM_NullReleases_AllowsNullReleases()
    {
        var album = new Album
        {
            Id = 1,
            Title = "Title",
            Artist = new Artist { Name = "Artist" },
            Genre = new Genre { Name = "Genre" },
        };

        var result = _factory.Service.MapAlbumToAlbumDetailsVM(album, null);

        Assert.Null(result.Releases);
    }

    [Fact]
    public void MapAlbumToAlbumDetailsVM_EmptyReleases_ReturnsEmptyCollection()
    {
        var album = new Album
        {
            Id = 1,
            Title = "Title",
            Artist = new Artist { Name = "Artist" },
            Genre = new Genre { Name = "Genre" },
        };

        var result = _factory.Service.MapAlbumToAlbumDetailsVM(album, []);

        Assert.NotNull(result.Releases);
        Assert.Empty(result.Releases);
    }
}
