using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Request;

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
        public IQueryable<Cartrige> Cartriges => _context.Cartriges;
        public IQueryable<CartrigeManufacturer> CartrigeManufacturers => _context.CartrigeManufacturers;
        public IQueryable<DigitalFormat> DigitalFormats => _context.DigitalFormats;
        public IQueryable<Player> Players => _context.Players;
        public IQueryable<PlayerManufacturer> PlayerManufacturers => _context.PlayerManufacturers;
        public IQueryable<SourceFormat> SourceFormats => _context.SourceFormats;
        public IQueryable<Processing> Processings => _context.Processings;
        public IQueryable<Sampling> Samplings => _context.Samplings;
        public IQueryable<VinylState> VinylStates => _context.VinylStates;
        public IQueryable<Wire> Wires => _context.Wires;
        public IQueryable<WireManufacturer> WireManufacturers => _context.WireManufacturers;
        public IQueryable<TechnicalInfo> TechInfos => _context.TechnicalInfos;

        public async Task<TechnicalInfo> CreateOrUpdateTechnicalInfoAsync(Album album, AlbumDataRequest request)
        {
            var tinfo = await _context.TechnicalInfos.FirstOrDefaultAsync(x => x.AlbumId == album.Id);

            if (tinfo == null)
            {
                tinfo = new();
                album.TechnicalInfo = tinfo;
                await _context.TechnicalInfos.AddAsync(tinfo);
            }
            
            await CreateOrUpdateAdcAsync(tinfo, request);
            await CreareOrUpdateAmplifierAsync(tinfo, request);
            await CreateOrUpdateCartridgeAsync(tinfo, request);
            await CreateOrUpdatePlayerAsync(tinfo, request);
            await CreateOrUpdateWireAsync(tinfo, request);
            await CreateOrUpdateBitnessAsync(tinfo, request);
            await CreateOrUpdateDigitalFormatAsync(tinfo, request);
            await CreateOrUpdateSourceFormatAsync(tinfo, request);
            await CreateOrUpdateProcessingAsync(tinfo, request);
            await CreateOrUpdateVinylStateAsync(tinfo, request);
            await CreateOrUpdateSamplingAsync(tinfo, request);                  

            await _context.SaveChangesAsync();

            return tinfo;
        }

        #region #Find Or Create Methods

        private async Task CreateOrUpdateAdcAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Adc))
            {
                tinfo.AdcId = null;
            }
            else
            {
                var _adc = await _context.Adces.FirstOrDefaultAsync(x => x.Data == request.Adc);

                if (_adc == null)
                {
                    _adc = new() { Data = request.Adc };
                    await _context.Adces.AddAsync(_adc);
                }

                if (!string.IsNullOrWhiteSpace(request.AdcManufacturer))
                {
                    var _manufacturer = await _context.AdcManufacturers.FirstOrDefaultAsync(x => x.Data == request.AdcManufacturer);

                    if (_manufacturer == null)
                    {
                        _manufacturer = new() { Data = request.AdcManufacturer };
                        await _context.AdcManufacturers.AddAsync(_manufacturer);
                    }

                    _adc.Manufacturer = _manufacturer;
                }

                tinfo.Adc = _adc;
            }
        }

        private async Task CreareOrUpdateAmplifierAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Amplifier))
            {
                tinfo.AmplifierId = null;
            }
            else
            {
                var _amp = await _context.Amplifiers.FirstOrDefaultAsync(x => x.Data == request.Amplifier);
                
                if (_amp == null)
                {
                    _amp = new () { Data = request.Amplifier };
                    await _context.Amplifiers.AddAsync(_amp);
                }

                if (!string.IsNullOrWhiteSpace(request.AmplifierManufacturer))
                {
                    var manufacturer = await _context.AmplifierManufacturers.FirstOrDefaultAsync(x => x.Data == request.AmplifierManufacturer);

                    if (manufacturer == null)
                    {
                        manufacturer = new () { Data = request.AmplifierManufacturer };
                        await _context.AmplifierManufacturers.AddAsync(manufacturer);
                    }
                    
                    _amp.Manufacturer = manufacturer;
                }

                tinfo.Amplifier = _amp;
            }
        }

        private async Task CreateOrUpdateCartridgeAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Cartridge))
            {
                tinfo.CartrigeId = null;
            }
            else
            {
                var _cartridge = await _context.Cartriges.FirstOrDefaultAsync(x => x.Data == request.Cartridge);

                if (_cartridge == null)
                {
                    _cartridge = new () { Data = request.Cartridge };
                    await _context.Cartriges.AddAsync(_cartridge);
                }

                if (!string.IsNullOrWhiteSpace(request.CartridgeManufacturer))
                {
                    var _manufacturer = await _context.CartrigeManufacturers.FirstOrDefaultAsync(x => x.Data == request.CartridgeManufacturer);

                    if (_manufacturer == null)
                    {
                        _manufacturer = new () { Data = request.CartridgeManufacturer };
                        await _context.CartrigeManufacturers.AddAsync(_manufacturer);
                    }

                    _cartridge.Manufacturer = _manufacturer;
                }

                tinfo.Cartrige = _cartridge;
            }
        }

        private async Task CreateOrUpdatePlayerAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Player))
            {
                tinfo.PlayerId = null; 
            }
            else
            {
                var _player = await _context.Players.FirstOrDefaultAsync(x => x.Data == request.Player);

                if (_player == null)
                {
                    _player = new () { Data = request.Player };
                    await _context.Players.AddAsync(_player);
                }

                if (!string.IsNullOrWhiteSpace(request.PlayerManufacturer))
                {
                    var _manufacturer = await _context.PlayerManufacturers.FirstOrDefaultAsync(x => x.Data == request.PlayerManufacturer);

                    if (_manufacturer == null)
                    {
                        _manufacturer = new () { Data = request.PlayerManufacturer };
                        await _context.PlayerManufacturers.AddAsync(_manufacturer);
                    }

                    _player.Manufacturer = _manufacturer;
                }

                tinfo.Player = _player;
            }
        }

        private async Task CreateOrUpdateWireAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Wire))
            {
                tinfo.WireId = null;
            }
            else
            {
                var _wire = await _context.Wires.FirstOrDefaultAsync(x => x.Data == request.Wire);

                if (_wire == null)
                {
                    _wire = new () { Data = request.Wire };
                    await _context.Wires.AddAsync(_wire);
                }

                if (!string.IsNullOrWhiteSpace(request.WireManufacturer))
                {
                    var _manufacturer = await _context.WireManufacturers.FirstOrDefaultAsync(x => x.Data == request.WireManufacturer);

                    if (_manufacturer == null)
                    {
                        _manufacturer = new () { Data = request.WireManufacturer };
                        await _context.WireManufacturers.AddAsync(_manufacturer);
                    }

                    _wire.Manufacturer = _manufacturer;
                }

                tinfo.Wire = _wire;
            }
        }

        private async Task CreateOrUpdateBitnessAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (request.Bitness == null)
            {
                tinfo.BitnessId = null;
            }
            else
            {
                var _bitness = await _context.Bitnesses.FirstOrDefaultAsync(x => x.Data == request.Bitness);

                if (_bitness == null)
                {
                    _bitness = new () { Data = request.Bitness.Value };
                    await _context.Bitnesses.AddAsync(_bitness);
                }

                tinfo.Bitness = _bitness;
            }
        }

        private async Task CreateOrUpdateDigitalFormatAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DigitalFormat))
            {
                tinfo.DigitalFormatId = null;
            }
            else
            {
                var _digitalFormat = await _context.DigitalFormats.FirstOrDefaultAsync(x => x.Data == request.DigitalFormat);

                if (_digitalFormat == null)
                {
                    _digitalFormat = new () { Data = request.DigitalFormat };
                    await _context.DigitalFormats.AddAsync(_digitalFormat);
                }

                tinfo.DigitalFormat = _digitalFormat;
            }
        }

        private async Task CreateOrUpdateSourceFormatAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SourceFormat))
            {
                tinfo.SourceFormatId = null;
            }
            else
            {
                var _sourceFormat = await _context.SourceFormats.FirstOrDefaultAsync(x => x.Data == request.SourceFormat);

                if (_sourceFormat == null)
                {
                    _sourceFormat = new () { Data = request.SourceFormat };
                    await _context.SourceFormats.AddAsync(_sourceFormat);
                }

                tinfo.SourceFormat = _sourceFormat;
            }
        }

        private async Task CreateOrUpdateProcessingAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Processing))
            {
                tinfo.ProcessingId = null;
            }
            else
            {
                var _processing = await _context.Processings.FirstOrDefaultAsync(x => x.Data == request.Processing);

                if (_processing == null)
                {
                    _processing = new () { Data = request.Processing };
                    await _context.Processings.AddAsync(_processing);
                }

                tinfo.Processing = _processing;
            }
        }

        private async Task CreateOrUpdateVinylStateAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.VinylState))
            {
                tinfo.VinylStateId = null;
            }
            else
            {
                var _vinylState = await _context.VinylStates.FirstOrDefaultAsync(x => x.Data == request.VinylState);

                if (_vinylState == null)
                {
                    _vinylState = new () { Data = request.VinylState };
                    await _context.VinylStates.AddAsync(_vinylState);   
                }

                tinfo.VinylState = _vinylState;
            }
        }

        private async Task CreateOrUpdateSamplingAsync(TechnicalInfo tinfo, AlbumDataRequest request)
        {
            if (request.Sampling == null)
            {
                tinfo.SamplingId = null;
            }
            else
            {
                var _sampling = await _context.Samplings.FirstOrDefaultAsync(x => x.Data == request.Sampling);

                if (_sampling == null)
                {
                    _sampling = new () { Data = request.Sampling.Value };
                    await _context.Samplings.AddAsync(_sampling);
                }

                tinfo.Sampling = _sampling;
            }
        }
        #endregion
    }
}
