using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Models;
using Web.ViewModels;

namespace Web.Features.Albums;

internal static class AlbumFeatureHelpers
{
    internal static Task<Album?> GetByIdAsync(Context context, int id) => context.Albums
        .Include(a => a.Artist)
        .Include(a => a.Genre)
        .FirstOrDefaultAsync(a => a.Id == id);

    internal static Task<List<Release>> GetReleasesAsync(Context context, int albumId) => context.Releases
        .Where(r => r.AlbumId == albumId)
        .Include(r => r.Year)
        .Include(r => r.Reissue)
        .Include(r => r.Country)
        .Include(r => r.Label)
        .Include(r => r.Storage)
        .Include(r => r.FormatInfo)
        .Include(r => r.EquipmentInfo)
        .ToListAsync();

    internal static AlbumDetailsViewModel ToDetails(Album album, IEnumerable<Release> releases) => new()
    {
        AlbumId = album.Id,
        Title = album.Title,
        Artist = album.Artist?.Name ?? string.Empty,
        Genre = album.Genre?.Name ?? string.Empty,
        AddedDate = album.AddedDate,
        UpdateDate = album.UpdateDate,
        Releases = releases
    };

    internal static async Task<Artist> FindOrCreateArtistAsync(Context context, string value)
    {
        var name = value.Trim();
        var entity = await context.Artists.FirstOrDefaultAsync(a => a.Name == name);
        if (entity is not null) return entity;
        entity = new Artist { Name = name };
        context.Artists.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    internal static async Task<Genre> FindOrCreateGenreAsync(Context context, string value)
    {
        var name = value.Trim();
        var entity = await context.Genres.FirstOrDefaultAsync(g => g.Name == name);
        if (entity is not null) return entity;
        entity = new Genre { Name = name };
        context.Genres.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    internal static async Task<Album> CreateOrFindAsync(Context context, TimeProvider timeProvider, string title, string artist, string genre)
    {
        var normalizedTitle = title.Trim();
        var normalizedArtist = artist.Trim();
        var album = await context.Albums
            .Include(a => a.Artist)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Artist != null && a.Title == normalizedTitle && a.Artist.Name == normalizedArtist);
        if (album is not null) return album;

        album = new Album
        {
            AddedDate = timeProvider.GetLocalNow().LocalDateTime,
            Title = normalizedTitle,
            Artist = await FindOrCreateArtistAsync(context, normalizedArtist),
            Genre = await FindOrCreateGenreAsync(context, genre.Trim())
        };
        context.Albums.Add(album);
        await context.SaveChangesAsync();
        return album;
    }
}
