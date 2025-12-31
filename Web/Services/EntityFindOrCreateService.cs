using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Models;

namespace Web.Services
{
    public class EntityFindOrCreateService
    {
        private readonly DMADbContext _context;

        public EntityFindOrCreateService(DMADbContext context)
        {
            _context = context;
        }

        public async Task<Year> FindOrCreateYearAsync(int yearValue)
        {
            var year = await _context.Years.FirstOrDefaultAsync(y => y.Value == yearValue);
            if (year == null)
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
            if (reissue == null)
            {
                reissue = new Reissue { Value = reissueValue };
                _context.Reissues.Add(reissue);
                await _context.SaveChangesAsync();
            }
            return reissue;
        }

        public async Task<Country> FindOrCreateCountryAsync(string countryName)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToLower() == countryName.ToLower());
            if (country == null)
            {
                country = new Country { Name = countryName };
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
            }
            return country;
        }

        public async Task<Label> FindOrCreateLabelAsync(string labelName)
        {
            var label = await _context.Labels.FirstOrDefaultAsync(l => l.Name.ToLower() == labelName.ToLower());
            if (label == null)
            {
                label = new Label { Name = labelName };
                _context.Labels.Add(label);
                await _context.SaveChangesAsync();
            }
            return label;
        }

        public async Task<Storage> FindOrCreateStorageAsync(string storageData)
        {
            var storage = await _context.Storages.FirstOrDefaultAsync(s => s.Data.ToLower() == storageData.ToLower());
            if (storage == null)
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
            if (bitness == null)
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
            if (sampling == null)
            {
                sampling = new Sampling { Value = samplingValue };
                _context.Samplings.Add(sampling);
                await _context.SaveChangesAsync();
            }
            return sampling;
        }

        public async Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName)
        {
            var format = await _context.DigitalFormats.FirstOrDefaultAsync(f => f.Name.ToLower() == formatName.ToLower());
            if (format == null)
            {
                format = new DigitalFormat { Name = formatName };
                _context.DigitalFormats.Add(format);
                await _context.SaveChangesAsync();
            }
            return format;
        }

        public async Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName)
        {
            var format = await _context.SourceFormats.FirstOrDefaultAsync(f => f.Name.ToLower() == formatName.ToLower());
            if (format == null)
            {
                format = new SourceFormat { Name = formatName };
                _context.SourceFormats.Add(format);
                await _context.SaveChangesAsync();
            }
            return format;
        }

        public async Task<VinylState> FindOrCreateVinylStateAsync(string stateName)
        {
            var state = await _context.VinylStates.FirstOrDefaultAsync(v => v.Name.ToLower() == stateName.ToLower());
            if (state == null)
            {
                state = new VinylState { Name = stateName };
                _context.VinylStates.Add(state);
                await _context.SaveChangesAsync();
            }
            return state;
        }

        public async Task<Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null)
        {
            var player = await _context.Players
                .Include(p => p.Manufacturer)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == playerName.ToLower());
            
            if (player == null)
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
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && player.ManufacturerId == null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.PlayerManufacturer);
                if (manufacturer != null)
                {
                    player.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return player;
        }

        public async Task<Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null)
        {
            var cartridge = await _context.Cartridges
                .Include(c => c.Manufacturer)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == cartridgeName.ToLower());
            
            if (cartridge == null)
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
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && cartridge.ManufacturerId == null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.CartridgeManufacturer);
                if (manufacturer != null)
                {
                    cartridge.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return cartridge;
        }

        public async Task<Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null)
        {
            var amplifier = await _context.Amplifiers
                .Include(a => a.Manufacturer)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == amplifierName.ToLower());
            
            if (amplifier == null)
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
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && amplifier.ManufacturerId == null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AmplifierManufacturer);
                if (manufacturer != null)
                {
                    amplifier.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return amplifier;
        }

        public async Task<Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null)
        {
            var adc = await _context.Adces
                .Include(a => a.Manufacturer)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == adcName.ToLower());
            
            if (adc == null)
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
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && adc.ManufacturerId == null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AdcManufacturer);
                if (manufacturer != null)
                {
                    adc.ManufacturerId = manufacturer.Id;
                    await _context.SaveChangesAsync();
                }
            }
            
            return adc;
        }

        public async Task<Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null)
        {
            var wire = await _context.Wires
                .Include(w => w.Manufacturer)
                .FirstOrDefaultAsync(w => w.Name.ToLower() == wireName.ToLower());
            
            if (wire == null)
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
            else if (!string.IsNullOrWhiteSpace(manufacturerName) && wire.ManufacturerId == null)
            {
                var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.WireManufacturer);
                if (manufacturer != null)
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

            var existingManufacturer = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name.ToLower() == manufacturerName.ToLower() && m.Type == manufacturerType);

            if (existingManufacturer != null)
                return existingManufacturer;

            var existingByName = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name.ToLower() == manufacturerName.ToLower());

            if (existingByName != null)
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

