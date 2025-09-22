using Microsoft.EntityFrameworkCore;
using Web.Enums;
using Web.Models;
using Web.ViewModels;

namespace Web.Db.Implementation
{
    public class TechnicalnfoRepository : ITechInfoRepository
    {
        private readonly DMADbContext _context;

        public TechnicalnfoRepository(DMADbContext ctx) => _context = ctx;

        public IQueryable<Adc> Adcs => _context.Adces;
        public IQueryable<AdcManufacturer> AdcManufacturers => _context.AdcManufacturers;
        public IQueryable<Amplifier> Amplifiers => _context.Amplifiers;
        public IQueryable<AmplifierManufacturer> AmplifierManufacturers => _context.AmplifierManufacturers;
        public IQueryable<Bitness> Bitnesses => _context.Bitnesses;
        public IQueryable<Cartridge> Cartridges => _context.Cartridges;
        public IQueryable<CartridgeManufacturer> CartridgeManufacturers => _context.CartridgeManufacturers;
        public IQueryable<DigitalFormat> DigitalFormats => _context.DigitalFormats;
        public IQueryable<Player> Players => _context.Players;
        public IQueryable<PlayerManufacturer> PlayerManufacturers => _context.PlayerManufacturers;
        public IQueryable<SourceFormat> SourceFormats => _context.SourceFormats;
        public IQueryable<Sampling> Samplings => _context.Samplings;
        public IQueryable<VinylState> VinylStates => _context.VinylStates;
        public IQueryable<Wire> Wires => _context.Wires;
        public IQueryable<WireManufacturer> WireManufacturers => _context.WireManufacturers;
        public IQueryable<TechnicalInfo> TechInfos => _context.TechnicalInfos;

        public async Task<TechnicalInfo> CreateOrUpdateTechnicalInfoAsync(Album album, AlbumCreateUpdateViewModel request)
        {
            var tinfo = await _context.TechnicalInfos.FirstOrDefaultAsync(x => x.AlbumId == album.Id);

            if (tinfo == null)
            {
                tinfo = new();
                album.TechnicalInfo = tinfo;
                await _context.TechnicalInfos.AddAsync(tinfo);
            }

            // adc
            if (string.IsNullOrWhiteSpace(request.Adc))
            {
                tinfo.AdcId = null;
            } 
            else
            {
                tinfo.Adc = await CreateOrUpdateAdcAsync(request.Adc, request.AdcManufacturer);
            }

            // amplifier
            if (string.IsNullOrWhiteSpace(request.Amplifier))
            {
                tinfo.AmplifierId = null;
            }
            else
            {
                tinfo.Amplifier = await CreateOrUpdateAmplifierAsync(request.Amplifier, request.AmplifierManufacturer);
            }

            // cartridge
            if (string.IsNullOrWhiteSpace(request.Cartridge))
            {
                tinfo.CartridgeId = null;
            }
            else
            {
                tinfo.Cartridge = await CreateOrUpdateCartridgeAsync(request.Cartridge, request.CartridgeManufacturer);
            }
                
            // player
            if (string.IsNullOrWhiteSpace(request.Player))
            {
                tinfo.PlayerId = null;
            }
            else
            {
                tinfo.Player = await CreateOrUpdatePlayerAsync(request.Player, request.PlayerManufacturer);
            }

            // wire
            if (string.IsNullOrWhiteSpace(request.Wire))
            {
                tinfo.WireId = null;
            }
            else
            {
                tinfo.Wire = await CreateOrUpdateWireAsync(request.Wire, request.WireManufacturer);
            }

            // bitness
            if (!request.Bitness.HasValue)
            {
                tinfo.BitnessId = null;
            }
            else
            {
                tinfo.Bitness = await CreateOrUpdateBitnessAsync(request.Bitness.Value);
            }

            // digital format
            if (string.IsNullOrWhiteSpace(request.DigitalFormat))
            {
                tinfo.DigitalFormatId = null;
            }
            else
            {
                tinfo.DigitalFormat = await CreateOrUpdateDigitalFormatAsync(request.DigitalFormat);
            }

            // source format
            if (string.IsNullOrWhiteSpace(request.SourceFormat))
            {
                tinfo.SourceFormatId = null;
            }
            else
            {
                tinfo.SourceFormat = await CreateOrUpdateSourceFormatAsync(request.SourceFormat);
            }

            // vinyl state
            if (string.IsNullOrWhiteSpace(request.VinylState))
            {
                tinfo.VinylStateId = null;
            }
            else
            {
                tinfo.VinylState = await CreateOrUpdateVinylStateAsync(request.VinylState);
            }

            // sampling
            if (!request.Sampling.HasValue)
            {
                tinfo.SamplingId = null;
            }
            else
            {
                tinfo.Sampling = await CreateOrUpdateSamplingAsync(request.Sampling.Value);
            }

            await _context.SaveChangesAsync();

            return tinfo;
        }

