using Microsoft.EntityFrameworkCore;
using Moq;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class AlbumServiceTestFactory : IDisposable
{
    private static readonly DateTimeOffset FixedUtcNow = new(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

    public Context Context { get; }
    public Mock<IReleaseService> ReleaseServiceMock { get; } = new();
    public Mock<IImageService> ImageServiceMock { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(FixedUtcNow);
    public AlbumService Service { get; }

    public DateTime FixedLocalNow => FixedUtcNow.LocalDateTime;
    public DateTime FixedUtcDateTime => FixedUtcNow.UtcDateTime;

    public AlbumServiceTestFactory()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();

        ReleaseServiceMock
            .Setup(s => s.GetByAlbumIdAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        ImageServiceMock
            .Setup(s => s.GetUrlAsync(It.IsAny<int>(), It.IsAny<EntityType>()))
            .ReturnsAsync("/images/nocover.png");

        Service = new AlbumService(
            ReleaseServiceMock.Object,
            ImageServiceMock.Object,
            Context,
            TimeProvider);
    }

    public async Task<(Album Album, Artist Artist, Genre Genre)> SeedAlbumAsync(
        string title = "Test Album",
        string artistName = "Test Artist",
        string genreName = "Rock",
        int? releaseYear = null,
        DateTime? addedDate = null)
    {
        var artist = await FindOrAddArtistAsync(artistName);
        var genre = await FindOrAddGenreAsync(genreName);

        var album = new Album
        {
            Title = title,
            ArtistId = artist.Id,
            GenreId = genre.Id,
            AddedDate = addedDate ?? FixedLocalNow,
        };

        Context.Albums.Add(album);
        await Context.SaveChangesAsync();

        album.Artist = artist;
        album.Genre = genre;

        if (releaseYear.HasValue)
        {
            await AddReleaseWithYearAsync(album.Id, releaseYear.Value);
        }

        return (album, artist, genre);
    }

    public async Task<Release> AddReleaseWithYearAsync(int albumId, int yearValue)
    {
        var year = Context.Years.Local.FirstOrDefault(y => y.Value == yearValue);
        if (year is null)
        {
            year = new Year { Value = yearValue };
            Context.Years.Add(year);
            await Context.SaveChangesAsync();
        }

        var release = new Release
        {
            AlbumId = albumId,
            YearId = year.Id,
        };

        Context.Releases.Add(release);
        await Context.SaveChangesAsync();
        return release;
    }

    public async Task<Artist> FindOrAddArtistAsync(string name)
    {
        var existing = Context.Artists.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            return existing;

        var artist = new Artist { Name = name };
        Context.Artists.Add(artist);
        await Context.SaveChangesAsync();
        return artist;
    }

    public async Task<Genre> FindOrAddGenreAsync(string name)
    {
        var existing = Context.Genres.FirstOrDefault(g => g.Name == name);
        if (existing is not null)
            return existing;

        var genre = new Genre { Name = name };
        Context.Genres.Add(genre);
        await Context.SaveChangesAsync();
        return genre;
    }

    public void Dispose() => Context.Dispose();
}
