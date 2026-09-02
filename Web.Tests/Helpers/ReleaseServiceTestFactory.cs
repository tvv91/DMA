using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class ReleaseServiceTestFactory : IDisposable
{
    private static readonly DateTimeOffset FixedUtcNow = new(2024, 9, 10, 12, 0, 0, TimeSpan.Zero);

    public static readonly EntityType[] SupportedEquipmentTypes =
    [
        EntityType.Player,
        EntityType.Cartridge,
        EntityType.Amplifier,
        EntityType.Adc,
        EntityType.Wire,
    ];

    private readonly TestMediatorContext _mediatorContext;

    public Context Context { get; }
    public FakeTimeProvider TimeProvider { get; } = new(FixedUtcNow);
    public ReleaseService Service { get; }

    public DateTime FixedLocalNow => FixedUtcNow.LocalDateTime;
    public DateTime FixedUtcDateTime => FixedUtcNow.UtcDateTime;

    public ReleaseServiceTestFactory()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        _mediatorContext = MediatorTestHelper.Create(Context, TimeProvider);
        Service = new ReleaseService(_mediatorContext);
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
        string? source = null,
        string? discogs = null,
        EquipmentInfo? equipmentInfo = null,
        FormatInfo? formatInfo = null,
        Country? country = null,
        Year? year = null)
    {
        if (country is not null)
        {
            Context.Countries.Add(country);
            await Context.SaveChangesAsync();
        }

        if (year is not null)
        {
            Context.Years.Add(year);
            await Context.SaveChangesAsync();
        }

        if (equipmentInfo is not null && equipmentInfo.Id == 0)
        {
            Context.EquipmentInfos.Add(equipmentInfo);
            await Context.SaveChangesAsync();
        }

        if (formatInfo is not null && formatInfo.Id == 0)
        {
            Context.FormatInfos.Add(formatInfo);
            await Context.SaveChangesAsync();
        }

        var release = new Release
        {
            AlbumId = albumId,
            Source = source,
            Discogs = discogs,
            CountryId = country?.Id,
            YearId = year?.Id,
            EquipmentInfoId = equipmentInfo?.Id,
            FormatInfoId = formatInfo?.Id,
        };

        if (country is not null && country.Id == 0)
            release.Country = country;
        if (year is not null && year.Id == 0)
            release.Year = year;
        if (equipmentInfo is not null && equipmentInfo.Id == 0)
            release.EquipmentInfo = equipmentInfo;
        if (formatInfo is not null && formatInfo.Id == 0)
            release.FormatInfo = formatInfo;

        Context.Releases.Add(release);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        return release;
    }

    public async Task<(Player Player, Release Release)> SeedReleaseWithPlayerAsync(int albumId, string playerName = "SL-1200")
    {
        var player = new Player { Name = playerName };
        Context.Players.Add(player);
        await Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { PlayerId = player.Id, Player = player };
        var release = await SeedReleaseAsync(albumId, equipmentInfo: equipmentInfo);
        return (player, release);
    }

    public async Task<(Cartridge Cartridge, Release Release)> SeedReleaseWithCartridgeAsync(int albumId)
    {
        var cartridge = new Cartridge { Name = "VM95" };
        Context.Cartridges.Add(cartridge);
        await Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { CartridgeId = cartridge.Id, Cartridge = cartridge };
        var release = await SeedReleaseAsync(albumId, equipmentInfo: equipmentInfo);
        return (cartridge, release);
    }

    public async Task<(Amplifier Amplifier, Release Release)> SeedReleaseWithAmplifierAsync(int albumId)
    {
        var amplifier = new Amplifier { Name = "PM-6007" };
        Context.Amplifiers.Add(amplifier);
        await Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { AmplifierId = amplifier.Id, Amplifier = amplifier };
        var release = await SeedReleaseAsync(albumId, equipmentInfo: equipmentInfo);
        return (amplifier, release);
    }

    public async Task<(Adc Adc, Release Release)> SeedReleaseWithAdcAsync(int albumId)
    {
        var adc = new Adc { Name = "ADS-1" };
        Context.Adces.Add(adc);
        await Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { AdcId = adc.Id, Adc = adc };
        var release = await SeedReleaseAsync(albumId, equipmentInfo: equipmentInfo);
        return (adc, release);
    }

    public async Task<(Wire Wire, Release Release)> SeedReleaseWithWireAsync(int albumId)
    {
        var wire = new Wire { Name = "Reference" };
        Context.Wires.Add(wire);
        await Context.SaveChangesAsync();

        var equipmentInfo = new EquipmentInfo { WireId = wire.Id, Wire = wire };
        var release = await SeedReleaseAsync(albumId, equipmentInfo: equipmentInfo);
        return (wire, release);
    }

    public void Dispose()
    {
        _mediatorContext.Dispose();
        Context.Dispose();
    }
}
