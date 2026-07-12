using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Models;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class SearchServiceTestFactory : IDisposable
{
    public const int AutocompleteMaxItems = 10;

    private readonly SqliteConnection _connection;

    public Context Context { get; }
    public SearchService Service { get; }

    public static readonly EntityType[] ManufacturerEntityTypes =
    [
        EntityType.PlayerManufacturer,
        EntityType.CartridgeManufacturer,
        EntityType.AmplifierManufacturer,
        EntityType.AdcManufacturer,
        EntityType.WireManufacturer,
    ];

    public static readonly EntityType[] ListAllOnEmptyQueryTypes =
    [
        EntityType.VinylState,
        EntityType.DigitalFormat,
        EntityType.SourceFormat,
        EntityType.Country,
        EntityType.Label,
        EntityType.Bitness,
        EntityType.Year,
        EntityType.Reissue,
        EntityType.Sampling,
        EntityType.Player,
        EntityType.Cartridge,
        EntityType.Amplifier,
        EntityType.Adc,
        EntityType.Wire,
        EntityType.PlayerManufacturer,
        EntityType.CartridgeManufacturer,
        EntityType.AmplifierManufacturer,
        EntityType.AdcManufacturer,
        EntityType.WireManufacturer,
    ];

    public static readonly EntityType[] EmptyOnBlankQueryTypes =
    [
        EntityType.Artist,
        EntityType.Genre,
        EntityType.Storage,
        EntityType.AlbumCover,
    ];

    public static readonly EntityType[] StringSearchTypes =
    [
        EntityType.Artist,
        EntityType.Genre,
        EntityType.VinylState,
        EntityType.DigitalFormat,
        EntityType.SourceFormat,
        EntityType.Country,
        EntityType.Label,
        EntityType.Storage,
        EntityType.Player,
        EntityType.Cartridge,
        EntityType.Amplifier,
        EntityType.Adc,
        EntityType.Wire,
    ];

    public static readonly EntityType[] NumberSearchTypes =
    [
        EntityType.Year,
        EntityType.Reissue,
        EntityType.Bitness,
    ];

    public SearchServiceTestFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<Context>()
            .UseSqlite(_connection)
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        Service = new SearchService(Context);
    }

    public async Task<Artist> SeedArtistAsync(string name)
    {
        var entity = new Artist { Name = name };
        Context.Artists.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Genre> SeedGenreAsync(string name)
    {
        var entity = new Genre { Name = name };
        Context.Genres.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Year> SeedYearAsync(int value)
    {
        var entity = new Year { Value = value };
        Context.Years.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Reissue> SeedReissueAsync(int value)
    {
        var entity = new Reissue { Value = value };
        Context.Reissues.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Country> SeedCountryAsync(string name)
    {
        var entity = new Country { Name = name };
        Context.Countries.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Label> SeedLabelAsync(string name)
    {
        var entity = new Label { Name = name };
        Context.Labels.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Storage> SeedStorageAsync(string name)
    {
        var entity = new Storage { Name = name };
        Context.Storages.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<VinylState> SeedVinylStateAsync(string name)
    {
        var entity = new VinylState { Name = name };
        Context.VinylStates.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<DigitalFormat> SeedDigitalFormatAsync(string name)
    {
        var entity = new DigitalFormat { Name = name };
        Context.DigitalFormats.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<SourceFormat> SeedSourceFormatAsync(string name)
    {
        var entity = new SourceFormat { Name = name };
        Context.SourceFormats.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Bitness> SeedBitnessAsync(int value)
    {
        var entity = new Bitness { Value = value };
        Context.Bitnesses.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Sampling> SeedSamplingAsync(double value)
    {
        var entity = new Sampling { Value = value };
        Context.Samplings.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Player> SeedPlayerAsync(string name)
    {
        var entity = new Player { Name = name };
        Context.Players.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Cartridge> SeedCartridgeAsync(string name)
    {
        var entity = new Cartridge { Name = name };
        Context.Cartridges.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Amplifier> SeedAmplifierAsync(string name)
    {
        var entity = new Amplifier { Name = name };
        Context.Amplifiers.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Adc> SeedAdcAsync(string name)
    {
        var entity = new Adc { Name = name };
        Context.Adces.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Wire> SeedWireAsync(string name)
    {
        var entity = new Wire { Name = name };
        Context.Wires.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<Manufacturer> SeedManufacturerAsync(string name)
    {
        var entity = new Manufacturer { Name = name };
        Context.Manufacturer.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task SeedPlayersAsync(int count, string prefix = "Player")
    {
        for (var i = 0; i < count; i++)
        {
            Context.Players.Add(new Player { Name = $"{prefix}-{i:D2}" });
        }

        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
