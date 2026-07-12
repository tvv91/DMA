using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Models;
using Web.Services;

namespace Web.Tests.Helpers;

[CollectionDefinition("StatisticService", DisableParallelization = true)]
public sealed class StatisticServiceCollection;

internal static class StatisticServiceTestState
{
    public static void ResetLastRefreshAttempt()
    {
        var field = typeof(StatisticService).GetField(
            "_lastRefreshAttempt",
            BindingFlags.Static | BindingFlags.NonPublic);

        field?.SetValue(null, null);
    }

    public static void SetLastRefreshAttempt(DateTime value)
    {
        var field = typeof(StatisticService).GetField(
            "_lastRefreshAttempt",
            BindingFlags.Static | BindingFlags.NonPublic);

        field?.SetValue(null, value);
    }
}

internal sealed class StatisticServiceTestFactory : IDisposable
{
    public const int TopStatisticsItems = 10;

    private static readonly DateTimeOffset DefaultUtcNow = new(2024, 9, 10, 12, 0, 0, TimeSpan.Zero);

    public Context Context { get; }
    public FakeTimeProvider TimeProvider { get; }
    public StatisticService Service { get; }

    public DateTime UtcNow => TimeProvider.GetUtcNow().UtcDateTime;

    public StatisticServiceTestFactory(DateTimeOffset? utcNow = null)
    {
        StatisticServiceTestState.ResetLastRefreshAttempt();

        TimeProvider = new FakeTimeProvider(utcNow ?? DefaultUtcNow);

        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        Service = new StatisticService(Context, TimeProvider);
    }

    public async Task<Statistic> SeedExistingStatisticAsync(
        DateTime lastUpdate,
        StatisticCounters? counters = null)
    {
        var data = JsonSerializer.Serialize(counters ?? new StatisticCounters { TotalAlbums = 1 });
        var stat = new Statistic { Data = data, LastUpdate = lastUpdate };
        Context.Statistics.Add(stat);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        return stat;
    }

    public async Task<(Album Album, Artist Artist, Genre Genre)> SeedAlbumAsync(
        string title = "Test Album",
        string artistName = "Test Artist",
        string genreName = "Rock")
    {
        var artist = new Artist { Name = artistName };
        var genre = new Genre { Name = genreName };
        Context.Artists.Add(artist);
        Context.Genres.Add(genre);
        await Context.SaveChangesAsync();

        var album = new Album
        {
            Title = title,
            ArtistId = artist.Id,
            GenreId = genre.Id,
        };
        Context.Albums.Add(album);
        await Context.SaveChangesAsync();
        album.Artist = artist;
        album.Genre = genre;
        return (album, artist, genre);
    }

    public async Task<Release> SeedReleaseAsync(
        int albumId,
        double? size = null,
        FormatInfo? formatInfo = null,
        EquipmentInfo? equipmentInfo = null,
        Country? country = null,
        Year? year = null,
        Label? label = null)
    {
        if (country is not null && country.Id == 0)
        {
            Context.Countries.Add(country);
            await Context.SaveChangesAsync();
        }

        if (year is not null && year.Id == 0)
        {
            Context.Years.Add(year);
            await Context.SaveChangesAsync();
        }

        if (label is not null && label.Id == 0)
        {
            Context.Labels.Add(label);
            await Context.SaveChangesAsync();
        }

        if (formatInfo is not null && formatInfo.Id == 0)
        {
            Context.FormatInfos.Add(formatInfo);
            await Context.SaveChangesAsync();
        }

        if (equipmentInfo is not null && equipmentInfo.Id == 0)
        {
            Context.EquipmentInfos.Add(equipmentInfo);
            await Context.SaveChangesAsync();
        }

        var release = new Release
        {
            AlbumId = albumId,
            Size = size,
            CountryId = country?.Id,
            YearId = year?.Id,
            LabelId = label?.Id,
            FormatInfoId = formatInfo?.Id,
            EquipmentInfoId = equipmentInfo?.Id,
        };

        Context.Releases.Add(release);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        return release;
    }

    public async Task<Storage> SeedStorageAsync(string name)
    {
        var storage = new Storage { Name = name };
        Context.Storages.Add(storage);
        await Context.SaveChangesAsync();
        return storage;
    }

    public async Task<(Player Player, Manufacturer? Manufacturer)> SeedPlayerAsync(
        string name,
        string? manufacturerName = null)
    {
        Manufacturer? manufacturer = null;
        if (manufacturerName is not null)
        {
            manufacturer = new Manufacturer { Name = manufacturerName };
            Context.Manufacturer.Add(manufacturer);
            await Context.SaveChangesAsync();
        }

        var player = new Player { Name = name, ManufacturerId = manufacturer?.Id };
        Context.Players.Add(player);
        await Context.SaveChangesAsync();
        return (player, manufacturer);
    }

    public async Task SeedGenresWithAlbumCountsAsync(int genreCount)
    {
        for (var i = 0; i < genreCount; i++)
        {
            await SeedAlbumAsync($"Album {i}", $"Artist {i}", $"Genre {i:D2}");
        }
    }

    public StatisticCounters DeserializeCounters(string data) =>
        JsonSerializer.Deserialize<StatisticCounters>(data)!;

    public void Dispose()
    {
        StatisticServiceTestState.ResetLastRefreshAttempt();
        Context.Dispose();
    }
}