        #region #Find Or Create Methods

        private async Task<Adc> CreateOrUpdateAdcAsync(string model, string? manufacturer, string? description = null, int id = 0)
        {
            var _adc = id > 0 ? await _context.Adces.FirstOrDefaultAsync(x => x.Id == id) : await _context.Adces.FirstOrDefaultAsync(x => x.Data == model);

            if (_adc == null)
            {
                _adc = new() { Data = model, Description =  description};
                await _context.Adces.AddAsync(_adc);
            }

            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                _adc.ManufacturerId = null;
            }
            else
            {
                var _manufacturer = await _context.AdcManufacturers.FirstOrDefaultAsync(x => x.Data == manufacturer);

                if (_manufacturer == null)
                {
                    _manufacturer = new() { Data = manufacturer };
                    await _context.AdcManufacturers.AddAsync(_manufacturer);
                }
                
                _adc.Manufacturer = _manufacturer;
            }

            _adc.Data = model;
            _adc.Description = description;

            await _context.SaveChangesAsync();
            return _adc;
        }

        private async Task<Amplifier> CreateOrUpdateAmplifierAsync(string model, string? manufacturer, string? description = null, int id = 0)
        {
            var _amp = id > 0 ? await _context.Amplifiers.FirstOrDefaultAsync(x => x.Id == id) :await _context.Amplifiers.FirstOrDefaultAsync(x => x.Data == model);

            if (_amp == null)
            {
                _amp = new() { Data = model, Description = description };
                await _context.Amplifiers.AddAsync(_amp);
            }

            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                _amp.ManufacturerId = null;
            }
            else
            {
                var _manufacturer = await _context.AmplifierManufacturers.FirstOrDefaultAsync(x => x.Data == manufacturer);

                if (_manufacturer == null)
                {
                    _manufacturer = new() { Data = manufacturer };
                    await _context.AmplifierManufacturers.AddAsync(_manufacturer);
                }

                _amp.Manufacturer = _manufacturer;
            }

            _amp.Data = model;
            _amp.Description = description;

