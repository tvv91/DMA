using DMA.Domain.Albums;
using DMA.Domain.Equipment;
using DMA.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace DMA.Infrastructure.Persistence.Repositories;

public class ReferenceDataRepository(DmaDbContext context) : IReferenceDataRepository
{
    private readonly DmaDbContext _context = context;

    public async Task<Year> FindOrCreateYearAsync(int yearValue, CancellationToken cancellationToken = default)
    {
        var year = await _context.Years.FirstOrDefaultAsync(y => y.Value == yearValue, cancellationToken);
        if (year is null)
        {
            year = new Year { Value = yearValue };
            _context.Years.Add(year);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return year;
    }

    public async Task<Reissue> FindOrCreateReissueAsync(int reissueValue, CancellationToken cancellationToken = default)
    {
        var reissue = await _context.Reissues.FirstOrDefaultAsync(r => r.Value == reissueValue, cancellationToken);
        if (reissue is null)
        {
            reissue = new Reissue { Value = reissueValue };
            _context.Reissues.Add(reissue);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return reissue;
    }

    public async Task<Country> FindOrCreateCountryAsync(string countryName, CancellationToken cancellationToken = default)
    {
        var normalizedCountryName = countryName.Trim();
        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == normalizedCountryName, cancellationToken);
        if (country is null)
        {
            country = new Country { Name = countryName };
            _context.Countries.Add(country);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return country;
    }

    public async Task<Label> FindOrCreateLabelAsync(string labelName, CancellationToken cancellationToken = default)
    {
        var normalizedLabelName = labelName.Trim();
        var label = await _context.Labels.FirstOrDefaultAsync(l => l.Name == normalizedLabelName, cancellationToken);
        if (label is null)
        {
            label = new Label { Name = labelName };
            _context.Labels.Add(label);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return label;
    }

    public async Task<Storage> FindOrCreateStorageAsync(string storageData, CancellationToken cancellationToken = default)
    {
        var normalizedStorageData = storageData.Trim();
        var storage = await _context.Storages.FirstOrDefaultAsync(s => s.Name == normalizedStorageData, cancellationToken);
        if (storage is null)
        {
            storage = new Storage { Name = storageData };
            _context.Storages.Add(storage);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return storage;
    }

    public async Task<Bitness> FindOrCreateBitnessAsync(int bitnessValue, CancellationToken cancellationToken = default)
    {
        var bitness = await _context.Bitnesses.FirstOrDefaultAsync(b => b.Value == bitnessValue, cancellationToken);
        if (bitness is null)
        {
            bitness = new Bitness { Value = bitnessValue };
            _context.Bitnesses.Add(bitness);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return bitness;
    }

    public async Task<Sampling> FindOrCreateSamplingAsync(double samplingValue, CancellationToken cancellationToken = default)
    {
        var sampling = await _context.Samplings.FirstOrDefaultAsync(s => s.Value == samplingValue, cancellationToken);
        if (sampling is null)
        {
            sampling = new Sampling { Value = samplingValue };
            _context.Samplings.Add(sampling);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return sampling;
    }

    public async Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName, CancellationToken cancellationToken = default)
    {
        var normalizedFormatName = formatName.Trim();
        var format = await _context.DigitalFormats.FirstOrDefaultAsync(f => f.Name == normalizedFormatName, cancellationToken);
        if (format is null)
        {
            format = new DigitalFormat { Name = formatName };
            _context.DigitalFormats.Add(format);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return format;
    }

    public async Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName, CancellationToken cancellationToken = default)
    {
        var normalizedFormatName = formatName.Trim();
        var format = await _context.SourceFormats.FirstOrDefaultAsync(f => f.Name == normalizedFormatName, cancellationToken);
        if (format is null)
        {
            format = new SourceFormat { Name = formatName };
            _context.SourceFormats.Add(format);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return format;
    }

    public async Task<VinylState> FindOrCreateVinylStateAsync(string stateName, CancellationToken cancellationToken = default)
    {
        var normalizedStateName = stateName.Trim();
        var state = await _context.VinylStates.FirstOrDefaultAsync(v => v.Name == normalizedStateName, cancellationToken);
        if (state is null)
        {
            state = new VinylState { Name = stateName };
            _context.VinylStates.Add(state);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return state;
    }

    public async Task<Artist> FindOrCreateArtistAsync(string artistName, CancellationToken cancellationToken = default)
    {
        var normalizedArtistName = artistName.Trim();
        var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name == normalizedArtistName, cancellationToken);
        if (artist is null)
        {
            artist = new Artist { Name = normalizedArtistName };
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return artist;
    }

    public async Task<Genre> FindOrCreateGenreAsync(string genreName, CancellationToken cancellationToken = default)
    {
        var normalizedGenreName = genreName.Trim();
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == normalizedGenreName, cancellationToken);
        if (genre is null)
        {
            genre = new Genre { Name = normalizedGenreName };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return genre;
    }

    public async Task<Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null, CancellationToken cancellationToken = default)
    {
        var normalizedPlayerName = playerName.Trim();
        var player = await _context.Players
            .Include(p => p.Manufacturer)
            .FirstOrDefaultAsync(p => p.Name == normalizedPlayerName, cancellationToken);

        if (player is null)
        {
            player = new Player { Name = playerName };
            if (!string.IsNullOrWhiteSpace(manufacturerName))
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, cancellationToken);
                player.ManufacturerId = manufacturer?.Id;
            }

            _context.Players.Add(player);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await ApplyManufacturerIfProvidedAsync(player.ManufacturerId, manufacturerName, id => player.ManufacturerId = id, cancellationToken);
        }

        return player;
    }

    public async Task<Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null, CancellationToken cancellationToken = default)
    {
        var normalizedCartridgeName = cartridgeName.Trim();
        var cartridge = await _context.Cartridges
            .Include(c => c.Manufacturer)
            .FirstOrDefaultAsync(c => c.Name == normalizedCartridgeName, cancellationToken);

        if (cartridge is null)
        {
            cartridge = new Cartridge { Name = cartridgeName };
            if (!string.IsNullOrWhiteSpace(manufacturerName))
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, cancellationToken);
                cartridge.ManufacturerId = manufacturer?.Id;
            }

            _context.Cartridges.Add(cartridge);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await ApplyManufacturerIfProvidedAsync(cartridge.ManufacturerId, manufacturerName, id => cartridge.ManufacturerId = id, cancellationToken);
        }

        return cartridge;
    }

    public async Task<Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null, CancellationToken cancellationToken = default)
    {
        var normalizedAmplifierName = amplifierName.Trim();
        var amplifier = await _context.Amplifiers
            .Include(a => a.Manufacturer)
            .FirstOrDefaultAsync(a => a.Name == normalizedAmplifierName, cancellationToken);

        if (amplifier is null)
        {
            amplifier = new Amplifier { Name = amplifierName };
            if (!string.IsNullOrWhiteSpace(manufacturerName))
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, cancellationToken);
                amplifier.ManufacturerId = manufacturer?.Id;
            }

