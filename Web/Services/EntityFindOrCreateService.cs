using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class EntityFindOrCreateService(Context context) : IEntityFindOrCreateService
    {
        private readonly Context _context = context;

        public async Task<Year> FindOrCreateYearAsync(int yearValue)
        {
            var year = await _context.Years.FirstOrDefaultAsync(y => y.Value == yearValue);
            if (year is null)
            {
                year = new Year { Value = yearValue };
                _context.Years.Add(year);
                await _context.SaveChangesAsync();
            }
            return year;
        }

        public async Task<Reissue> FindOrCreateReissueAsync(int reissueValue)
        {
            var reissue = await _context.Reissues.FirstOrDefaultAsync(r => r.Value == reissueValue);
            if (reissue is null)
            {
                reissue = new Reissue { Value = reissueValue };
                _context.Reissues.Add(reissue);
                await _context.SaveChangesAsync();
            }
            return reissue;
        }

        public async Task<Country> FindOrCreateCountryAsync(string countryName)
        {
            var normalizedCountryName = countryName.Trim();

            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == normalizedCountryName);
            if (country is null)
            {
                country = new Country { Name = countryName };
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
            }
            return country;
        }

        public async Task<Label> FindOrCreateLabelAsync(string labelName)
        {
            var normalizedLabelName = labelName.Trim();

            var label = await _context.Labels.FirstOrDefaultAsync(l => l.Name == normalizedLabelName);
            if (label is null)
            {
                label = new Label { Name = labelName };
                _context.Labels.Add(label);
                await _context.SaveChangesAsync();
            }
            return label;
        }

        public async Task<Storage> FindOrCreateStorageAsync(string storageData)
        {
            var normalizedStorageData = storageData.Trim();

            var storage = await _context.Storages.FirstOrDefaultAsync(s => s.Data == normalizedStorageData);
            if (storage is null)
            {
                storage = new Storage { Data = storageData };
                _context.Storages.Add(storage);
                await _context.SaveChangesAsync();
            }
            return storage;
        }

        public async Task<Bitness> FindOrCreateBitnessAsync(int bitnessValue)
        {
            var bitness = await _context.Bitnesses.FirstOrDefaultAsync(b => b.Value == bitnessValue);
            if (bitness is null)
            {
                bitness = new Bitness { Value = bitnessValue };
                _context.Bitnesses.Add(bitness);
                await _context.SaveChangesAsync();
            }
            return bitness;
        }

        public async Task<Sampling> FindOrCreateSamplingAsync(double samplingValue)
        {
            var sampling = await _context.Samplings.FirstOrDefaultAsync(s => s.Value == samplingValue);
            if (sampling is null)
            {
                sampling = new Sampling { Value = samplingValue };
                _context.Samplings.Add(sampling);
                await _context.SaveChangesAsync();
            }
            return sampling;
        }

        public async Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName)
        {
            var normalizedFormatName = formatName.Trim();

            var format = await _context.DigitalFormats.FirstOrDefaultAsync(f => f.Name == normalizedFormatName);
            if (format is null)
            {
                format = new DigitalFormat { Name = formatName };
                _context.DigitalFormats.Add(format);
                await _context.SaveChangesAsync();
            }
            return format;
        }

        public async Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName)
        {
            var normalizedFormatName = formatName.Trim();

            var format = await _context.SourceFormats.FirstOrDefaultAsync(f => f.Name == normalizedFormatName);
            if (format is null)
            {
                format = new SourceFormat { Name = formatName };
                _context.SourceFormats.Add(format);
                await _context.SaveChangesAsync();
            }
            return format;
        }

        public async Task<VinylState> FindOrCreateVinylStateAsync(string stateName)
        {
            var normalizedStateName = stateName.Trim();

            var state = await _context.VinylStates.FirstOrDefaultAsync(v => v.Name == normalizedStateName);
            if (state is null)
            {
                state = new VinylState { Name = stateName };
                _context.VinylStates.Add(state);
                await _context.SaveChangesAsync();
            }
            return state;
        }

        public async Task<Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null)
        {
            var normalizedPlayerName = playerName.Trim();

            var player = await _context.Players
                .Include(p => p.Manufacturer)
                .FirstOrDefaultAsync(p => p.Name == normalizedPlayerName);
            
            if (player is null)
            {
                player = new Player { Name = playerName };
                
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.PlayerManufacturer);
                    player.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Players.Add(player);
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && player.ManufacturerId is null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.PlayerManufacturer);
                if (manufacturer is not null)
                {
                    player.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return player;
        }

        public async Task<Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null)
        {
            var normalizedCartridgeName = cartridgeName.Trim();

            var cartridge = await _context.Cartridges
                .Include(c => c.Manufacturer)
                .FirstOrDefaultAsync(c => c.Name == normalizedCartridgeName);
            
            if (cartridge is null)
            {
                cartridge = new Cartridge { Name = cartridgeName };
                
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.CartridgeManufacturer);
                    cartridge.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Cartridges.Add(cartridge);
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && cartridge.ManufacturerId is null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.CartridgeManufacturer);
                if (manufacturer is not null)
                {
                    cartridge.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return cartridge;
        }

        public async Task<Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null)
        {
            var normalizedAmplifierName = amplifierName.Trim();

            var amplifier = await _context.Amplifiers
                .Include(a => a.Manufacturer)
                .FirstOrDefaultAsync(a => a.Name == normalizedAmplifierName);
            
            if (amplifier is null)
            {
                amplifier = new Amplifier { Name = amplifierName };
                
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AmplifierManufacturer);
                    amplifier.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Amplifiers.Add(amplifier);
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && amplifier.ManufacturerId is null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AmplifierManufacturer);
                if (manufacturer is not null)
                {
                    amplifier.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return amplifier;
        }

        public async Task<Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null)
        {
            var normalizedAdcName = adcName.Trim();

            var adc = await _context.Adces
                .Include(a => a.Manufacturer)
                .FirstOrDefaultAsync(a => a.Name == normalizedAdcName);
            
            if (adc is null)
            {
                adc = new Adc { Name = adcName };
                
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AdcManufacturer);
                    adc.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Adces.Add(adc);
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && adc.ManufacturerId is null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AdcManufacturer);
                if (manufacturer is not null)
                {
                    adc.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return adc;
        }

        public async Task<Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null)
        {
            var normalizedWireName = wireName.Trim();

            var wire = await _context.Wires
                .Include(w => w.Manufacturer)
                .FirstOrDefaultAsync(w => w.Name == normalizedWireName);
            
            if (wire is null)
            {
                wire = new Wire { Name = wireName };
                
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.WireManufacturer);
                    wire.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Wires.Add(wire);
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && wire.ManufacturerId is null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.WireManufacturer);
                if (manufacturer is not null)
                {
                    wire.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return wire;
        }

        public async Task<Manufacturer?> FindOrCreateManufacturerAsync(string manufacturerName, EntityType manufacturerType)
        {
            if (string.IsNullOrWhiteSpace(manufacturerName))
                return null;

            var normalizedManufacturerName = manufacturerName.Trim();

            var existingManufacturer = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name == normalizedManufacturerName && m.Type == manufacturerType);

            if (existingManufacturer is not null)
                return existingManufacturer;

            var existingByName = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name == normalizedManufacturerName);

            if (existingByName is not null)
            {
                if (existingByName.Type != manufacturerType)
                {
                    existingByName.Type = manufacturerType;
                    await _context.SaveChangesAsync();
                }
                return existingByName;
            }

            var newManufacturer = new Manufacturer
            {
                Name = manufacturerName.Trim(),
                Type = manufacturerType
            };

            _context.Manufacturer.Add(newManufacturer);
            await _context.SaveChangesAsync();

            return newManufacturer;
        }
    }
}

