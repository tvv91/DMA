using System.Text.Json;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("StatisticService")]
public class StatisticServiceCountersTests : IDisposable
{
    private readonly StatisticServiceTestFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task ProcessAsync_ComputesAggregateTotals()
    {
        var (albumOne, _, _) = await _factory.SeedAlbumAsync("One");
        var (albumTwo, _, _) = await _factory.SeedAlbumAsync("Two");
        await _factory.SeedReleaseAsync(albumOne.Id, size: 2.0);
        await _factory.SeedReleaseAsync(albumTwo.Id, size: 3.5);
        await _factory.SeedStorageAsync("A");
        await _factory.SeedStorageAsync("B");
        await _factory.SeedPlayerAsync("SL-1200", "Technics");
        await _factory.SeedPlayerAsync("PLX-1000");

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Equal(2, counters.TotalAlbums);
        Assert.Equal(2, counters.TotalReleases);
        Assert.Equal(5.5, counters.TotalSize);
        Assert.Equal(2, counters.StorageCount);
        Assert.Equal(2, counters.TotalArtists);
        Assert.Equal(2, counters.TotalEquipment);
    }

    [Fact]
    public async Task ProcessAsync_GenreAndArtistCounters_IncludeAlbumCounts()
    {
        var sharedArtist = new Artist { Name = "Shared Artist" };
        var otherArtist = new Artist { Name = "Other Artist" };
        var rock = new Genre { Name = "Rock" };
        var jazz = new Genre { Name = "Jazz" };
        _factory.Context.Artists.AddRange(sharedArtist, otherArtist);
        _factory.Context.Genres.AddRange(rock, jazz);
        await _factory.Context.SaveChangesAsync();

        _factory.Context.Albums.AddRange(
            new Album { Title = "A1", ArtistId = sharedArtist.Id, GenreId = rock.Id },
            new Album { Title = "A2", ArtistId = sharedArtist.Id, GenreId = rock.Id },
            new Album { Title = "A3", ArtistId = otherArtist.Id, GenreId = jazz.Id });
        await _factory.Context.SaveChangesAsync();

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.Genre!, x => x.Description == "Rock" && x.Count == 2);
        Assert.Contains(counters.Artist!, x => x.Description == "Shared Artist" && x.Count == 2);
        Assert.Contains(counters.Genre!, x => x.Description == "Jazz" && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_BitnessCounter_UsesBitSuffix()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var formatInfo = new FormatInfo { BitnessId = 2 };
        await _factory.SeedReleaseAsync(album.Id, formatInfo: formatInfo);

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.Bitness!, x => x.Description == "24 bit" && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_SamplingCounter_UsesKhzAndMhzLabels()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, formatInfo: new FormatInfo { SamplingId = 1 });
        await _factory.SeedReleaseAsync(album.Id, formatInfo: new FormatInfo { SamplingId = 4 });

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.Sampling!, x => x.Description == "96 kHz" && x.Count == 1);
        Assert.Contains(counters.Sampling!, x => x.Description is not null && x.Description.EndsWith("MHz") && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_PlayerCounter_IncludesManufacturerName()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var (player, _) = await _factory.SeedPlayerAsync("SL-1200", "Technics");
        var equipmentInfo = new EquipmentInfo { PlayerId = player.Id };
        await _factory.SeedReleaseAsync(album.Id, equipmentInfo: equipmentInfo);

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.Player!, x => x.Description == "Technics SL-1200" && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_YearCountryLabelCounters_CountReleases()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        var year = new Year { Value = 1973 };
        var country = new Country { Name = "USA" };
        var label = new Label { Name = "Columbia" };
        await _factory.SeedReleaseAsync(album.Id, year: year, country: country, label: label);

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.Year!, x => x.Description == "1973" && x.Count == 1);
        Assert.Contains(counters.Country!, x => x.Description == "USA" && x.Count == 1);
        Assert.Contains(counters.Label!, x => x.Description == "Columbia" && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_SourceAndDigitalFormatCounters_CountReleases()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, formatInfo: new FormatInfo { SourceFormatId = 1 });
        await _factory.SeedReleaseAsync(album.Id, formatInfo: new FormatInfo { DigitalFormatId = 1 });

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.SourceFormat!, x => x.Description == "LP 12'' 33RPM" && x.Count == 1);
        Assert.Contains(counters.DigitalFormat!, x => x.Description == "FLAC" && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_VinylStateCounter_CountsReleases()
    {
        var (album, _, _) = await _factory.SeedAlbumAsync();
        await _factory.SeedReleaseAsync(album.Id, formatInfo: new FormatInfo { VinylStateId = 1 });

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.Contains(counters.VinylState!, x => x.Description == "Mint" && x.Count == 1);
    }

    [Fact]
    public async Task ProcessAsync_CountersWithZeroUsage_AreExcluded()
    {
        await _factory.SeedAlbumAsync("Only Album", "Only Artist", "Only Genre");

        var result = await _factory.Service.ProcessAsync();
        var counters = _factory.DeserializeCounters(result.Data);

        Assert.DoesNotContain(counters.Year!, x => x.Count > 0);
        Assert.DoesNotContain(counters.Country!, x => x.Count > 0);
        Assert.DoesNotContain(counters.Player!, x => x.Count > 0);
    }
}