            _context.Amplifiers.Add(amplifier);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await ApplyManufacturerIfProvidedAsync(amplifier.ManufacturerId, manufacturerName, id => amplifier.ManufacturerId = id, cancellationToken);
        }

        return amplifier;
    }

    public async Task<Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null, CancellationToken cancellationToken = default)
    {
        var normalizedAdcName = adcName.Trim();
        var adc = await _context.Adces
            .Include(a => a.Manufacturer)
            .FirstOrDefaultAsync(a => a.Name == normalizedAdcName, cancellationToken);

        if (adc is null)
        {
            adc = new Adc { Name = adcName };
            if (!string.IsNullOrWhiteSpace(manufacturerName))
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, cancellationToken);
                adc.ManufacturerId = manufacturer?.Id;
            }

            _context.Adces.Add(adc);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await ApplyManufacturerIfProvidedAsync(adc.ManufacturerId, manufacturerName, id => adc.ManufacturerId = id, cancellationToken);
        }

        return adc;
    }

    public async Task<Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null, CancellationToken cancellationToken = default)
    {
        var normalizedWireName = wireName.Trim();
        var wire = await _context.Wires
            .Include(w => w.Manufacturer)
            .FirstOrDefaultAsync(w => w.Name == normalizedWireName, cancellationToken);

        if (wire is null)
        {
            wire = new Wire { Name = wireName };
            if (!string.IsNullOrWhiteSpace(manufacturerName))
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, cancellationToken);
                wire.ManufacturerId = manufacturer?.Id;
            }

            _context.Wires.Add(wire);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await ApplyManufacturerIfProvidedAsync(wire.ManufacturerId, manufacturerName, id => wire.ManufacturerId = id, cancellationToken);
        }

        return wire;
    }

    public async Task<Manufacturer?> FindOrCreateManufacturerAsync(string manufacturerName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(manufacturerName))
            return null;

        var normalizedManufacturerName = manufacturerName.Trim();
        var existing = await _context.Manufacturer
            .FirstOrDefaultAsync(m => m.Name == normalizedManufacturerName, cancellationToken);

        if (existing is not null)
            return existing;

        var newManufacturer = new Manufacturer { Name = normalizedManufacturerName };
        _context.Manufacturer.Add(newManufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return newManufacturer;
    }

    private async Task ApplyManufacturerIfProvidedAsync(int? currentManufacturerId, string? manufacturerName, Action<int?> setManufacturerId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(manufacturerName))
            return;

        var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, cancellationToken);
        if (manufacturer is not null && currentManufacturerId != manufacturer.Id)
        {
            setManufacturerId(manufacturer.Id);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