            await _context.SaveChangesAsync();
            return _amp;
        }

        private async Task<Cartridge> CreateOrUpdateCartridgeAsync(string model, string? manufacturer, string? description = null, int id = 0)
        {
            var _cartridge = id > 0 ? await _context.Cartridges.FirstOrDefaultAsync(x => x.Id == id) : await _context.Cartridges.FirstOrDefaultAsync(x => x.Data == model);

            if (_cartridge == null)
            {
                _cartridge = new() { Data = model, Description = description };
                await _context.Cartridges.AddAsync(_cartridge);
            }

            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                _cartridge.ManufacturerId = null;
            }
            else
            {
                var _manufacturer = await _context.CartridgeManufacturers.FirstOrDefaultAsync(x => x.Data == manufacturer);

                if (_manufacturer == null)
                {
                    _manufacturer = new() { Data = manufacturer };
                    await _context.CartridgeManufacturers.AddAsync(_manufacturer);
                }

                _cartridge.Manufacturer = _manufacturer;
            }

            _cartridge.Data = model;
            _cartridge.Description = description;

            await _context.SaveChangesAsync();
            return _cartridge;
        }

        private async Task<Player> CreateOrUpdatePlayerAsync(string model, string? manufacturer, string? description = null, int id = 0)
        {
            var _player = id > 0 ? await _context.Players.FirstOrDefaultAsync(x => x.Id == id) : await _context.Players.FirstOrDefaultAsync(x => x.Data == model);

            if (_player == null)
            {
                _player = new() { Data = model, Description = description };
                await _context.Players.AddAsync(_player);
            }

            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                _player.ManufacturerId = null;
            }
            else
            {
                var _manufacturer = await _context.PlayerManufacturers.FirstOrDefaultAsync(x => x.Data == manufacturer);

                if (_manufacturer == null)
                {
                    _manufacturer = new() { Data = manufacturer };
                    await _context.PlayerManufacturers.AddAsync(_manufacturer);
                }

                _player.Manufacturer = _manufacturer;
            }

            _player.Data = model;
            _player.Description = description;

            await _context.SaveChangesAsync();
            return _player;
        }

        private async Task<Wire> CreateOrUpdateWireAsync(string model, string? manufacturer, string? description = null, int id = 0)
        {
            var _wire = id > 0 ? await _context.Wires.FirstOrDefaultAsync(x => x.Id == id) : await _context.Wires.FirstOrDefaultAsync(x => x.Data == model);

            if (_wire == null)
            {
                _wire = new() { Data = model, Description = description };
                await _context.Wires.AddAsync(_wire);
            }

            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                _wire.ManufacturerId = null;
            }
            else
            {
                var _manufacturer = await _context.WireManufacturers.FirstOrDefaultAsync(x => x.Data == manufacturer);

                if (_manufacturer == null)
                {
                    _manufacturer = new() { Data = manufacturer };
                    await _context.WireManufacturers.AddAsync(_manufacturer);
                }

                _wire.Manufacturer = _manufacturer;
            }

            _wire.Data = model;
            _wire.Description = description;

            await _context.SaveChangesAsync();
            return _wire;
        }

        private async Task<Bitness> CreateOrUpdateBitnessAsync(int bitness)
        {
            var _bitness = await _context.Bitnesses.FirstOrDefaultAsync(x => x.Data == bitness);

            if (_bitness == null)
            {
                _bitness = new() { Data = bitness };
                await _context.Bitnesses.AddAsync(_bitness);
            }

            return _bitness;
        }

        private async Task<DigitalFormat> CreateOrUpdateDigitalFormatAsync(string digitalFormat)
        {
            var _digitalFormat = await _context.DigitalFormats.FirstOrDefaultAsync(x => x.Data == digitalFormat);

            if (_digitalFormat == null)
            {
                _digitalFormat = new() { Data = digitalFormat };
                await _context.DigitalFormats.AddAsync(_digitalFormat);
            }

            await _context.SaveChangesAsync();
            return _digitalFormat;
        }

        private async Task<SourceFormat> CreateOrUpdateSourceFormatAsync(string sourceFormat)
        {
            var _sourceFormat = await _context.SourceFormats.FirstOrDefaultAsync(x => x.Data == sourceFormat);

            if (_sourceFormat == null)
            {
                _sourceFormat = new() { Data = sourceFormat };
                await _context.SourceFormats.AddAsync(_sourceFormat);
            }

            await _context.SaveChangesAsync();
            return _sourceFormat;
        }

        private async Task<VinylState> CreateOrUpdateVinylStateAsync(string vinylState)
        {
            var _vinylState = await _context.VinylStates.FirstOrDefaultAsync(x => x.Data == vinylState);

            if (_vinylState == null)
            {
                _vinylState = new() { Data = vinylState };
                await _context.VinylStates.AddAsync(_vinylState);
            }

            await _context.SaveChangesAsync();
            return _vinylState;
        }

        private async Task<Sampling> CreateOrUpdateSamplingAsync(double sampling)
        {
            var _sampling = await _context.Samplings.FirstOrDefaultAsync(x => x.Data == sampling);

            if (_sampling == null)
            {
                _sampling = new() { Data = sampling };
                await _context.Samplings.AddAsync(_sampling);
            }

            await _context.SaveChangesAsync();
            return _sampling;
        }

        public async Task<TechnicalInfo?> GetByIdAsync(int id)
        {
            return await _context.TechnicalInfos
                .Include(x => x.VinylState)
                .Include(x => x.DigitalFormat)
                .Include(x => x.Bitness)
                .Include(x => x.Sampling)
                .Include(x => x.SourceFormat)
                .Include(x => x.Player)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Cartridge)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Amplifier)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Adc)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Adc)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Wire)
                .ThenInclude(x => x.Manufacturer)
                .FirstOrDefaultAsync(x => x.AlbumId == id);
        }
        #endregion

        #region Equipment methods
        public async Task<int> CreateOrUpdateEquipmentAsync(EquipmentViewModel request)
        {
            switch (request.EquipmentType)
            {
                case EntityType.Adc:
                    var adc = await CreateOrUpdateAdcAsync(request.ModelName, request.Manufacturer, request.Description, request.EquipmentId);
                    return adc.Id;
                case EntityType.Amplifier:
                    var amp = await CreateOrUpdateAmplifierAsync(request.ModelName, request.Manufacturer, request.Description, request.EquipmentId);
                    return amp.Id;
                case EntityType.Cartridge:
                    var cartridge = await CreateOrUpdateCartridgeAsync(request.ModelName, request.Manufacturer, request.Description, request.EquipmentId);
                    return cartridge.Id;
                case EntityType.Player:
                    var player = await CreateOrUpdatePlayerAsync(request.ModelName, request.Manufacturer, request.Description, request.EquipmentId);
                    return player.Id;
                case EntityType.Wire:
                    var wire = await CreateOrUpdateWireAsync(request.ModelName, request.Manufacturer, request.Description, request.EquipmentId);
                    return wire.Id;
                default:
                    return 0;
            }
        }        
        #endregion
    }
}
